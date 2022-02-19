namespace StreamDeck.Events.Received
{
    using StreamDeck.Serialization;

    /// <summary>
    /// Defines the possible font styles supported by the Elgato Stream Deck.
    /// </summary>
    public class FontStyle : EnumString<FontStyle>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FontStyle"/> class.
        /// </summary>
        /// <param name="style">The style.</param>
        internal FontStyle(string style)
            : base(style)
        {
        }

        /// <summary>
        /// Regular font.
        /// </summary>
        public static readonly FontStyle Regular = new FontStyle(nameof(Regular));

        /// <summary>
        /// Bold font.
        /// </summary>
        public static readonly FontStyle Bold = new FontStyle(nameof(Bold));

        /// <summary>
        /// Italic font.
        /// </summary>
        public static readonly FontStyle Italic = new FontStyle(nameof(Italic));

        /// <summary>
        /// Bold and italic font.
        /// </summary>
        public static readonly FontStyle BoldItalic = new FontStyle("Bold Italic");
    }
}
