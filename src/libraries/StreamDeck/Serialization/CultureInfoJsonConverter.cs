namespace StreamDeck.Serialization
{
    using System;
    using System.Globalization;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Provides a JSON converter for <see cref="CultureInfo"/>.
    /// </summary>
    public class CultureInfoJsonConverter : JsonConverter<CultureInfo>
    {
        /// <inheritdoc/>
        public override CultureInfo Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {
                return CultureInfo.GetCultureInfo(reader.GetString());
            }
            catch
            {
                return CultureInfo.CurrentCulture;
            }
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, CultureInfo value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.Name);
    }
}
