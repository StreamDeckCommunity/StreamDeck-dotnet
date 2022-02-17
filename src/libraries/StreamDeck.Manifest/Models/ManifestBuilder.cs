namespace StreamDeck.Manifest.Models
{
    using System.Linq;
    using System.Reflection;
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
            : base()
        {
            this.LoadFrom(assembly, manifestAttrData);
        }

        /// <summary>
        /// Gets the manifest.
        /// </summary>
        private ManifestInfo Manifest { get; } = new ManifestInfo();

        /// <summary>
        /// Attempts to parse the <see cref="Manifest"/> from the generation context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="manifest">The manifest.</param>
        /// <returns><c>true</c> when the compilation assembly has an associated <see cref="ManifestAttribute"/>.</returns>
        internal static bool TryParse(GeneratorExecutionContext context, out ManifestInfo manifest)
        {
            if (!context.Compilation.Assembly.TryGetAttribute<ManifestAttribute>(out var manifestAttrData))
            {
                manifest = default;
                return false;
            }

            manifest = new ManifestBuilder(context.Compilation.Assembly, manifestAttrData).Manifest;
            return true;
        }

        /// <summary>
        /// Loads the base data of the <see cref="Manifest"/> from the specified <paramref name="assembly"/>, and supporting <see cref="ManifestAttribute"/>.
        /// </summary>
        /// <param name="assembly">The assembly that is generating the manifest.</param>
        /// <param name="manifestAttrData">The manifest attribute defined within the assembly.</param>
        private void LoadFrom(IAssemblySymbol assembly, AttributeData manifestAttrData)
        {
            // Enable reflection to the model.
            var manifestProperties = typeof(ManifestInfo)
                .GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .ToDictionary(p => p.Name, p => p);

            // Required.
            this.Manifest.Author = assembly.GetAttributeValueOrDefault<AssemblyCompanyAttribute, string>();
            this.Manifest.Description = assembly.GetAttributeValueOrDefault<AssemblyDescriptionAttribute, string>();
            this.Manifest.Name = assembly.Identity.Name;
            this.Manifest.Version = assembly.Identity.Version.ToString(3);
            this.Manifest.Software.MinimumVersion = "5.0";
            this.Manifest.SDKVersion = 2;

            // Required (assumed).
            this.Manifest.CodePath = $"{assembly.Identity.Name}.exe";
            this.Manifest.Icon = $"{assembly.Identity.Name}.png";

            // Named arguments.
            foreach (var argument in manifestAttrData.NamedArguments)
            {
                switch (argument.Key)
                {
                    case nameof(ManifestAttribute.OSMacMinimumVersion):
                        this.Manifest.OS.Add(OperatingSystem.Mac(argument.Value.Value.ToString()));
                        break;

                    case nameof(ManifestAttribute.OSWindowsMinimumVersion):
                        this.Manifest.OS.Add(OperatingSystem.Windows(argument.Value.Value.ToString()));
                        break;

                    case nameof(ManifestAttribute.ApplicationsToMonitorMac):
                        if (this.Manifest.ApplicationsToMonitor == null)
                        {
                            this.Manifest.ApplicationsToMonitor = new ApplicationsToMonitor();
                        }

                        this.Manifest.ApplicationsToMonitor.Mac.AddRange(argument.Value.AsEnumerable<string>());
                        break;

                    case nameof(ManifestAttribute.ApplicationsToMonitorWin):
                        if (this.Manifest.ApplicationsToMonitor == null)
                        {
                            this.Manifest.ApplicationsToMonitor = new ApplicationsToMonitor();
                        }

                        this.Manifest.ApplicationsToMonitor.Windows.AddRange(argument.Value.AsEnumerable<string>());
                        break;

                    case nameof(ManifestAttribute.SoftwareMinimumVersion):
                        this.Manifest.Software.MinimumVersion = argument.Value.ToString();
                        break;

                    default:
                        if (manifestProperties.TryGetValue(argument.Key, out var propInfo))
                        {
                            propInfo.SetValue(this.Manifest, argument.Value.Value);
                        }

                        break;
                }
            }

            // Default to Windows 10 if not operating system was specified.
            if (this.Manifest.OS.Count == 0)
            {
                this.Manifest.OS.Add(OperatingSystem.Windows("10"));
            }
        }
    }
}
