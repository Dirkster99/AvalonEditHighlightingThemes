namespace HL.Manager
{
    using HL.Interfaces;
    using HL.Resources;
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
        /// Defines the root namespace under which the built-in xsdh resource files can be found
        /// </summary>
        public const string HL_NAMESPACE_ROOT = "HL.Resources";

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

            var theme = new HLTheme("Dark", HL_NAMESPACE_ROOT, "Dark");
            _ThemedHighlightings.Add(theme.ThemeName, theme);

            theme = new HLTheme("Light", HL_NAMESPACE_ROOT, "Light");
            _ThemedHighlightings.Add(theme.ThemeName, theme);

            CurrentTheme = theme;

            theme = new HLTheme("TrueBlue", HL_NAMESPACE_ROOT, "True Blue");
            _ThemedHighlightings.Add(theme.ThemeName, theme);
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
        /// Gets a copy of all highlightings.
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
                if (_ThemedHighlightings.TryGetValue(CurrentTheme.ThemeName, out theme) == true)
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
        /// (eg: WPF APP Theme 'Dark' -> Resolve highlighting 'C#' to 'Dark'->'C#')
        /// 
        /// Throws an <see cref="IndexOutOfRangeException"/> if the WPF APP theme is not known.
        /// </summary>
        /// <param name="name"></param>
        public void SetCurrentTheme(string name)
        {
            CurrentTheme = _ThemedHighlightings[name];
            HLResources.RegisterBuiltInHighlightings(DefaultHighlightingManager.Instance,
                                                     HL_NAMESPACE_ROOT, CurrentTheme.ThemeName);
        }

        /// <summary>
        /// Helper method to find the correct namespace of an internal xshd resource
        /// based on the name of a (WPF) theme (eg. 'Dark' or 'Light') and an internal
        /// constant (eg. 'HL.Resources')
        /// </summary>
        /// <param name="themeName"></param>
        /// <returns></returns>
        protected virtual string GetPrefix(string themeName)
        {
            HLTheme theme;
            if (_ThemedHighlightings.TryGetValue(themeName, out theme) == true)
            {
                return string.Format("{0}.{1}", theme.Prefix, themeName);
            }

            return null;
        }
        #endregion methods
    }
}
