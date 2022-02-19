namespace StreamDeck.Events.Received
{
    using StreamDeck.Serialization;

    /// <summary>
    /// Provides an enumeration of platforms.
    /// </summary>
    public class Platform : EnumString<Platform>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Platform"/> class.
        /// </summary>
        /// <param name="platform">The platform.</param>
        internal Platform(string platform)
            : base(platform)
        {
        }

        /// <summary>
        /// The Mac platform (kESDSDKApplicationInfoPlatformMac).
        /// </summary>
        public static readonly TitleAlignment Mac = new TitleAlignment(nameof(Mac).ToLowerInvariant());

        /// <summary>
        /// The Windows platform (kESDSDKApplicationInfoPlatformWindows).
        /// </summary>
        public static readonly TitleAlignment Windows = new TitleAlignment(nameof(Windows).ToLowerInvariant());
    }
}