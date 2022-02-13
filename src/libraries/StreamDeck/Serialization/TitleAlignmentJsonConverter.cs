namespace StreamDeck.Serialization
{
    using StreamDeck.Enums;

    /// <summary>
    /// Provides a JSON converter for <see cref="TitleAlignment"/>.
    /// </summary>
    public class TitleAlignmentJsonConverter : MapJsonConverter<TitleAlignment>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TitleAlignmentJsonConverter"/> class.
        /// </summary>
        public TitleAlignmentJsonConverter()
            : base()
        {
            this.Add(TitleAlignment.Bottom, "bottom")
                .Add(TitleAlignment.Middle, "middle")
                .Add(TitleAlignment.Top, "top");
        }
    }
}
