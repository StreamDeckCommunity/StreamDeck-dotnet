namespace StreamDeck.Manifest
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StreamDeck.Manifest.Extensions;

    /// <summary>
    /// A manifest builder capable of intercepting syntax contexts to discover Stream Deck actions, and then construct a manifest from the compilation assembly.
    /// </summary>
    internal class ManifestBuilder : ISyntaxContextReceiver
    {
        /// <summary>
        /// Gets the actions.
        /// </summary>
        private Collection<ActionClassDeclarationSyntax> Actions { get; } = new Collection<ActionClassDeclarationSyntax>();

        /// <inheritdoc/>
        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (context.Node is ClassDeclarationSyntax _)
            {
                var symbol = context.SemanticModel.GetDeclaredSymbol(context.Node);
                if (symbol.TryGetAttribute<PluginActionAttribute>(out var _))
                {
                    this.Actions.Add(new ActionClassDeclarationSyntax(context.Node.GetLocation(), symbol));
                }
            }
        }

        /// <summary>
        /// Attempts to build the <see cref="Manifest"/> from the generation context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="manifest">The manifest.</param>
        /// <returns><c>true</c> when the compilation assembly has an associated <see cref="Manifest"/>.</returns>
        internal bool TryBuild(GeneratorExecutionContext context, out PluginManifestAttribute manifest)
        {
            if (!context.Compilation.Assembly.TryGetAttribute<PluginManifestAttribute>(out var manifestAttrData))
            {
                manifest = default;
                return false;
            }

            manifest = new PluginManifestAttribute(context.Compilation.Assembly);
            manifestAttrData.Populate(manifest);

            manifest.Actions.AddRange(this.GetActions(context));

            return true;
        }

        /// <summary>
        /// Gets the <see cref="PluginActionAttribute"/> from the discovered <see cref="ActionClassDeclarationSyntax"/>.
        /// </summary>
        /// <returns>The actions.</returns>
        private IEnumerable<PluginActionAttribute> GetActions(GeneratorExecutionContext context)
        {
            foreach (var actionDeclaration in this.Actions)
            {
                actionDeclaration.Symbol.TryGetAttribute<PluginActionAttribute>(out var actionAttrData);
                var action = actionAttrData.CreateInstance<PluginActionAttribute>();

                var states = actionDeclaration.Symbol.GetAttributes<PluginActionStateAttribute>().ToArray();
                if (states.Length > 0)
                {
                    // When there is a state image defined, and custom states, warn of duplication.
                    if (action.States.Count > 0)
                    {
                        context.ReportDiagnostic(Diagnostics.StateImageIsObsoleteWhenStateIsDefined(actionDeclaration.Location));
                    }

                    action.States.Clear();
                    action.States.AddRange(states.Select(s => s.CreateInstance<PluginActionStateAttribute>()).Take(2));
                }

                // Ensure we have at least 1 action state.
                if (states.Length == 0)
                {
                    context.ReportDiagnostic(Diagnostics.NoActionStatesDefined(actionDeclaration.Location));
                }

                // Ensure we dont have more than 2 action states.
                if (states.Length > 2)
                {
                    context.ReportDiagnostic(Diagnostics.TooManyActionStates(actionDeclaration.Location));
                }

                yield return action;
            }
        }

        /// <summary>
        /// Provides information about the declaration of a Stream Deck action.
        /// </summary>
        private struct ActionClassDeclarationSyntax
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ActionClassDeclarationSyntax"/> struct.
            /// </summary>
            /// <param name="location">The location.</param>
            /// <param name="symbol">The symbol that represents the declaration.</param>
            public ActionClassDeclarationSyntax(Location location, ISymbol symbol)
            {
                this.Location = location;
                this.Symbol = symbol;
            }

            /// <summary>
            /// Gets the location.
            /// </summary>
            public Location Location { get; }

            /// <summary>
            /// Gets the symbol that represents the declaration
            /// </summary>
            public ISymbol Symbol { get; }
        }
    }
}
