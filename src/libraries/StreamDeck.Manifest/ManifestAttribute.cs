namespace StreamDeck.Manifest
{
    using System;
    using StreamDeck.Manifest.Models;

    /// <summary>
    /// Provides information about the manifest of the plugin.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class ManifestAttribute : ManifestInfo
    {
        /// <summary>
        /// Gets or sets the list of application identifiers to monitor (applications launched or terminated) on Mac. See the applicationDidLaunch and applicationDidTerminate events.
        /// </summary>
        public string[] ApplicationsToMonitorMac { get; set; } = new string[0];

        /// <summary>
        /// Gets or sets the list of application identifiers to monitor (applications launched or terminated) on Windows. See the applicationDidLaunch and applicationDidTerminate events.
        /// </summary>
        public string[] ApplicationsToMonitorWin { get; set; } = new string[0];

        /// <summary>
        /// Gets or sets minimum version of Mac supported by the plugin
        /// </summary>
        public string OSMacMinimumVersion { get; set; }

        /// <summary>
        /// Gets or sets minimum version of Windows supported by the plugin
        /// </summary>
        public string OSWindowsMinimumVersion { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates which version of the Stream Deck application is required to install the plugin.
        /// </summary>
        public string SoftwareMinimumVersion { get; set; } = "5.0";
    }
}
