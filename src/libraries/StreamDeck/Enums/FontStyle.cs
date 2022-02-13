namespace StreamDeck.Enums
{
    using System.Text.Json.Serialization;
    using StreamDeck.Serialization;

    /// <summary>
    /// Defines the possible font styles supported by the Elgato Stream Deck.
    /// </summary>
    [JsonConverter(typeof(FontStyleJsonConverter))]
    public enum FontStyle
    {
        /// <summary>
        /// Regular font.
        /// </summary>
        Regular,

        /// <summary>
        /// Bold font.
        /// </summary>
        Bold,

        /// <summary>
        /// Italic font.
        /// </summary>
        Italic,

        /// <summary>
        /// Bold and italic font.
        /// </summary>
        BoldItalic
    }
}
