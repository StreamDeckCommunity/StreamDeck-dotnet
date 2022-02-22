namespace StreamDeck.Events.Received
{
    using System.Text.Json.Serialization;
    using StreamDeck.Serialization.Converters;

    /// <summary>
    /// Provides information about a title.
    /// </summary>
    public class TitleParameters
    {
        /// <summary>
        /// Gets or sets the font family for the title.
        /// </summary>
        [JsonConverter(typeof(FontFamilyJsonConverter))]
        public FontFamily? FontFamily { get; set; }

        /// <summary>
        /// Gets or sets the font size for the title.
        /// </summary>
        public uint FontSize { get; set; }

        /// <summary>
        /// Gets or sets the font style for the title.
        /// </summary>
        [JsonConverter(typeof(FontStyleJsonConverter))]
        public FontStyle? FontStyle { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the title is underlined.
        /// </summary>
        public bool FontUnderline { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the title is visible.
        /// </summary>
        public bool ShowTitle { get; set; }

        /// <summary>
        /// Gets or sets the vertical alignment of the title. Possible values are "top", "bottom" and "middle".
        /// </summary>
        [JsonConverter(typeof(TitleAlignmentJsonConverter))]
        public TitleAlignment? TitleAlignment { get; set; }

        /// <summary>
        /// Gets or sets the title color, as a hexidecimal, e.g. #ffffff.
        /// </summary>
        public string? TitleColor { get; set; }
    }
}
