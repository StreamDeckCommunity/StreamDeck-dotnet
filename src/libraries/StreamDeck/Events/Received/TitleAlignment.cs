namespace StreamDeck.Events.Received
{
    using StreamDeck.Serialization;

    /// <summary>
    /// Defines the possible title alignments supported by the Elgato Stream Deck.
    /// </summary>
    public class TitleAlignment : EnumString<TitleAlignment>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TitleAlignment"/> class.
        /// </summary>
        /// <param name="alignment">The alignment.</param>
        internal TitleAlignment(string alignment)
            : base(alignment)
        {
        }

        /// <summary>
        /// Top alignment.
        /// </summary>
        public static readonly TitleAlignment Top = new TitleAlignment(nameof(Top).ToLowerInvariant());

        /// <summary>
        /// Middle alignment.
        /// </summary>
        public static readonly TitleAlignment Middle = new TitleAlignment(nameof(Middle).ToLowerInvariant());

        /// <summary>
        /// Bottom alignment.
        /// </summary>
        public static readonly TitleAlignment Bottom = new TitleAlignment(nameof(Bottom).ToLowerInvariant());
    }
}
