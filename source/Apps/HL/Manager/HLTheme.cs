namespace HL.Manager
{
    using HL.HighlightingTheme;
    using HL.Resources;
    using HL.Xshtd;
    using HL.Xshtd.interfaces;
    using ICSharpCode.AvalonEdit.Highlighting;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Xml;

    /// <summary>
    /// Implements a highlighting theme which is based on a WPF theme (eg. 'Light')
    /// with a corresponding set of highlighting definitions (eg. 'XML', 'C#' etc)
    /// to ensure that highlightings are correct in the contecxt of
    /// (different background colors) WPF themes.
    /// </summary>
    internal class HLTheme : IHLTheme
    {
        #region fields
        private readonly object lockObj = new object();
        private Dictionary<string, IHighlightingDefinition> highlightingsByName = new Dictionary<string, IHighlightingDefinition>();
        private Dictionary<string, IHighlightingDefinition> highlightingsByExtension = new Dictionary<string, IHighlightingDefinition>(StringComparer.OrdinalIgnoreCase);
        private List<IHighlightingDefinition> allHighlightings = new List<IHighlightingDefinition>();
        private bool _HLThemeIsInitialized;

        private XhstdThemeDefinition _xshtd;
        private XmlHighlightingThemeDefinition _hlTheme;
        private readonly IHighlightingThemeDefinitionReferenceResolver _hLThemeResolver;
        #endregion fields

        #region ctors
        public HLTheme(string paramKey,
                       string paramPrefix, string paramThemeName, string paramDisplayName,
                       string paramHLPrefix, string paramHLThemeName,
                       IHighlightingThemeDefinitionReferenceResolver themeResolver)
            : this()
        {
            Key = paramKey;
            Prefix = paramPrefix;
            ThemeName = paramThemeName;

            HLPrefix = paramHLPrefix;
            HLThemeName = paramHLThemeName;
            _hLThemeResolver = themeResolver;

            DisplayName = paramDisplayName;
        }


        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="paramThemeName"></param>
        /// <param name="paramPrefix"></param>
        /// <param name="paramDisplayName"></param>
        public HLTheme(string paramKey,
                       string paramPrefix,
                       string paramThemeName,
                       string paramDisplayName)
            : this()
        {
            Key = paramKey;
            ThemeName = paramThemeName;
            Prefix = paramPrefix;
            DisplayName = paramDisplayName;
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        protected HLTheme()
        {
        }
        #endregion ctors

        #region properties
        /// <summary>
        /// Gets the display independent key value that is unique in an
        /// overall collection of highlighting themes and should be used for retrieval purposes.
        /// </summary>
        public string Key { get; }


        /// <summary>
        /// Gets the prefix of the XSHD resources that should be used to lookup
        /// the actual resource for this theme.
        /// </summary>
        public string Prefix { get; }

        /// <summary>
        /// Gets the name of theme (eg. 'Dark' or 'Light' which is used as
        /// the base of an actual highlighting definition (eg. 'XML').
        /// </summary>
        public string ThemeName { get; }

        public string HLPrefix { get; }

        public string HLThemeName { get; }

        /// <summary>
        /// Gets the name of theme (eg. 'Dark', 'Light' or 'True Blue' for display purposes in the UI.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Gets a copy of all highlightings.
        /// </summary>
        public ReadOnlyCollection<IHighlightingDefinition> HighlightingDefinitions
        {
            get
            {
                lock (lockObj)
                {
                    return Array.AsReadOnly(allHighlightings.ToArray());
                }
            }
        }

        public IHighlightingThemeDefinition HlTheme
        {
            get
            {
                ResolveHighLightingTheme();

                return _hlTheme;
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Gets the highlighting definition by name, or null if it is not found.
        /// </summary>
        public IHighlightingDefinition GetDefinition(string name)
        {
            lock (lockObj)
            {
                this.ResolveHighLightingTheme();

                IHighlightingDefinition rh;
                if (highlightingsByName.TryGetValue(name, out rh))
                    return rh;
                else
                    return null;
            }
        }

        /// <summary>
        /// Gets the highlighting theme definition  by name, or null if it is not found.
        /// </summary>
        public SyntaxDefinition GetThemeDefinition(string name)
        {
            lock (lockObj)
            {
                this.ResolveHighLightingTheme();

                return _hlTheme.GetNamedSyntaxDefinition(name);
            }
        }

        /// <summary>
        /// Gets a highlighting definition by extension.
        /// Returns null if the definition is not found.
        /// </summary>
        public IHighlightingDefinition GetDefinitionByExtension(string extension)
        {
            lock (lockObj)
            {
                this.ResolveHighLightingTheme();

                IHighlightingDefinition rh;
                if (highlightingsByExtension.TryGetValue(extension, out rh))
                    return rh;
                else
                    return null;
            }
        }

        /// <summary>
        /// Registers a highlighting definition.
        /// </summary>
        /// <param name="name">The name to register the definition with.</param>
        /// <param name="extensions">The file extensions to register the definition for.</param>
        /// <param name="highlighting">The highlighting definition.</param>
        public void RegisterHighlighting(string name,
                                         string[] extensions,
                                         IHighlightingDefinition highlighting)
        {
            lock (lockObj)
            {
                allHighlightings.Add(highlighting);
                if (name != null)
                {
                    highlightingsByName[name] = highlighting;
                }

                if (extensions != null)
                {
                    foreach (string ext in extensions)
                    {
                        highlightingsByExtension[ext] = highlighting;
                    }
                }
            }
        }

        /// <summary>
        /// Loads the highlighting theme for this highlighting definition
        /// (if was an additional theme was configured)
        /// </summary>
        protected virtual void ResolveHighLightingTheme()
        {
            if (_hlTheme != null || _HLThemeIsInitialized == true)
                return;

            _HLThemeIsInitialized = true;            // Initialize this at most once

            // Load the highlighting theme and setup converter to XmlHighlightingThemeDefinition
            _xshtd = ResolveHighLightingTheme(HLPrefix, HLThemeName);

            if (_hLThemeResolver == null || _xshtd == null)
                return;

            _hlTheme = new XmlHighlightingThemeDefinition(_xshtd, _hLThemeResolver);
        }

        /// <summary>
        /// Converts a XSHD reference from namespace prefix and themename
        /// into a <see cref="XhstdThemeDefinition"/> object and returns it.
        /// </summary>
        /// <param name="hLPrefix"></param>
        /// <param name="hLThemeName"></param>
        /// <returns></returns>
        public XhstdThemeDefinition ResolveHighLightingTheme(string hLPrefix, string hLThemeName)
        {
            if (string.IsNullOrEmpty(hLPrefix) || string.IsNullOrEmpty(hLThemeName))
                return null;

            using (Stream s = HLResources.OpenStream(hLPrefix, hLThemeName))
            {
                using (XmlTextReader reader = new XmlTextReader(s))
                {
                    return HighlightingThemeLoader.LoadXshd(reader, false);
                }
            }
        }
        #endregion methods
    }
}
