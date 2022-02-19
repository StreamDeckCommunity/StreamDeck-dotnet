namespace StreamDeck
{
    using System;
    using System.Text.Json;
    using System.Text.Json.Nodes;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using StreamDeck.Events.Received;
    using StreamDeck.Events.Sent;
    using StreamDeck.Net;
    using StreamDeck.Serialization;

    /// <summary>
    /// Provides a connection between Elgato Stream Deck devices and a Stream Deck client.
    /// </summary>
    public sealed class StreamDeckConnection : IStreamDeckConnection, IStreamDeckConnectionManager
    {
        /// <inheritdoc/>
        public event EventHandler<StreamDeckEventArgs<ApplicationPayload>> ApplicationDidLaunch;

        /// <inheritdoc/>
        public event EventHandler<StreamDeckEventArgs<ApplicationPayload>> ApplicationDidTerminate;

        /// <inheritdoc/>
        public event EventHandler<DeviceConnectEventArgs> DeviceDidConnect;

        /// <inheritdoc/>
        public event EventHandler<DeviceEventArgs> DeviceDidDisconnect;

        /// <inheritdoc/>
        public event EventHandler<StreamDeckEventArgs<SettingsPayload>> DidReceiveGlobalSettings;

        /// <inheritdoc/>
        public event EventHandler<ActionEventArgs<ActionPayload>> DidReceiveSettings;

        /// <inheritdoc/>
        public event EventHandler<ActionEventArgs<KeyPayload>> KeyDown;

        /// <inheritdoc/>
        public event EventHandler<ActionEventArgs<KeyPayload>> KeyUp;

        /// <inheritdoc/>
        public event EventHandler<ActionEventArgs> PropertyInspectorDidAppear;

        /// <inheritdoc/>
        public event EventHandler<ActionEventArgs> PropertyInspectorDidDisappear;

        /// <inheritdoc/>
        public event EventHandler<ActionEventArgs<JsonObject>> SendToPlugin;

        /// <inheritdoc/>
        public event EventHandler<StreamDeckEventArgs> SystemDidWakeUp;

        /// <inheritdoc/>
        public event EventHandler<ActionEventArgs<TitlePayload>> TitleParametersDidChange;

        /// <inheritdoc/>
        public event EventHandler<ActionEventArgs<AppearancePayload>> WillAppear;

        /// <inheritdoc/>
        public event EventHandler<ActionEventArgs<AppearancePayload>> WillDisappear;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckConnection"/> class.
        /// </summary>
        /// <param name="logger">The optional logger.</param>
        public StreamDeckConnection(ILogger<StreamDeckConnection> logger = null)
            : this(Environment.GetCommandLineArgs(), logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckConnection"/> class.
        /// </summary>
        /// <param name="args">The CLI arguments that contain the registration parameters.</param>
        /// <param name="logger">The optional logger.</param>
        public StreamDeckConnection(string[] args, ILogger<StreamDeckConnection> logger = null)
            : this(new RegistrationParameters(args), logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDeckConnection"/> class.
        /// </summary>
        /// <param name="registrationParameters">The registration parameters.</param>
        /// <param name="logger">The optional logger.</param>
        public StreamDeckConnection(RegistrationParameters registrationParameters, ILogger<StreamDeckConnection> logger = null)
        {
            this.Logger = logger;
            this.RegistrationParameters = registrationParameters;
        }

        /// <inheritdoc/>
        public RegistrationInfo RegistrationInfo => this.RegistrationParameters.Info;

        /// <summary>
        /// Gets the logger.
        /// </summary>
        private ILogger<StreamDeckConnection> Logger { get; }

        /// <summary>
        /// Gets or sets the registration parameters.
        /// </summary>
        private RegistrationParameters RegistrationParameters { get; set; }

        /// <summary>
        /// Gets or sets the web socket.
        /// </summary>
        private WebSocketConnection WebSocket { get; set; }

        /// <inheritdoc/>
        public async Task ConnectAsync(CancellationToken cancellationToken = default)
        {
            this.Logger?.LogTrace("Connecting to Stream Deck.");
            this.WebSocket = new WebSocketConnection($"ws://localhost:{this.RegistrationParameters.Port}/");
            this.WebSocket.MessageReceived += this.WebSocket_MessageReceived;

            await this.WebSocket.ConnectAsync();
            this.Logger?.LogTrace($"Connected to Stream Deck; registering plugin.");

            await this.WebSocket.SendJsonAsync(new RegistrationMessage(this.RegistrationParameters.Event, this.RegistrationParameters.PluginUUID), cancellationToken);
            this.Logger?.LogTrace($"Plugin registrered.");
        }

        /// <inheritdoc/>
        public Task DisconnectAsync()
            => this.WebSocket.DisconnectAsync();

        /// <inheritdoc/>
        public Task WaitForDisconnectAsync(CancellationToken cancellationToken = default)
            => this.WebSocket.WaitForDisconnectAsync(cancellationToken);

        /// <inheritdoc/>
        public async ValueTask DisposeAsync()
        {
            await (this.WebSocket?.DisconnectAsync() ?? Task.CompletedTask);
            this.WebSocket = null;
        }

        /// <inheritdoc/>
        public Task GetGlobalSettingsAsync(CancellationToken cancellationToken = default)
            => this.SendAsync(new ContextMessage("getGlobalSettings", this.RegistrationParameters.PluginUUID), cancellationToken);

        /// <inheritdoc/>
        public Task GetSettingsAsync(string context, CancellationToken cancellationToken = default)
            => this.SendAsync(new ContextMessage("getSettings", context), cancellationToken);

        /// <inheritdoc/>
        public Task LogMessageAsync(string msg, CancellationToken cancellationToken = default)
            => this.SendAsync(new Message<LogPayload>("logMessage", new LogPayload(msg)), cancellationToken);

        /// <inheritdoc/>
        public Task OpenUrlAsync(string url, CancellationToken cancellationToken = default)
            => this.SendAsync(new Message<UrlPayload>("openUrl", new UrlPayload(url)), cancellationToken);

        /// <inheritdoc/>
        public Task SendToPropertyInspectorAsync(string context, string action, object payload, CancellationToken cancellationToken = default)
            => this.SendAsync(new ActionMessage<object>("sendToPropertyInspector", context, action, payload), cancellationToken);

        /// <inheritdoc/>
        public Task SetGlobalSettingsAsync(object settings, CancellationToken cancellationToken = default)
            => this.SendAsync(new ContextMessage<object>("setGlobalSettings", this.RegistrationParameters.PluginUUID, settings), cancellationToken);

        /// <inheritdoc/>
        public Task SetImageAsync(string context, string image = "", Target target = Target.Both, int? state = null, CancellationToken cancellationToken = default)
            => this.SendAsync(new ContextMessage<SetImagePayload>("setImage", context, new SetImagePayload(image, target, state)), cancellationToken);

        /// <inheritdoc/>
        public Task SetSettingsAsync(string context, object settings, CancellationToken cancellationToken = default)
            => this.SendAsync(new ContextMessage<object>("setSettings", context, settings), cancellationToken);

        /// <inheritdoc/>
        public Task SetStateAsync(string context, int state = 0, CancellationToken cancellationToken = default)
            => this.SendAsync(new ContextMessage<SetStatePayload>("setState", context, new SetStatePayload(state)), cancellationToken);

        /// <inheritdoc/>
        public Task SetTitleAsync(string context, string title = "", Target target = Target.Both, int? state = null, CancellationToken cancellationToken = default)
            => this.SendAsync(new ContextMessage<SetTitlePayload>("setTitle", context, new SetTitlePayload(title, target, state)), cancellationToken);

        /// <inheritdoc/>
        public Task ShowAlertAsync(string context, CancellationToken cancellationToken = default)
            => this.SendAsync(new ContextMessage("showAlert", context), cancellationToken);

        /// <inheritdoc/>
        public Task ShowOkAsync(string context, CancellationToken cancellationToken = default)
            => this.SendAsync(new ContextMessage("showOk", context), cancellationToken);

        /// <inheritdoc/>
        public Task SwitchToProfileAsync(string context, string device, string profile = "", CancellationToken cancellationToken = default)
            => this.SendAsync(new DeviceMessage<SwitchToProfilePayload>("switchToProfile", context, device, new SwitchToProfilePayload(profile)), cancellationToken);

        /// <summary>
        /// Sends the value to the Stream Deck asynchronously.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The task of sending the value.</returns>
        private Task SendAsync(object value, CancellationToken cancellationToken)
            => this.WebSocket.SendJsonAsync(value, cancellationToken);

        /// <summary>
        /// Handles the <see cref="WebSocketConnection.MessageReceived"/> public event of <see cref="WebSocket"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="WebSocketMessageEventArgs"/> instance containing the public event data.</param>
        private void WebSocket_MessageReceived(object sender, WebSocketMessageEventArgs e)
        {
            try
            {
                var args = JsonSerializer.Deserialize(e.Message, StreamDeckJsonContext.Default.StreamDeckEventArgs);
                switch (args.Event)
                {
                    // Global.
                    case "applicationDidLaunch":
                        this.ApplicationDidLaunch?.Invoke(this, JsonSerializer.Deserialize(e.Message, StreamDeckJsonContext.Default.StreamDeckEventArgsApplicationPayload));
                        break;

                    case "applicationDidTerminate":
                        this.ApplicationDidTerminate?.Invoke(this, JsonSerializer.Deserialize(e.Message, StreamDeckJsonContext.Default.StreamDeckEventArgsApplicationPayload));
                        break;

                    case "deviceDidConnect":
                        this.DeviceDidConnect?.Invoke(this, JsonSerializer.Deserialize(e.Message, StreamDeckJsonContext.Default.DeviceConnectEventArgs));
                        break;

                    case "deviceDidDisconnect":
                        this.DeviceDidDisconnect?.Invoke(this, JsonSerializer.Deserialize(e.Message, StreamDeckJsonContext.Default.DeviceEventArgs));
                        break;

                    case "didReceiveGlobalSettings":
                        this.DidReceiveGlobalSettings?.Invoke(this, JsonSerializer.Deserialize(e.Message, StreamDeckJsonContext.Default.StreamDeckEventArgsSettingsPayload));
                        break;

                    case "systemDidWakeUp":
                        this.SystemDidWakeUp?.Invoke(this, JsonSerializer.Deserialize(e.Message, StreamDeckJsonContext.Default.StreamDeckEventArgs));
                        break;

                    // Action specific.
                    case "didReceiveSettings":
                        this.DidReceiveSettings?.Invoke(this, JsonSerializer.Deserialize(e.Message, StreamDeckJsonContext.Default.ActionEventArgsActionPayload));
                        break;

                    case "keyDown":
                        this.KeyDown?.Invoke(this, JsonSerializer.Deserialize(e.Message, StreamDeckJsonContext.Default.ActionEventArgsKeyPayload));
                        break;

                    case "keyUp":
                        this.KeyUp?.Invoke(this, JsonSerializer.Deserialize(e.Message, StreamDeckJsonContext.Default.ActionEventArgsKeyPayload));
                        break;

                    case "propertyInspectorDidAppear":
                        this.PropertyInspectorDidAppear?.Invoke(this, JsonSerializer.Deserialize(e.Message, StreamDeckJsonContext.Default.ActionEventArgs));
                        break;

                    case "propertyInspectorDidDisappear":
                        this.PropertyInspectorDidDisappear?.Invoke(this, JsonSerializer.Deserialize(e.Message, StreamDeckJsonContext.Default.ActionEventArgs));
                        break;

                    case "sendToPlugin":
                        this.SendToPlugin?.Invoke(this, JsonSerializer.Deserialize(e.Message, StreamDeckJsonContext.Default.ActionEventArgsJsonObject));
                        break;

                    case "titleParametersDidChange":
                        this.TitleParametersDidChange?.Invoke(this, JsonSerializer.Deserialize(e.Message, StreamDeckJsonContext.Default.ActionEventArgsTitlePayload));
                        break;

                    case "willAppear":
                        this.WillAppear?.Invoke(this, JsonSerializer.Deserialize(e.Message, StreamDeckJsonContext.Default.ActionEventArgsAppearancePayload));
                        break;

                    case "willDisappear":
                        this.WillDisappear?.Invoke(this, JsonSerializer.Deserialize(e.Message, StreamDeckJsonContext.Default.ActionEventArgsAppearancePayload));
                        break;

                    // Unrecognised
                    default:
                        throw new ArgumentException($"Unrecognised event: {args.Event}", nameof(args.Event));
                }
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex, ex.Message);
            }
        }
    }
}
