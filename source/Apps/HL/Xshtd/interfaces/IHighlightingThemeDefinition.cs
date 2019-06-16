namespace HL.Xshtd.interfaces
{
    using HL.HighlightingTheme;
    using System.ComponentModel;

    /// <summary>
    /// A highlighting definition.
    /// </summary>
    [TypeConverter(typeof(HighlightingThemeDefinitionTypeConverter))]
    public interface IHighlightingThemeDefinition
    {
        /// <summary>
        /// Gets the name of the highlighting theme definition.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets a named highlighting color.
        /// </summary>
        /// <returns>The highlighting color, or null if it is not found.</returns>
        ////HighlightingColor GetNamedColor(string name);

        SyntaxDefinition GetNamedSyntaxDefinition(string name);
    }
}
