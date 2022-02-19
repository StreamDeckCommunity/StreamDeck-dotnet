namespace StreamDeck.Events.Received
{
    using System;
    using System.Globalization;
    using System.Text.Json.Serialization;
    using StreamDeck.Serialization.Converters;

    /// <summary>
    /// Provides information about an application.
    /// </summary>
    public class Application
    {
        /// <summary>
        /// Gets or sets the language in which the Stream Deck application is running. Possible values are en, fr, de, es, ja, zh_CN.
        /// </summary>
        [JsonConverter(typeof(CultureInfoJsonConverter))]
        public CultureInfo Language { get; set; }

        /// <summary>
        /// Gets or sets which platform the Stream Deck application is running
        /// </summary>
        [JsonConverter(typeof(PlatformJsonConverter))]
        public Platform Platform { get; set; }

        /// <summary>
        /// Gets or sets the Stream Deck application version.
        /// </summary>
        public Version Version { get; set; }
    }
}
