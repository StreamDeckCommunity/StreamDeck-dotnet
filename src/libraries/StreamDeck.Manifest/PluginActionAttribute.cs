namespace StreamDeck.Manifest
{
    using System;
    using System.Collections.Generic;
    using StreamDeck.Manifest.Serialization;

    /// <summary>
    /// Provides information about an action.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class PluginActionAttribute : Attribute
    {
        /// <summary>
        /// Private member field for <see cref="StateImage"/>.
        /// </summary>
        private string _stateIcon;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginActionAttribute"/> class.
        /// </summary>
        /// <param name="name">The name of the action. This string is visible to the user in the actions list.</param>
        /// <param name="uuid">The the unique identifier of the action. It must be a uniform type identifier (UTI) that contains only lowercase alphanumeric characters (a-z, 0-9), hyphen (-), and period (.). The string must be in reverse-DNS format. For example, if your domain is elgato.com and you create a plugin named Hello with the action My Action, you could assign the string com.elgato.hello.myaction as your action's Unique Identifier.</param>
        /// <param name="icon">The relative path to a PNG image without the .png extension. This image is displayed in the actions list. The PNG image should be a 20pt x 20pt image. You should provide @1x and @2x versions of the image. The Stream Deck application takes care of loading the appropriate version of the image. This icon is not required for actions not visible in the actions list (VisibleInActionsList set to false).</param>
        public PluginActionAttribute(string name, string uuid, string icon)
        {
            this.UUID = uuid;
            this.Name = name;
            this.Icon = icon;
        }

        /// <summary>
        /// Gets the relative path to a PNG image without the .png extension. This image is displayed in the actions list. The PNG image should be a 20pt x 20pt image. You should provide @1x and @2x versions of the image. The Stream Deck application takes care of loading the appropriate version of the image. This icon is not required for actions not visible in the actions list (VisibleInActionsList set to false).
        /// </summary>
        public string Icon { get; internal set; }

        /// <summary>
        /// Gets the name of the action. This string is visible to the user in the actions list.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets or sets property inspector path; this can override PropertyInspectorPath member from the plugin if you wish to have a different PropertyInspectorPath based on the action. The relative path to the Property Inspector HTML file if your plugin wants to display some custom settings in the Property Inspector.
        /// </summary>
        public string PropertyInspectorPath { get; set; }

        /// <summary>
        /// Gets or sets the default image for the only state; to define more information about a state, or define multiple states, please use the <see cref="PluginActionStateAttribute"/>.
        /// </summary>
        [JsonIgnore]
        public string StateImage
        {
            get => this._stateIcon;
            set
            {
                this._stateIcon = value;
                this.States = new List<PluginActionStateAttribute> { new PluginActionStateAttribute(value) };
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to prevent the action from being used in a Multi Action. <c>true</c> by default.
        /// </summary>
        public bool SupportedInMultiActions { get; set; } = true;

        /// <summary>
        /// Gets or sets the string to display as a tooltip when the user leaves the mouse over your action in the actions list.
        /// </summary>
        public string Tooltip { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the action. It must be a uniform type identifier (UTI) that contains only lowercase alphanumeric characters (a-z, 0-9), hyphen (-), and period (.). The string must be in reverse-DNS format. For example, if your domain is elgato.com and you create a plugin named Hello with the action My Action, you could assign the string com.elgato.hello.myaction as your action's Unique Identifier.
        /// </summary>
        public string UUID { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to hide the action in the actions list. This can be used for a plugin that only works with a specific profile. <c>true</c> by default.
        /// </summary>
        public bool VisibleInActionsList { get; set; } = true;

        /// <inheritdoc/>
        [JsonIgnore]
        public override object TypeId => base.TypeId;

        /// <summary>
        /// Gets or sets the information about the states of the action.
        /// </summary>
        internal List<PluginActionStateAttribute> States { get; set; } = new List<PluginActionStateAttribute>();
    }
}
