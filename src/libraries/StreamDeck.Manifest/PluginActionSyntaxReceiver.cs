namespace StreamDeck.Manifest
{
    using System.Collections.ObjectModel;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StreamDeck.Manifest.Extensions;

    /// <summary>
    /// Provides a <see cref="ISyntaxContextReceiver"/> that is capable of discovering classes that represent Stream Deck plugin actions.
    /// </summary>
    internal class PluginActionSyntaxReceiver : ISyntaxContextReceiver
    {
        /// <summary>
        /// Gets the classes that represent Stream Deck plugin actions.
        /// </summary>
        internal Collection<DecoratedSymbolDeclaration> Actions { get; } = new Collection<DecoratedSymbolDeclaration>();

        /// <inheritdoc/>
        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (context.Node is ClassDeclarationSyntax _)
            {
                var symbol = context.SemanticModel.GetDeclaredSymbol(context.Node);
                if (symbol.TryGetAttribute<PluginActionAttribute>(out var attrData))
                {
                    this.Actions.Add(new DecoratedSymbolDeclaration
                    {
                        Symbol = symbol,
                        AttributeData = attrData
                    });
                }
            }
        }

        /// <summary>
        /// Provides information about a symbol that was discovered as the result of its attribute data.
        /// </summary>
        internal struct DecoratedSymbolDeclaration
        {
            /// <summary>
            /// Gets or sets the symbol.
            /// </summary>
            public ISymbol Symbol { get; set; }

            /// <summary>
            /// Gets or sets the attribute data.
            /// </summary>
            public AttributeData AttributeData { get; set; }
        }
    }
}
