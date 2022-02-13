namespace StreamDeck.Serialization
{
    using StreamDeck.Enums;

    /// <summary>
    /// Provides a JSON converter for <see cref="Platform"/>.
    /// </summary>
    public class PlatformJsonConverter : MapJsonConverter<Platform>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlatformJsonConverter"/> class.
        /// </summary>
        public PlatformJsonConverter()
            : base()
        {
            this.Add(Platform.Mac, "mac")
                .Add(Platform.Windows, "windows");
        }
    }
}
