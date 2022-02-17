namespace StreamDeck.Manifest.Serialization
{
    using System;

    /// <summary>
    /// Indicates that the decorated symbol should not be serialized.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property)]
    internal class JsonIgnoreAttribute : Attribute
    {
    }
}
