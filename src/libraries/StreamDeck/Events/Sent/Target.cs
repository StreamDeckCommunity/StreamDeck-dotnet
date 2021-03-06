namespace StreamDeck.Events.Sent
{
    /// <summary>
    /// Provides an enumeration of targets.
    /// </summary>
    public enum Target
    {
        /// <summary>
        /// Both <see cref="Target.Hardware"/> and <see cref="Target.Software"/>.
        /// </summary>
        Both = 0,

        /// <summary>
        /// Only hardware.
        /// </summary>
        Hardware = 1,

        /// <summary>
        /// Only software.
        /// </summary>
        Software = 2
    }
}
