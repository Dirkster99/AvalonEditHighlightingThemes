namespace HL.Manager
{
    using HL.Resources;
    using HL.Xshtd;
    using ICSharpCode.AvalonEdit.Highlighting;
    using ICSharpCode.AvalonEdit.Highlighting.Xshd;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Media;
    using System.Xml;
    using System.Xml.Schema;

    /// <summary>
    /// Loads .xshd files, version 2.0.
    /// Version 2.0 files are recognized by the namespace.
    /// </summary>
    static class XshtdLoader
    {
        public const string Namespace = "http://icsharpcode.net/sharpdevelop/themesyntaxdefinition/2019";

        static XmlSchemaSet schemaSet;

        static XmlSchemaSet SchemaSet
        {
            get
            {
                if (schemaSet == null)
                {
                    schemaSet = HighlightingLoader.LoadSchemaSet(new XmlTextReader(
                        HLResources.OpenStream("HL.Modes", "ModeV2_htd.xsd")));
                }
                return schemaSet;
            }
        }

        public static XhstdThemeDefinition LoadDefinition(XmlReader reader, bool skipValidation)
        {
            reader = HighlightingLoader.GetValidatingReader(reader, true, skipValidation ? null : SchemaSet);
            reader.Read();
            return ParseDefinition(reader);
        }

        static XhstdThemeDefinition ParseDefinition(XmlReader reader)
        {
            Debug.Assert(reader.LocalName == "ThemeSyntaxDefinition");
            XhstdThemeDefinition def = new XhstdThemeDefinition();

            def.Name = reader.GetAttribute("name");

            Stack<XshtdElement> xmlPath = new Stack<XshtdElement>();

            ParseElements(def.Elements, reader, xmlPath);

            Debug.Assert(reader.NodeType == XmlNodeType.EndElement);
            Debug.Assert(reader.LocalName == "ThemeSyntaxDefinition");

            return def;
        }

        static XshtdSyntaxDefinition ParseSyntaxDefinition(XmlReader reader,
                                                           Stack<XshtdElement> xmlPath)
        {
            Debug.Assert(reader.LocalName == "SyntaxDefinition");
            XshtdSyntaxDefinition def = new XshtdSyntaxDefinition();

            def.Name = reader.GetAttribute("name");
            string extensions = reader.GetAttribute("extensions");

            if (extensions != null)
                def.Extensions.AddRange(extensions.Split(';'));

            xmlPath.Push(def);
            ParseElements(def.Elements, reader, xmlPath);

            Debug.Assert(reader.NodeType == XmlNodeType.EndElement);
            Debug.Assert(reader.LocalName == "SyntaxDefinition");

            return def;
        }

        static void ParseElements(ICollection<XshtdElement> c,
                                  XmlReader reader,
                                  Stack<XshtdElement> xmlPath)
        {
            if (reader.IsEmptyElement)
                return;

            while (reader.Read() && reader.NodeType != XmlNodeType.EndElement)
            {
                Debug.Assert(reader.NodeType == XmlNodeType.Element);
                if (reader.NamespaceURI != Namespace)
                {
                    if (!reader.IsEmptyElement)
                        reader.Skip();
                    continue;
                }

                switch (reader.Name)
                {
                    case "SyntaxDefinition":
                        c.Add(ParseSyntaxDefinition(reader, xmlPath));
                        break;

////                    case "Property":
////                        c.Add(ParseProperty(reader));
////                        break;
                    case "Color":
                        var parent = xmlPath.Peek() as XshtdSyntaxDefinition;
                        if (parent == null)
                            throw new Exception("Syntax Error: Color cannot occurr outside of SyntaxDefinition");

                        c.Add(ParseNamedColor(reader, parent));
                        break;
////                    case "Keywords":
////                        c.Add(ParseKeywords(reader));
////                        break;
////                    case "Span":
////                        c.Add(ParseSpan(reader));
////                        break;
////                    case "Import":
////                        c.Add(ParseImport(reader));
////                        break;
////                    case "Rule":
////                        c.Add(ParseRule(reader));
////                        break;
                    default:
                        throw new NotSupportedException("Unknown element " + reader.Name);
                }
            }
        }

        static Exception Error(XmlReader reader, string message)
        {
            return Error(reader as IXmlLineInfo, message);
        }

        static Exception Error(IXmlLineInfo lineInfo, string message)
        {
            if (lineInfo != null)
                return new HighlightingDefinitionInvalidException(HighlightingLoader.FormatExceptionMessage(message, lineInfo.LineNumber, lineInfo.LinePosition));
            else
                return new HighlightingDefinitionInvalidException(message);
        }

        /// <summary>
        /// Sets the element's position to the XmlReader's position.
        /// </summary>
        static void SetPosition(XshtdElement element, XmlReader reader)
        {
            IXmlLineInfo lineInfo = reader as IXmlLineInfo;
            if (lineInfo != null)
            {
                element.LineNumber = lineInfo.LineNumber;
                element.ColumnNumber = lineInfo.LinePosition;
            }
        }

        static XshdReference<XshdRuleSet> ParseRuleSetReference(XmlReader reader)
        {
            string ruleSet = reader.GetAttribute("ruleSet");
            if (ruleSet != null)
            {
                // '/' is valid in highlighting definition names, so we need the last occurence
                int pos = ruleSet.LastIndexOf('/');
                if (pos >= 0)
                {
                    return new XshdReference<XshdRuleSet>(ruleSet.Substring(0, pos), ruleSet.Substring(pos + 1));
                }
                else
                {
                    return new XshdReference<XshdRuleSet>(null, ruleSet);
                }
            }
            else
            {
                return new XshdReference<XshdRuleSet>();
            }
        }

        static void CheckElementName(XmlReader reader, string name)
        {
            if (name != null)
            {
                if (name.Length == 0)
                    throw Error(reader, "The empty string is not a valid name.");
                if (name.IndexOf('/') >= 0)
                    throw Error(reader, "Element names must not contain a slash.");
            }
        }

        #region ParseColor
        static XshtdColor ParseNamedColor(XmlReader reader, XshtdSyntaxDefinition syntax)
        {
            XshtdColor color = ParseColorAttributes(reader, syntax);
            // check removed: invisible named colors may be useful now that apps can read highlighting data
            //if (color.Foreground == null && color.FontWeight == null && color.FontStyle == null)
            //	throw Error(reader, "A named color must have at least one element.");
            color.Name = reader.GetAttribute("name");
            CheckElementName(reader, color.Name);
            color.ExampleText = reader.GetAttribute("exampleText");
            return color;
        }

        static XshtdColor ParseColorAttributes(XmlReader reader, XshtdSyntaxDefinition syntax)
        {
            XshtdColor color = new XshtdColor(syntax);
            SetPosition(color, reader);

            IXmlLineInfo position = reader as IXmlLineInfo;

            color.Foreground = ParseColor(position, reader.GetAttribute("foreground"));
            color.Background = ParseColor(position, reader.GetAttribute("background"));
            color.FontWeight = ParseFontWeight(reader.GetAttribute("fontWeight"));
            color.FontStyle = ParseFontStyle(reader.GetAttribute("fontStyle"));
            color.Underline = reader.GetBoolAttribute("underline");

            return color;
        }

        internal readonly static ColorConverter ColorConverter = new ColorConverter();
        internal readonly static FontWeightConverter FontWeightConverter = new FontWeightConverter();
        internal readonly static FontStyleConverter FontStyleConverter = new FontStyleConverter();

        static HighlightingBrush ParseColor(IXmlLineInfo lineInfo, string color)
        {
            if (string.IsNullOrEmpty(color))
                return null;
            if (color.StartsWith("SystemColors.", StringComparison.Ordinal))
                return GetSystemColorBrush(lineInfo, color);
            else
                return FixedColorHighlightingBrush((Color?)ColorConverter.ConvertFromInvariantString(color));
        }

        internal static SystemColorHighlightingBrush GetSystemColorBrush(IXmlLineInfo lineInfo, string name)
        {
            Debug.Assert(name.StartsWith("SystemColors.", StringComparison.Ordinal));
            string shortName = name.Substring(13);
            var property = typeof(SystemColors).GetProperty(shortName + "Brush");
            if (property == null)
                throw Error(lineInfo, "Cannot find '" + name + "'.");
            return new SystemColorHighlightingBrush(property);
        }

        static HighlightingBrush FixedColorHighlightingBrush(Color? color)
        {
            if (color == null)
                return null;
            return new SimpleHighlightingBrush(color.Value);
        }

        static FontWeight? ParseFontWeight(string fontWeight)
        {
            if (string.IsNullOrEmpty(fontWeight))
                return null;
            return (FontWeight?)FontWeightConverter.ConvertFromInvariantString(fontWeight);
        }

        static FontStyle? ParseFontStyle(string fontStyle)
        {
            if (string.IsNullOrEmpty(fontStyle))
                return null;
            return (FontStyle?)FontStyleConverter.ConvertFromInvariantString(fontStyle);
        }
        #endregion
    }
}
