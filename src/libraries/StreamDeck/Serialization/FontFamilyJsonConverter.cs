namespace StreamDeck.Serialization
{
    using StreamDeck.Enums;

    /// <summary>
    /// Provides a JSON converter for <see cref="FontFamily"/>.
    /// </summary>
    public class FontFamilyJsonConverter : MapJsonConverter<FontFamily>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FontFamilyJsonConverter"/> class.
        /// </summary>
        public FontFamilyJsonConverter()
            : base()
        {
            this.Add(FontFamily.Arial)
                .Add(FontFamily.ArialBlack, "Arial Black")
                .Add(FontFamily.ComicSansMS, "Comic Sans MS")
                .Add(FontFamily.Courier)
                .Add(FontFamily.CourierNew, "Courier New")
                .Add(FontFamily.Georgia)
                .Add(FontFamily.Impact)
                .Add(FontFamily.MicrosoftSansSerif, "Microsoft Sans Serif")
                .Add(FontFamily.Symbol)
                .Add(FontFamily.Tahoma)
                .Add(FontFamily.TimesNewRoman, "Times New Roman")
                .Add(FontFamily.TrebuchetMS, "Trebuchet MS")
                .Add(FontFamily.Verdana)
                .Add(FontFamily.Webdings)
                .Add(FontFamily.Wingdings);
        }
    }
}
