namespace StreamDeck.Extensions
{
    using System;
    using System.Text.Json;
    using System.Text.Json.Serialization.Metadata;
    using System.Threading;
    using System.Threading.Tasks;
    using StreamDeck.Events.Received;
    using StreamDeck.Serialization;

    /// <summary>
    /// Provides extension methods for <see cref="IStreamDeckConnection"/>.
    /// </summary>
    public static class StreamDeckConnectionExtensions
    {
        /// <summary>
        /// Requests the persistent global data stored for the plugin.
        /// </summary>
        /// <typeparam name="T">The type of the settings.</typeparam>
        /// <param name="connection">The connection with the Stream Deck.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The global settings.</returns>
        public static Task<T> GetGlobalSettingsAsync<T>(this IStreamDeckConnection connection, CancellationToken cancellationToken = default)
            => connection.GetGlobalSettingsAsync(settings => settings.Deserialize<T>(StreamDeckJsonContext.Default.Options), cancellationToken);

        /// <summary>
        /// Requests the persistent global data stored for the plugin.
        /// </summary>
        /// <typeparam name="T">The type of the settings.</typeparam>
        /// <param name="connection">The connection with the Stream Deck.</param>
        /// <param name="jsonTypeInfo">The JSON type information used to deserialize the global settings.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The global settings.</returns>
        public static Task<T> GetGlobalSettingsAsync<T>(this IStreamDeckConnection connection, JsonTypeInfo<T> jsonTypeInfo, CancellationToken cancellationToken = default)
            => connection.GetGlobalSettingsAsync(settings => settings.Deserialize(jsonTypeInfo), cancellationToken);

        /// <summary>
        /// Requests the persistent data stored for the action's instance.
        /// </summary>
        /// <typeparam name="T">The type of the settings.</typeparam>
        /// <param name="connection">The connection with the Stream Deck.</param>
        /// <param name="context">The context of the action to get the settings for.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The action instance's settings.</returns>
        public static Task<T> GetSettingsAsync<T>(this IStreamDeckConnection connection, string context, CancellationToken cancellationToken = default)
            => connection.GetSettingsAsync(context, settings => settings.Deserialize<T>(StreamDeckJsonContext.Default.Options), cancellationToken);

        /// <summary>
        /// Requests the persistent data stored for the action's instance.
        /// </summary>
        /// <typeparam name="T">The type of the settings.</typeparam>
        /// <param name="connection">The connection with the Stream Deck.</param>
        /// <param name="context">The context of the action to get the settings for.</param>
        /// <param name="jsonTypeInfo">The JSON type information used to deserialize the global settings.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The action instance's settings.</returns>
        public static Task<T> GetSettingsAsync<T>(this IStreamDeckConnection connection, string context, JsonTypeInfo<T> jsonTypeInfo, CancellationToken cancellationToken = default)
            => connection.GetSettingsAsync(context, settings => settings.Deserialize(jsonTypeInfo), cancellationToken);

        /// <summary>
        /// Runs the plugin and returns a <see cref="Task"/> that only completes when the plugin disconnects from the Stream Deck.
        /// </summary>
        /// <param name="connection">The connection with the Stream Deck.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public static async Task RunAsync(this StreamDeckConnection connection, CancellationToken cancellationToken = default)
        {
            await connection.ConnectAsync(cancellationToken).ConfigureAwait(false);
            await connection.WaitForShutdownAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Requests the persistent global data stored for the plugin.
        /// </summary>
        /// <typeparam name="T">The type of the settings.</typeparam>
        /// <param name="connection">The connection with the Stream Deck.</param>
        /// <param name="getSettings">The delegate responsible for deserializing the settings.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The global settings.</returns>
        private static Task<T> GetGlobalSettingsAsync<T>(this IStreamDeckConnection connection, Func<JsonElement, T> getSettings, CancellationToken cancellationToken)
        {
            var taskSource = new TaskCompletionSource<T>();

            // Declare the local function handler that sets the task result
            void handler(object sender, StreamDeckEventArgs<SettingsPayload> e)
            {
                if (taskSource.TrySetResult(getSettings(e.Payload.Settings)))
                {
                    connection.DidReceiveGlobalSettings -= handler;
                }
            }

            // Register the cancellation.
            cancellationToken.Register(() =>
            {
                if (taskSource.TrySetCanceled())
                {
                    connection.DidReceiveGlobalSettings -= handler;
                }
            });

            // Listen for receiving events, and trigger a request.
            connection.DidReceiveGlobalSettings += handler;
            connection.GetGlobalSettingsAsync(cancellationToken);

            return taskSource.Task;
        }

        /// <summary>
        /// Requests the persistent data stored for the action's instance.
        /// </summary>
        /// <typeparam name="T">The type of the settings.</typeparam>
        /// <param name="connection">The connection with the Stream Deck.</param>
        /// <param name="context">The context of the action to get the settings for.</param>
        /// <param name="getSettings">The delegate responsible for deserializing the settings.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The action instance's settings.</returns>
        private static Task<T> GetSettingsAsync<T>(this IStreamDeckConnection connection, string context, Func<JsonElement, T> getSettings, CancellationToken cancellationToken)
        {
            var taskSource = new TaskCompletionSource<T>();

            // Declare the local function handler that sets the task result.
            void handler(object sender, ActionEventArgs<ActionPayload> e)
            {
                if (e.Context == context
                    && taskSource.TrySetResult(getSettings(e.Payload.Settings)))
                {
                    connection.DidReceiveSettings -= handler;
                }
            }

            // Register the cancellation.
            cancellationToken.Register(() =>
            {
                if (taskSource.TrySetCanceled())
                {
                    connection.DidReceiveSettings -= handler;
                }
            });

            // Listen for receiving events, and trigger a request.
            connection.DidReceiveSettings += handler;
            connection.GetSettingsAsync(context, cancellationToken);

            return taskSource.Task;
        }
    }
}
