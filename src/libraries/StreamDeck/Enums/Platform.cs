namespace StreamDeck.Enums
{
    using System.Text.Json.Serialization;
    using StreamDeck.Serialization;

    /// <summary>
    /// Provides an enumeration of platforms.
    /// </summary>
    [JsonConverter(typeof(PlatformJsonConverter))]
    public enum Platform
    {
        /// <summary>
        /// Defines the Mac platform: kESDSDKApplicationInfoPlatformMac.
        /// </summary>
        Mac,

        /// <summary>
        /// Defines the Windows platform: kESDSDKApplicationInfoPlatformWindows.
        /// </summary>
        Windows
    }
}
