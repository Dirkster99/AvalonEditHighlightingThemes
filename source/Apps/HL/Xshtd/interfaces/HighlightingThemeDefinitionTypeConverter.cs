namespace HL.Xshtd.interfaces
{
    using HL.Manager;
    using System;
    using System.ComponentModel;
    using System.Globalization;

    /// <summary>
    /// Converts between strings and <see cref="IHighlightingThemeDefinition"/> by treating the string as
    /// the definition name and calling
    /// <c>HighlightingManager.Instance.<see cref="HighlightingManager.GetThemeDefinition">GetDefinition</see>(name)</c>.
    /// </summary>
    public sealed class HighlightingThemeDefinitionTypeConverter : TypeConverter
    {
        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the
        /// type of this converter, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="sourceType">A <see cref="System.Type"/> that represents the type you want
        /// to convert from.</param>
        /// <returns>true if this converter can perform the conversion; otherwise, false.</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;
            else
                return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Converts the given object to the type of this converter, using the specified
        /// context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">The <see cref="CultureInfo"/> to use as the current culture.</param>
        /// <param name="value">The <see cref="System.Object"/> to convert.</param>
        /// <returns>An <see cref="System.Object"/> that represents the converted value.</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string definitionName = value as string;
            if (definitionName != null)
                return ThemedHighlightingManager.Instance.GetThemeDefinition(definitionName);
            else
                return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// Returns whether this converter can convert the object to the specified type,
        /// using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="destinationType">The <see cref="System.Object"/> that represents the type you want to convert to.</param>
        /// <returns>true if this converter can perform the conversion; otherwise, false.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;
            else
                return base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        /// Converts the given value object to the specified type, using the specified context
        /// and culture information.
        /// </summary>
        /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">The <see cref="CultureInfo"/> to use as the current culture.</param>
        /// <param name="value">The <see cref="System.Object"/> to convert.</param>
        /// <param name="destinationType">The <see cref="System.Object"/> that represents the type you want to convert to.</param>
        /// <returns>An <see cref="System.Object"/> that represents the converted value.</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            IHighlightingThemeDefinition definition = value as IHighlightingThemeDefinition;
            if (definition != null && destinationType == typeof(string))
                return definition.Name;
            else
                return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}