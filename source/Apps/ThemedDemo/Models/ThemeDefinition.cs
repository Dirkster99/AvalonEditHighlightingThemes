namespace ThemedDemo.Models
{
    using MLib.Interfaces;
    using System;
    using System.Collections.Generic;

    public class ThemeDefinition : IThemeInfo
    {
        #region constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="themeName"></param>
        /// <param name="themeSources"></param>
        public ThemeDefinition(string themeName,
                               List<Uri> themeSources,
                               string highlightingThemeName)
            : this()
        {
            DisplayName = themeName;

            if (themeSources != null)
            {
                foreach (var item in themeSources)
                    ThemeSources.Add(new Uri(item.OriginalString, UriKind.Relative));
            }

            HighlightingThemeName = highlightingThemeName;
        }

        /// <summary>
        /// Copy constructor from <see cref="IThemeInfo"/> parameter.
        /// </summary>
        /// <param name="theme"></param>
        public ThemeDefinition(IThemeInfo theme)
            : this()
        {
            this.DisplayName = theme.DisplayName;
            this.ThemeSources = new List<Uri>(theme.ThemeSources);
        }

        /// <summary>
        /// Hidden standard constructor
        /// </summary>
        protected ThemeDefinition()
        {
            DisplayName = string.Empty;
            ThemeSources = new List<Uri>();
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Gets the displayable (localized) name for this theme.
        /// </summary>
        public string DisplayName { get; private set; }

        /// <summary>
        /// Gets the Uri sources for this theme.
        /// </summary>
        public List<Uri> ThemeSources { get; private set; }

        public string HighlightingThemeName { get; private set; }
        #endregion properties

        #region methods
        /// <summary>
        /// Adds additional resource file references into the existing theme definition.
        /// </summary>
        /// <param name="additionalResource"></param>
        public void AddResources(List<Uri> additionalResource)
        {
            foreach (var item in additionalResource)
                ThemeSources.Add(item);
        }
        #endregion methods
    }
}
