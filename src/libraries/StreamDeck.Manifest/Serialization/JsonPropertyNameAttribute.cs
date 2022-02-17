namespace StreamDeck.Manifest.Serialization
{
    using System;

    /// <summary>
    /// Provides information about the serialized value of a properties' name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    internal class JsonPropertyNameAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonPropertyNameAttribute"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public JsonPropertyNameAttribute(string propertyName)
            => this.PropertyName = propertyName;

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        public string PropertyName { get; }
    }
}
