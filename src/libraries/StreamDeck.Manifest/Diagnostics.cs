namespace StreamDeck.Manifest
{
    using System;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Provides a set of <see cref="Diagnostic"/> that can be reported by the generator.
    /// </summary>
    internal static class Diagnostics
    {
        /// <summary>
        /// The category.
        /// </summary>
        private const string CATEGORY = "Stream Deck Manifest";

        /// <summary>
        /// The manifest file is missing.
        /// </summary>
        internal static readonly Diagnostic MissingManifestFile
            = Error("ESD0001", "Unable to generate manifest: \"manifest.json\" must be referenced as an additional file");

        /// <summary>
        /// The manifest attribute is missing.
        /// </summary>
        internal static readonly Diagnostic MissingManifestAttribute
            = Error("ESD0002", $"Unable to generate manifest: could not find assembly with {nameof(PluginManifestAttribute)}");

        /// <summary>
        /// A general exception was encountered.
        /// </summary>
        /// <param name="ex">The exception.</param>
        /// <returns>The diagnostic.</returns>
        internal static Diagnostic Exception(Exception ex)
            => Error("ESD0013", $"Failed to generate manifest file: {ex.Message}");

        /// <summary>
        /// An error reported when an action does not define <see cref="PluginActionAttribute.StateImage"/> or <see cref="PluginActionStateAttribute"/>.
        /// </summary>
        /// <param name="location">The location of the action class declaration.</param>
        /// <returns>The diagnostic.</returns>
        internal static Diagnostic NoActionStatesDefined(Location location)
            => Diagnostic.Create(
                new DiagnosticDescriptor(
                    id: "ESD0003",
                    title: "No action states",
                    messageFormat: $"Unable to generate manifest: actions must have either a {nameof(PluginActionAttribute)}.{nameof(PluginActionAttribute.StateImage)}, or up to two {nameof(PluginActionStateAttribute)}",
                    category: CATEGORY,
                    defaultSeverity: DiagnosticSeverity.Error,
                    isEnabledByDefault: true),
                location);

        /// <summary>
        /// Generates a new diagnostic used to indicate when a <see cref="PluginActionAttribute.StateImage"/> is not required as the states are already defined.
        /// </summary>
        /// <param name="location">The location of the action class delcaration.</param>
        /// <returns>The diagnostic.</returns>
        internal static Diagnostic StateImageIsObsoleteWhenStateIsDefined(Location location)
            => Diagnostic.Create(
                new DiagnosticDescriptor(
                    id: "ESD1001",
                    title: $"Unnecessary \"{nameof(PluginActionAttribute.StateImage)}\"",
                    messageFormat: $"Unnecessary \"{nameof(PluginActionAttribute.StateImage)}\" on action: states are already defined with {nameof(PluginActionStateAttribute)}",
                    category: CATEGORY,
                    defaultSeverity: DiagnosticSeverity.Warning,
                    isEnabledByDefault: true),
                location);

        /// <summary>
        /// Generates a new diagnostic used to indicate when an action has more than two states.
        /// </summary>
        /// <param name="location">The location of the action class delcaration.</param>
        /// <returns>The diagnostic.</returns>
        internal static Diagnostic TooManyActionStates(Location location)
            => Diagnostic.Create(
                new DiagnosticDescriptor(
                    id: "ESD0004",
                    title: "Too many action states",
                    messageFormat: $"Unable to generate manifest: actions can have a maximum of two states",
                    category: CATEGORY,
                    defaultSeverity: DiagnosticSeverity.Error,
                    isEnabledByDefault: true),
                location);

        /// <summary>
        /// Creates a new <see cref="Diagnostic"/> with severity of <see cref="DiagnosticSeverity.Error"/>.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="message">The message.</param>
        /// <returns>The diagnostic.</returns>
        private static Diagnostic Error(string id, string message)
            => Diagnostic.Create(id, CATEGORY, message, DiagnosticSeverity.Error, DiagnosticSeverity.Error, true, 0);
    }
}
