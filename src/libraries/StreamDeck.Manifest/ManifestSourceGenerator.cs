namespace StreamDeck.Manifest
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Microsoft.CodeAnalysis;
    using StreamDeck.Manifest.Extensions;
    using StreamDeck.Manifest.Serialization;
    using System.Collections.Generic;

    /// <summary>
    /// Provides auto-generation of the manifest.json file that accompanies a Stream Deck plugin.
    /// </summary>
    [Generator]
    public class ManifestSourceGenerator : ISourceGenerator
    {
        /// <inheritdoc/>
        public void Execute(GeneratorExecutionContext context)
        {
            // Should only generate the manifest file if we have a receiver.
            if (!(context.SyntaxContextReceiver is PluginActionSyntaxReceiver syntaxReceiver)
                || context.CancellationToken.IsCancellationRequested)
            {
                return;
            }

            try
            {
                // Ensure we know where the manifest.json file is located.
                if (!this.TryGetFilePath(context, out var filePath))
                {
                    context.ReportMissingManifestFile();
                    return;
                }

                // Ensure we have the manifest attribute associated with the assembly.
                if (!context.Compilation.Assembly.TryGetAttribute<PluginManifestAttribute>(out var manifestAttr))
                {
                    context.ReportMissingManifestAttribute();
                    return;
                }

                // Construct the manifest, and the actions associated with it
                var manifest = new PluginManifestAttribute(context.Compilation.Assembly);
                manifestAttr.Populate(manifest);
                manifest.Actions.AddRange(this.GetActions(context, syntaxReceiver));

                // Write the manifest file.
                var json = JsonSerializer.Serialize(manifest);
                File.WriteAllText(filePath, json, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                context.ReportException(ex);
            }
        }

        /// <inheritdoc/>
        public void Initialize(GeneratorInitializationContext context)
            => context.RegisterForSyntaxNotifications(() => new PluginActionSyntaxReceiver());

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

        /// <summary>
        /// Gets the actions associated with the generator.
        /// </summary>
        /// <param name="context">The generator execution context.</param>
        /// <param name="syntaxReceiver">The syntax receiver that contains the actions.</param>
        /// <returns>The actions discovered whilst traversing the compilation nodes.</returns>
        private IEnumerable<PluginActionAttribute> GetActions(GeneratorExecutionContext context, PluginActionSyntaxReceiver syntaxReceiver)
        {
            foreach (var actionDeclaration in syntaxReceiver.Actions)
            {
                var action = actionDeclaration.AttributeData.CreateInstance<PluginActionAttribute>();
                var states = actionDeclaration.Symbol.GetAttributes<PluginActionStateAttribute>().ToArray();

                if (states.Length > 0)
                {
                    // When there is a state image defined, and custom states, warn of duplication.
                    if (action.States.Count > 0)
                    {
                        context.ReportStateImageValueObsolete(actionDeclaration.Symbol.Locations.First());
                    }

                    action.States.Clear();
                    action.States.AddRange(states.Select(s => s.CreateInstance<PluginActionStateAttribute>()).Take(2));
                }

                // Ensure we have at least 1 action state.
                if (action.States.Count == 0)
                {
                    context.ReportNoActionStatesDefined(actionDeclaration.Symbol.Locations.First());
                }

                // Ensure we dont have more than 2 action states.
                if (action.States.Count > 2)
                {
                    context.ReportTooManyActionStates(actionDeclaration.Symbol.Locations.First());
                }

                yield return action;
            }
        }
    }
}
