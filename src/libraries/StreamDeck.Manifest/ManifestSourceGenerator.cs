namespace StreamDeck.Manifest
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Microsoft.CodeAnalysis;
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
            // Ensure we have a registered context receiver; this is responsible for discovering actions and states.
            if (context.SyntaxContextReceiver is ManifestBuilder builder)
            {
                try
                {
                    // Ensure we know where the manifest.json file is located.
                    var canWrite = this.TryGetFilePath(context, out var manifestFilePath);
                    if (!canWrite)
                    {
                        context.ReportDiagnostic(Diagnostics.MissingManifestFile);
                    }

                    // Attempt to parse the manifest information from the context.
                    if (builder.TryBuild(context, out var manifest))
                    {
                        if (canWrite)
                        {
                            File.WriteAllText(manifestFilePath, JsonSerializer.Serialize(manifest), Encoding.UTF8);
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
        }

        /// <inheritdoc/>
        public void Initialize(GeneratorInitializationContext context)
            => context.RegisterForSyntaxNotifications(() => new ManifestBuilder());

        /// <summary>
        /// Attempts to get the manifest.json file path from the additional files defined in the <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The generator execution context.</param>
        /// <param name="filePath">The manifest.json file path.</param>
        /// <returns><c>true</c> when the manifest.json file path was present; otherwise <c>false</c>.</returns>
        internal bool TryGetFilePath(GeneratorExecutionContext context, out string filePath)
        {
            filePath = context.AdditionalFiles.FirstOrDefault(a => a.Path.EndsWith("manifest.json", StringComparison.OrdinalIgnoreCase))?.Path;
            return filePath != null;
        }
    }
}
