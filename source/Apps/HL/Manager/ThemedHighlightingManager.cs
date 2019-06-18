namespace HL.Manager
{
    using HL.HighlightingTheme;
    using HL.Interfaces;
    using HL.Resources;
    using HL.Xshtd.interfaces;
    using ICSharpCode.AvalonEdit.Highlighting;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Implements a Highlighting Manager that associates syntax highlighting definitions with file extentions
    /// (*.cs -> 'C#') with consideration of the current WPF App theme
    /// 
    /// Extension  App Theme   SyntaxHighlighter
    /// (*.cs  +   'Dark')  -> 'C#' (with color definitions for 'Dark')
    /// </summary>
    public class ThemedHighlightingManager : IThemedHighlightingManager
    {
        #region fields
        /// <summary>
        /// Defines the root namespace under which the built-in xshd highlighting
        /// resource files can be found
        /// (eg all highlighting for 'Light' should be located here).
        /// </summary>
        public const string HL_GENERIC_NAMESPACE_ROOT = "HL.Resources.Light";

        /// <summary>
        /// Defines the root namespace under which the built-in additional xshtd highlighting theme
        /// resource files can be found
        /// (eg 'Dark' and 'TrueBlue' themes should be located here).
        /// </summary>
        public const string HL_THEMES_NAMESPACE_ROOT = "HL.Resources.Themes";

        private readonly object lockObj = new object();
        private readonly Dictionary<string, HLTheme> _ThemedHighlightings;
        #endregion fields

        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        public ThemedHighlightingManager()
        {
            _ThemedHighlightings = new Dictionary<string, HLTheme>();

            var theme = new HLTheme("Dark", "Light", "Dark",
                                    HL_THEMES_NAMESPACE_ROOT, "Dark.xshtd", this);
            _ThemedHighlightings.Add(theme.Key, theme);

            theme = new HLTheme("Light", HL_GENERIC_NAMESPACE_ROOT, "Light");
            _ThemedHighlightings.Add(theme.Key, theme);
            CurrentTheme = theme;

            theme = new HLTheme("TrueBlue", "Light", "True Blue",
                                HL_THEMES_NAMESPACE_ROOT, "TrueBlue.xshtd", this);
            _ThemedHighlightings.Add(theme.Key, theme);
        }
        #endregion ctors

        #region properties
        /// <summary>
        /// Gets the current highlighting theme (eg 'Light' or 'Dark') that should be used as
        /// a base for the syntax highlighting in AvalonEdit.
        /// </summary>
        public IHLTheme CurrentTheme { get; private set; }

        /// <summary>
        /// Gets the default HighlightingManager instance.
        /// The default HighlightingManager comes with built-in highlightings.
        /// </summary>
        public static IThemedHighlightingManager Instance
        {
            get
            {
                return DefaultHighlightingManager.Instance;
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Gets the highlighting definition by name, or null if it is not found.
        /// </summary>
        IHighlightingDefinition IHighlightingDefinitionReferenceResolver.GetDefinition(string name)
        {
            lock (lockObj)
            {
                if (CurrentTheme != null)
                    return CurrentTheme.GetDefinition(name);

                return null;
            }
        }

        /// <summary>
        /// Gets an (ordered by Name) list copy of all highlightings defined in this object
        /// or an empty collection if there is no highlighting definition available.
        /// </summary>
        public ReadOnlyCollection<IHighlightingDefinition> HighlightingDefinitions
        {
            get
            {
                lock (lockObj)
                {
                    if (CurrentTheme != null)
                        return CurrentTheme.HighlightingDefinitions;

                    return new ReadOnlyCollection<IHighlightingDefinition>(new List<IHighlightingDefinition>());
                }
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
                HLTheme theme;
                if (_ThemedHighlightings.TryGetValue(CurrentTheme.Key, out theme) == true)
                {
                    return theme.GetDefinitionByExtension(extension);
                }

                return null;
            }
        }

        /// <summary>
        /// Registers a highlighting definition for the <see cref="CurrentTheme"/>.
        /// </summary>
        /// <param name="name">The name to register the definition with.</param>
        /// <param name="extensions">The file extensions to register the definition for.</param>
        /// <param name="highlighting">The highlighting definition.</param>
        public void RegisterHighlighting(string name, string[] extensions, IHighlightingDefinition highlighting)
        {
            if (highlighting == null)
                throw new ArgumentNullException("highlighting");

            lock (lockObj)
            {
                if (this.CurrentTheme != null)
                {
                    CurrentTheme.RegisterHighlighting(name, extensions, highlighting);
                }
            }
        }

        /// <summary>
        /// Registers a highlighting definition.
        /// </summary>
        /// <param name="name">The name to register the definition with.</param>
        /// <param name="extensions">The file extensions to register the definition for.</param>
        /// <param name="lazyLoadedHighlighting">A function that loads the highlighting definition.</param>
        public void RegisterHighlighting(string name, string[] extensions, Func<IHighlightingDefinition> lazyLoadedHighlighting)
        {
            if (lazyLoadedHighlighting == null)
                throw new ArgumentNullException("lazyLoadedHighlighting");

            RegisterHighlighting(name, extensions, new DelayLoadedHighlightingDefinition(name, lazyLoadedHighlighting));
        }

        /// <summary>
        /// Resets the highlighting theme based on the name of the WPF App Theme
        /// (eg: WPF APP Theme 'TrueBlue' -> Resolve highlighting 'C#' to 'TrueBlue'->'C#')
        /// 
        /// Throws an <see cref="IndexOutOfRangeException"/> if the WPF APP theme is not known.
        /// </summary>
        /// <param name="themeNameKey"></param>
        public void SetCurrentTheme(string themeNameKey)
        {
            CurrentTheme = _ThemedHighlightings[themeNameKey];
            HLResources.RegisterBuiltInHighlightings(DefaultHighlightingManager.Instance,
                                                     CurrentTheme);
        }

        /// <summary>
        /// Helper method to find the correct namespace of an internal xshd resource
        /// based on the name of a (WPF) theme (eg. 'TrueBlue') and an internal
        /// constant (eg. 'HL.Resources')
        /// </summary>
        /// <param name="themeNameKey"></param>
        /// <returns></returns>
        protected virtual string GetPrefix(string themeNameKey)
        {
            HLTheme theme;
            if (_ThemedHighlightings.TryGetValue(themeNameKey, out theme) == true)
            {
                return theme.HLBasePrefix;
            }

            return null;
        }

        /// <summary>
        /// Gets the highlighting theme definition by name, or null if there is none to be found.
        /// </summary>
        /// <param name="hlThemeNamethemeName"></param>
        SyntaxDefinition IHighlightingThemeDefinitionReferenceResolver.GetThemeDefinition(string hlThemeNamethemeName)
        {
            lock (lockObj)
            {
                if (CurrentTheme != null)
                    return CurrentTheme.GetThemeDefinition(hlThemeNamethemeName);

                return null;
            }
        }

        /// <summary>
        /// Gets the highlighting theme definition by name of the theme (eg 'Dark2' or 'TrueBlue')
        /// and the highlighting, or null if there is none to be found.
        /// </summary>
        /// <param name="themeName"></param>
        /// <param name="highlightingName"></param>
        SyntaxDefinition IHighlightingThemeDefinitionReferenceResolver.GetThemeDefinition(string hlThemeName,
                                                                                          string highlightingName)
        {
            lock (lockObj)
            {
                HLTheme highlighting;
                this._ThemedHighlightings.TryGetValue(hlThemeName, out highlighting);

                if (highlighting != null)
                    return highlighting.GetThemeDefinition(hlThemeName);

                return null;
            }
        }
        #endregion methods
    }
}
