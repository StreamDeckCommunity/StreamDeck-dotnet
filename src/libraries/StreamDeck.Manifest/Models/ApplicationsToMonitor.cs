namespace StreamDeck.Manifest.Models
{
    using System.Collections.Generic;
    using StreamDeck.Manifest.Serialization;

    /// <summary>
    /// Provides information about applications to monitor.
    /// </summary>
    internal class ApplicationsToMonitor
    {
        /// <summary>
        /// Gets or sets list of application identifiers to monitor on macOS.
        /// </summary>
        [JsonPropertyName(OperatingSystem.MAC)]
        internal List<string> Mac { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets list of application identifiers to monitor on Windows.
        /// </summary>
        [JsonPropertyName(OperatingSystem.WINDOWS)]
        internal List<string> Windows { get; set; } = new List<string>();
    }
}
