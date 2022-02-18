namespace StreamDeck.Manifest
{
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
            = Error("ESD0002", $"Unable to generate manifest: could not find assembly with \"{nameof(ManifestAttribute)}\"");

        /// <summary>
        /// A general exception was encountered.
        /// </summary>
        /// <param name="ex">The exception.</param>
        /// <returns>The diagnostic.</returns>
        internal static Diagnostic Exception(System.Exception ex)
            => Error("ESD0013", $"Failed to generate manifest file: {ex.Message}");

        /// <summary>
        /// Creates a new <see cref="Diagnostic"/> with severity of <see cref="DiagnosticSeverity.Error"/>.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="message">The message.</param>
        /// <returns>The diagnostic.</returns>
        private static Diagnostic Error(string id, string message)
            => Diagnostic.Create(id, CATEGORY, message, DiagnosticSeverity.Error, DiagnosticSeverity.Error, true, 0);

        /// <summary>
        /// Creates a new <see cref="Diagnostic"/> with severity of <see cref="DiagnosticSeverity.Warning"/>.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="message">The message.</param>
        /// <returns>The diagnostic.</returns>
        private static Diagnostic Warning(string id, string message)
            => Diagnostic.Create(id, CATEGORY, message, DiagnosticSeverity.Warning, DiagnosticSeverity.Warning, true, 1);
    }
}
