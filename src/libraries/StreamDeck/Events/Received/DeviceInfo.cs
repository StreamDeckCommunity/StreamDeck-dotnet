namespace StreamDeck.Events.Received
{
    /// <summary>
    /// Provides information about a device.
    /// </summary>
    public class DeviceInfo
    {
        /// <summary>
        /// Gets or sets the device name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the number of columns and rows of keys that the device owns.
        /// </summary>
        public Size? Size { get; set; }

        /// <summary>
        /// Gets or sets the type of device.
        /// </summary>
        public Device Type { get; set; }
    }
}
