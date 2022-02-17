namespace StreamDeck.Manifest
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using StreamDeck.Manifest.Models;
    using StreamDeck.Manifest.Serialization;

    /// <summary>
    /// Provides auto-generation of the manifest.json file that accompanies a Stream Deck plugin.
    /// </summary>
    [Generator]
    public class ManifestSourceGenerator : ISourceGenerator
    {
        /// <inheritdoc/>
        public void Execute(GeneratorExecutionContext context)
        {
            Debugger.Launch();

            try
            {
                if (!this.TryGetManifestFilePath(context, out var manifestFilePath))
                {
                    context.ReportDiagnostic(Diagnostics.MissingManifestFile);
                }

                if (ManifestBuilder.TryParse(context, out var manifest))
                {
                    if (!string.IsNullOrWhiteSpace(manifestFilePath))
                    {
                        File.WriteAllText(manifestFilePath, JsonSerializer.Serialize(manifest), System.Text.Encoding.UTF8);
                    }
                }
                else
                {
                    context.ReportDiagnostic(Diagnostics.MissingManifestAttribute);
                }
            }
            catch (Exception ex)
            {
                context.ReportDiagnostic(Diagnostics.Exception(ex));
            }
        }

        /// <inheritdoc/>
        public void Initialize(GeneratorInitializationContext context) { /* Do nothing. */ }

        /// <summary>
        /// Attempts to get the manifest.json file path from the additional files defined in the <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The generator execution context.</param>
        /// <param name="filePath">The manifest.json file path.</param>
        /// <returns><c>true</c> when the manifest.json file path was present; otherwise <c>false</c>.</returns>
        private bool TryGetManifestFilePath(GeneratorExecutionContext context, out string filePath)
        {
            filePath = context.AdditionalFiles.FirstOrDefault(a => a.Path.EndsWith("manifest.json", StringComparison.OrdinalIgnoreCase))?.Path;
            return filePath != null;
        }
    }
}
