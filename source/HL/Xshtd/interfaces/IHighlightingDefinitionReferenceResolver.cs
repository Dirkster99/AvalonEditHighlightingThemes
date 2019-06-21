namespace HL.Xshtd.interfaces
{
    using HL.HighlightingTheme;

    public interface IHighlightingThemeDefinitionReferenceResolver
    {
        /// <summary>
        /// Gets the highlighting definition by name, or null if it is not found.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        SyntaxDefinition GetThemeDefinition(string name);

        /// <summary>
        /// Gets the highlighting theme definition by name of the theme and the highlighting,
        /// or null if there is none to be found.
        /// </summary>
        /// <param name="hlThemeName"></param>
        /// <param name="highlightingName"></param>
        SyntaxDefinition GetThemeDefinition(string hlThemeName,
                                            string highlightingName);
    }
}
