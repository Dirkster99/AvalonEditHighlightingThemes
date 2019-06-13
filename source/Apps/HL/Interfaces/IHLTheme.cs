namespace HL.Manager
{
    using System.Collections.ObjectModel;
    using ICSharpCode.AvalonEdit.Highlighting;

    /// <summary>
    /// Defines a highlighting theme which is based on a WPF theme (eg. 'Light')
    /// with a corresponding set of highlighting definitions (eg. 'XML', 'C#' etc)
    /// to ensure that highlightings are correct in the contecxt of
    /// (different background colors) WPF themes.
    /// </summary>
    public interface IHLTheme
    {
        #region properties
        /// <summary>
        /// Gets a copy of all highlightings.
        /// </summary>
        ReadOnlyCollection<IHighlightingDefinition> HighlightingDefinitions { get; }

        /// <summary>
        /// Gets the prefix of the XSHD resources that should be used to lookup
        /// the actual resource for this theme.
        /// </summary>
        string Prefix { get; }

        /// <summary>
        /// Gets the name of theme (eg. 'Dark' or 'Light' which is used as
        /// the base of an actual highlighting definition (eg. 'XML').
        /// </summary>
        string ThemeName { get; }

        /// <summary>
        /// Gets the name of theme (eg. 'Dark', 'Light' or 'True Blue' for display purposes in the UI.
        /// </summary>
        string DisplayName { get; }
        #endregion properties

        #region methods
        /// <summary>
        /// Gets the highlighting definition by name, or null if it is not found.
        /// </summary>
        IHighlightingDefinition GetDefinition(string name);

        /// <summary>
        /// Gets a highlighting definition by extension.
        /// Returns null if the definition is not found.
        /// </summary>
        IHighlightingDefinition GetDefinitionByExtension(string extension);

        /// <summary>
        /// Registers a highlighting definition.
        /// </summary>
        /// <param name="name">The name to register the definition with.</param>
        /// <param name="extensions">The file extensions to register the definition for.</param>
        /// <param name="highlighting">The highlighting definition.</param>
        void RegisterHighlighting(string name, string[] extensions, IHighlightingDefinition highlighting);
        #endregion methods
    }
}