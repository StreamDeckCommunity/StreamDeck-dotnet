namespace StreamDeck.Manifest
{
    using Microsoft.CodeAnalysis;
    using StreamDeck.Manifest.Extensions;

    /// <summary>
    /// Provides a builder for <see cref="Manifest"/>.
    /// </summary>
    internal class ManifestBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManifestBuilder"/> class.
        /// </summary>
        /// <param name="assembly">The assembly that is generating the manifest.</param>
        /// <param name="manifestAttrData">The manifest attribute defined within the assembly.</param>
        private ManifestBuilder(IAssemblySymbol assembly, AttributeData manifestAttrData)
        {
            this.Manifest = new ManifestAttribute(assembly);
            manifestAttrData.Populate(this.Manifest);
        }

        /// <summary>
        /// Gets the manifest.
        /// </summary>
        private ManifestAttribute Manifest { get; } = new ManifestAttribute();

        /// <summary>
        /// Attempts to parse the <see cref="Manifest"/> from the generation context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="manifest">The manifest.</param>
        /// <returns><c>true</c> when the compilation assembly has an associated <see cref="Manifest"/>.</returns>
        internal static bool TryParse(GeneratorExecutionContext context, out ManifestAttribute manifest)
        {
            if (!context.Compilation.Assembly.TryGetAttribute<ManifestAttribute>(out var manifestAttrData))
            {
                manifest = default;
                return false;
            }

            manifest = new ManifestBuilder(context.Compilation.Assembly, manifestAttrData).Manifest;
            return true;
        }
    }
}
