namespace StreamDeck.Events.Received
{
    using StreamDeck.Serialization;

    /// <summary>
    /// An enumeration of fonts available to the Elgato Stream Deck.
    /// </summary>
    public class FontFamily : EnumString<FontFamily>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FontFamily"/> class.
        /// </summary>
        /// <param name="name">The name of the font.</param>
        internal FontFamily(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Default font.
        /// </summary>
        public static readonly FontFamily Default = string.Empty;

        /// <summary>
        /// Arial font.
        /// </summary>
        public static readonly FontFamily Arial = nameof(Arial);

        /// <summary>
        /// Arial Black font.
        /// </summary>
        public static readonly FontFamily ArialBlack = "Arial Black";

        /// <summary>
        /// Comic Sans MS font.
        /// </summary>
        public static readonly FontFamily ComicSansMS = "Comic Sans MS";

        /// <summary>
        /// Courier font.
        /// </summary>
        public static readonly FontFamily Courier = nameof(Courier);

        /// <summary>
        /// Courier New font.
        /// </summary>
        public static readonly FontFamily CourierNew = "Courier New";

        /// <summary>
        /// Georgia font.
        /// </summary>
        public static readonly FontFamily Georgia = nameof(Georgia);

        /// <summary>
        /// Impact font.
        /// </summary>
        public static readonly FontFamily Impact = nameof(Impact);

        /// <summary>
        /// Microsoft Sans Serif font.
        /// </summary>
        public static readonly FontFamily MicrosoftSansSerif = "Microsoft Sans Serif";

        /// <summary>
        /// Symbol font.
        /// </summary>
        public static readonly FontFamily Symbol = nameof(Symbol);

        /// <summary>
        /// Tahoma font.
        /// </summary>
        public static readonly FontFamily Tahoma = nameof(Tahoma);

        /// <summary>
        /// Times New Roman font.
        /// </summary>
        public static readonly FontFamily TimesNewRoman = "Times New Roman";

        /// <summary>
        /// Trebuchet MS font.
        /// </summary>
        public static readonly FontFamily TrebuchetMS = "Trebuchet MS";

        /// <summary>
        /// Verdana font.
        /// </summary>
        public static readonly FontFamily Verdana = nameof(Verdana);

        /// <summary>
        /// Webdings font.
        /// </summary>
        public static readonly FontFamily Webdings = nameof(Webdings);

        /// <summary>
        /// Wingdings font.
        /// </summary>
        public static readonly FontFamily Wingdings = nameof(Wingdings);

        /// <summary>
        /// Performs an implicit conversion from <see cref="string"/> to <see cref="FontFamily"/>.
        /// </summary>
        /// <param name="fontFamily">The font family.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator FontFamily(string fontFamily)
            => new FontFamily(fontFamily);
    }
}
