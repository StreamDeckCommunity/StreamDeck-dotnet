namespace StreamDeck.Manifest.Models
{
    /// <summary>
    /// Provides information about the operating systems supported by the plugin.
    /// </summary>
    internal class OperatingSystem
    {
        /// <summary>
        /// Identifies a "mac" operating system.
        /// </summary>
        internal const string MAC = "mac";

        /// <summary>
        /// Identifies a "windows" operating system.
        /// </summary>
        internal const string WINDOWS = "windows";

        /// <summary>
        /// Gets the platform that defines the operating system.
        /// </summary>
        internal string Platform { get; private set; }

        /// <summary>
        /// Gets the minimum version required.
        /// </summary>
        internal string MinimumVersion { get; private set;}

        /// <summary>
        /// Creates a new <see cref="OperatingSystem"/> for macOS.
        /// </summary>
        /// <param name="minimumVersion">The minimum version.</param>
        /// <returns>The operating system.</returns>
        internal static OperatingSystem Mac(string minimumVersion)
            => new OperatingSystem
            {
                Platform = MAC,
                MinimumVersion = minimumVersion
            };

        /// <summary>
        /// Creates a new <see cref="OperatingSystem"/> for Windows.
        /// </summary>
        /// <param name="minimumVersion">The minimum version.</param>
        /// <returns>The operating system.</returns>
        internal static OperatingSystem Windows(string minimumVersion)
            => new OperatingSystem
            {
                Platform = WINDOWS,
                MinimumVersion = minimumVersion
            };
    }
}
