namespace HL.Manager
{
    using ICSharpCode.AvalonEdit.Highlighting;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

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
        #endregion fields

        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="paramThemeName"></param>
        /// <param name="paramPrefix"></param>
        /// <param name="paramDisplayName"></param>
        public HLTheme(string paramThemeName, string paramPrefix, string paramDisplayName)
            : this()
        {
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

        /// <summary>
        /// Gets the name of theme (eg. 'Dark', 'Light' or 'True Blue' for display purposes in the UI.
        /// </summary>
        public string DisplayName { get; }
        #endregion properties

        #region methods
        /// <summary>
        /// Gets the highlighting definition by name, or null if it is not found.
        /// </summary>
        public IHighlightingDefinition GetDefinition(string name)
        {
            lock (lockObj)
            {
                IHighlightingDefinition rh;
                if (highlightingsByName.TryGetValue(name, out rh))
                    return rh;
                else
                    return null;
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
        #endregion methods
    }
}
