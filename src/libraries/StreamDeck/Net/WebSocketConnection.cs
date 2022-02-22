namespace StreamDeck.Net
{
    using System;
    using System.Net.WebSockets;
    using System.Text;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using StreamDeck.Serialization;

    /// <summary>
    /// Provides a light-weight wrapper for <see cref="ClientWebSocket"/>.
    /// </summary>
    public sealed class WebSocketConnection : IAsyncDisposable
    {
        /// <summary>
        /// The buffer size.
        /// </summary>
        private const int BUFFER_SIZE = 1024 * 1024;

        /// <summary>
        /// The process synchronize root.
        /// </summary>
        private readonly SemaphoreSlim _syncRoot = new SemaphoreSlim(1);

        /// <summary>
        /// Initializes a new instance of the <see cref="WebSocketConnection" /> class.
        /// </summary>
        /// <param name="uri">The URI.</param>
        public WebSocketConnection(string uri)
            => this.Uri = new Uri(uri);

        /// <summary>
        /// Occurs when a message is received.
        /// </summary>
        public event EventHandler<WebSocketMessageEventArgs>? MessageReceived;

        /// <summary>
        /// Gets or sets the encoding.
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.UTF8;

        /// <summary>
        /// Gets the URI.
        /// </summary>
        public Uri Uri { get; }

        /// <summary>
        /// Gets the connection task completion source.
        /// </summary>
        private TaskCompletionSource<WebSocketCloseStatus> ConnectionTaskCompletionSource { get; } = new TaskCompletionSource<WebSocketCloseStatus>();

        /// <summary>
        /// Gets or sets the web socket.
        /// </summary>
        private ClientWebSocket? WebSocket { get; set; }

        /// <summary>
        /// Connects the web socket.
        /// </summary>
        public async Task ConnectAsync(CancellationToken cancellationToken = default)
        {
            if (this.WebSocket == null)
            {
                // Connect the web socket.
                this.WebSocket = new ClientWebSocket();
                await this.WebSocket.ConnectAsync(this.Uri, cancellationToken);

                // Asynchronously listen for messages.
                _ = Task.Factory.StartNew(
                    async () => this.ConnectionTaskCompletionSource.TrySetResult(await this.ReceiveAsync(cancellationToken)),
                    cancellationToken,
                    TaskCreationOptions.LongRunning | TaskCreationOptions.RunContinuationsAsynchronously,
                    TaskScheduler.Default);
            }
        }

        /// <summary>
        /// Disconnects the web socket.
        /// </summary>
        public async Task DisconnectAsync()
        {
            if (this.WebSocket != null)
            {
                var socket = this.WebSocket;
                this.WebSocket = null;

                if (socket.State == WebSocketState.Open)
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Disconnecting", CancellationToken.None);
                }

                socket.Dispose();
                this.ConnectionTaskCompletionSource?.TrySetResult(WebSocketCloseStatus.NormalClosure);
            }
        }

        /// <inheritdoc/>
        public async ValueTask DisposeAsync()
        {
            await this.DisconnectAsync();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Sends the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task SendAsync(string message, CancellationToken cancellationToken)
        {
            if (this.WebSocket == null
                || this.WebSocket.State != WebSocketState.Open)
            {
                throw new InvalidOperationException("The web socket is not connected.");
            }

            try
            {
                await this._syncRoot.WaitAsync(cancellationToken);

                var buffer = this.Encoding.GetBytes(message);
                await this.WebSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, cancellationToken);
            }
            finally
            {
                this._syncRoot.Release();
            }
        }

        /// <summary>
        /// Serializes the value, and sends the message asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The value to serialize and send.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public Task SendJsonAsync<T>(T value, CancellationToken cancellationToken)
        {
            var json = JsonSerializer.Serialize(value, typeof(T), StreamDeckJsonContext.Default);
            return this.SendAsync(json, cancellationToken);
        }

        /// <summary>
        /// Waits for the underlying connection to disconnect asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The task of the live connection.</returns>
        public Task WaitForDisconnectAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.Register(() => this.ConnectionTaskCompletionSource.TrySetCanceled(), useSynchronizationContext: false);
            return this.ConnectionTaskCompletionSource.Task;
        }

        /// <summary>
        /// Receive data as an asynchronous operation.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        private async Task<WebSocketCloseStatus> ReceiveAsync(CancellationToken cancellationToken)
        {
            var buffer = new byte[BUFFER_SIZE];
            var textBuffer = new StringBuilder(BUFFER_SIZE);

            try
            {
                while (this.WebSocket?.State == WebSocketState.Open
                    && !cancellationToken.IsCancellationRequested)
                {
                    // Await a message.
                    var result = await this.WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
                    if (result == null)
                    {
                        continue;
                    }

                    if (result.MessageType == WebSocketMessageType.Close
                        || (result.CloseStatus != null && result.CloseStatus.HasValue && result.CloseStatus.Value != WebSocketCloseStatus.Empty))
                    {
                        // Stop listening, and return the close status.
                        return result.CloseStatus.GetValueOrDefault();
                    }
                    else if (result.MessageType == WebSocketMessageType.Text)
                    {
                        // Append to the text buffer, and determine if the message has finished
                        textBuffer.Append(this.Encoding.GetString(buffer, 0, result.Count));
                        if (result.EndOfMessage)
                        {
                            this.MessageReceived?.Invoke(this, new WebSocketMessageEventArgs(textBuffer.ToString()));
                            textBuffer.Clear();
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                return WebSocketCloseStatus.NormalClosure;
            }
            catch (Exception)
            {
                return WebSocketCloseStatus.InternalServerError;
            }

            return WebSocketCloseStatus.NormalClosure;
        }
    }
}
