namespace StreamDeck.Enums
{
    using System.Text.Json.Serialization;
    using StreamDeck.Serialization;

    /// <summary>
    /// Provides an enumeration of alignments
    /// </summary>
    [JsonConverter(typeof(TitleAlignmentJsonConverter))]
    public enum TitleAlignment
    {
        /// <summary>
        /// Top alignment.
        /// </summary>
        Top,

        /// <summary>
        /// Bottom alignment.
        /// </summary>
        Middle,

        /// <summary>
        /// Middle alignment.
        /// </summary>
        Bottom
    }
}
