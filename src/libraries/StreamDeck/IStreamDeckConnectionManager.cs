namespace StreamDeck
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides connection management for the plugin, and the underlying connection with the Stream Deck.
    /// </summary>
    public interface IStreamDeckConnectionManager : IAsyncDisposable
    {
        /// <summary>
        /// Connects to the Stream Deck asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The optioanl cancellation token.</param>
        /// <returns>The task of connecting to the Stream Deck.</returns>
        public Task ConnectAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Disconnects from the Stream Deck asynchronously.
        /// </summary>
        /// <returns>The task of waiting of disconnecting.</returns>
        public Task DisconnectAsync();

        /// <summary>
        /// Waits for the underlying connection to disconnect asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The task of the live connection.</returns>
        public Task WaitForShutdownAsync(CancellationToken cancellationToken = default);
    }
}
