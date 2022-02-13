namespace StreamDeck.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Provides a map (bi-directional dictionary) of two values.
    /// </summary>
    /// <typeparam name="T">The source value.</typeparam>
    public class MapJsonConverter<T> : JsonConverter<T>
    {
        /// <summary>
        /// Gets the read map.
        /// </summary>
        private IDictionary<string, T> ReadMap { get; } = new Dictionary<string, T>();

        /// <summary>
        /// Gets the write map.
        /// </summary>
        private IDictionary<T, string> WriteMap { get; } = new Dictionary<T, string>();

        /// <inheritdoc/>
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var jsonValue = reader.GetString();
            return jsonValue != null && this.ReadMap.TryGetValue(jsonValue, out var sourceValue)
                ? sourceValue
                : default;
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(
                this.WriteMap.TryGetValue(value, out var jsonValue)
                    ? jsonValue
                    : value.ToString());
        }

        /// <summary>
        /// Adds the specified value to the map; the JSON value will be stringifed.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>This instance.</returns>
        public MapJsonConverter<T> Add(T value)
            => this.Add(value, value.ToString());

        /// <summary>
        /// Adds the specified items to the map.
        /// </summary>
        /// <param name="sourceValue">The source value.</param>
        /// <param name="jsonValue">The JSON value.</param>
        /// <returns>This instance.</returns>
        public MapJsonConverter<T> Add(T sourceValue, string jsonValue)
        {
            this.ReadMap.Add(jsonValue, sourceValue);
            this.WriteMap.Add(sourceValue, jsonValue);

            return this;
        }
    }
}
