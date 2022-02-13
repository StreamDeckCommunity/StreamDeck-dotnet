namespace StreamDeck.Serialization
{
    using StreamDeck.Enums;

    /// <summary>
    /// Provides a JSON converter for <see cref="FontStyle"/>.
    /// </summary>
    public class FontStyleJsonConverter : MapJsonConverter<FontStyle>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FontStyleJsonConverter"/> class.
        /// </summary>
        public FontStyleJsonConverter()
            : base()
        {
            this.Add(FontStyle.Regular)
                .Add(FontStyle.Bold)
                .Add(FontStyle.Italic)
                .Add(FontStyle.BoldItalic, "Bold Italic");
        }
    }
}
