namespace StreamDeck.Enums
{
    using System.Text.Json.Serialization;
    using StreamDeck.Serialization;

    /// <summary>
    /// An enumeration of fonts available to the Elgato Stream Deck.
    /// </summary>
    [JsonConverter(typeof(FontFamilyJsonConverter))]
    public enum FontFamily
    {
        /// <summary>
        /// Arial font.
        /// </summary>
        Arial,

        /// <summary>
        /// Arial Black font.
        /// </summary>
        ArialBlack,

        /// <summary>
        /// Comic Sans MS font.
        /// </summary>
        ComicSansMS,

        /// <summary>
        /// Courier font.
        /// </summary>
        Courier,

        /// <summary>
        /// Courier New font.
        /// </summary>
        CourierNew,

        /// <summary>
        /// Georgia font.
        /// </summary>
        Georgia,

        /// <summary>
        /// Impact font.
        /// </summary>
        Impact,

        /// <summary>
        /// Microsoft Sans Serif font.
        /// </summary>
        MicrosoftSansSerif,

        /// <summary>
        /// Symbol font.
        /// </summary>
        Symbol,

        /// <summary>
        /// Tahoma font.
        /// </summary>
        Tahoma,

        /// <summary>
        /// Times New Roman font.
        /// </summary>
        TimesNewRoman,

        /// <summary>
        /// Trebuchet MS font.
        /// </summary>
        TrebuchetMS,

        /// <summary>
        /// Verdana font.
        /// </summary>
        Verdana,

        /// <summary>
        /// Webdings font.
        /// </summary>
        Webdings,

        /// <summary>
        /// Wingdings font.
        /// </summary>
        Wingdings,
    }
}
