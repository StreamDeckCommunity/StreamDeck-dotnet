namespace StreamDeck.Manifest.Models
{
    using StreamDeck.Manifest.Serialization;

    /// <summary>
    /// Provides information about applications to monitor.
    /// </summary>
    internal class ApplicationsToMonitor
    {
        /// <summary>
        /// Gets or sets list of application identifiers to monitor on macOS.
        /// </summary>
        [JsonPropertyName(SupportedOperatingSystem.MAC)]
        internal string[] Mac { get; set; }

        /// <summary>
        /// Gets or sets list of application identifiers to monitor on Windows.
        /// </summary>
        [JsonPropertyName(SupportedOperatingSystem.WINDOWS)]
        internal string[] Windows { get; set; }
    }
}
