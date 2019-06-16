namespace HL.Xshtd
{
    using HL.HighlightingTheme;
    using HL.Xshtd.interfaces;
    using ICSharpCode.AvalonEdit.Highlighting;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.Serialization;

    internal class XmlHighlightingThemeDefinition : IHighlightingThemeDefinition
    {
        #region fields
        private Dictionary<string, SyntaxDefinition> syntaxDefDict;
        private readonly XhstdThemeDefinition _xshtd;

        [OptionalField]
        private Dictionary<string, string> propDict = new Dictionary<string, string>();
        #endregion fields

        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        public XmlHighlightingThemeDefinition(XhstdThemeDefinition xshtd,
                                              IHighlightingThemeDefinitionReferenceResolver resolver)
            : this()
        {
            this.Name = xshtd.Name;

            // Create HighlightingRuleSet instances
            xshtd.AcceptElements(new RegisterNamedElementsVisitor(this));

            // Translate elements within the rulesets (resolving references and processing imports)
            xshtd.AcceptElements(new TranslateElementVisitor(this, resolver));

            _xshtd = xshtd;

            foreach (var p in xshtd.Elements.OfType<XshtdProperty>())
                propDict.Add(p.Name, p.Value);
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        protected XmlHighlightingThemeDefinition()
        {
            syntaxDefDict = new Dictionary<string, SyntaxDefinition>();
        }
        #endregion ctors

        #region properties
        public string Name { get; private set; }

        public IDictionary<string, string> Properties
        {
            get
            {
                return propDict;
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Gets the syntaxdefinition colors that should be applied for a certain highlighting (eg 'C#')
        /// within this theme (eg TrueBlue).
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public SyntaxDefinition GetNamedSyntaxDefinition(string name)
        {
            SyntaxDefinition item = null;
            syntaxDefDict.TryGetValue(name, out item);

            return item;
        }

        public HighlightingColor GetNamedColor(string synDefName, string colorName)
        {
            var synDef = GetNamedSyntaxDefinition(synDefName);
            if (synDef == null)
                return null;

            return synDef.ColorGet(colorName);
        }

        /// <summary>
        /// Gets an enumeration of all highlighting colors that are defined
        /// for this highlighting pattern (eg. C#) as part of a highlighting theme (eg 'True Blue').
        /// </summary>
        public IEnumerable<HighlightingColor> NamedHighlightingColors(string synDefName)
        {
            var synDef = GetNamedSyntaxDefinition(synDefName);
            if (synDef == null)
                return new List<HighlightingColor>();

            return synDef.NamedHighlightingColors;
        }

        /// <summary>
        /// Helper method to generate a <see cref="HighlightingDefinitionInvalidException"/>
        /// containing more insights (line number, coloumn) to verify the actual problem.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private static System.Exception Error(XshtdElement element, string message)
        {
            if (element.LineNumber > 0)
                return new HighlightingDefinitionInvalidException(
                    "Error at line " + element.LineNumber + ":\n" + message);
            else
                return new HighlightingDefinitionInvalidException(message);
        }
        #endregion methods

        #region RegisterNamedElements
        sealed class RegisterNamedElementsVisitor : IXshtdVisitor
        {
            #region fields
            private readonly XmlHighlightingThemeDefinition def;
            #endregion fields

            #region ctors
            /// <summary>
            /// Class constructor
            /// </summary>
            public RegisterNamedElementsVisitor(XmlHighlightingThemeDefinition def)
                : this()
            {
                this.def = def;
            }

            /// <summary>
            /// Class constructor
            /// </summary>
            private RegisterNamedElementsVisitor()
            {
            }
            #endregion ctors

            #region properties

            #endregion properties

            #region methods
            /// <summary>
            /// Method checks if given color name is unique and adds the color into the internal
            /// collection of inique colors if it is.
            /// </summary>
            /// <param name="color"></param>
            /// <returns>Always returns null. Throws a <see cref="HighlightingDefinitionInvalidException"/>
            /// if color name is a duplicate.</returns>
            public object VisitColor(XshtdSyntaxDefinition syntax, XshtdColor color)
            {
                if (color.Name != null)
                {
                    if (color.Name.Length == 0)
                        throw Error(color, "Name must not be the empty string");

                    if (syntax == null)
                        throw Error(syntax, "Syntax Definition for theme must not be null");

                    SyntaxDefinition synDef;
                    if (def.syntaxDefDict.TryGetValue(syntax.Name, out synDef) == false)
                        throw Error(syntax, "Themed Syntax Definition does not exist '" + syntax.Name + "'.");

                    if (synDef.ColorGet(color.Name) == null)
                        synDef.ColorAdd(new HighlightingColor() { Name = color.Name });
                    else
                        throw Error(color, "Duplicate color name '" + color.Name + "'.");
                }

                return null;
            }

            public object VisitSyntaxDefinition(XshtdSyntaxDefinition syntax)
            {
                if (syntax.Name != null)
                {
                    if (syntax.Name.Length == 0)
                        throw Error(syntax, "Name must not be the empty string");

                    if (def.syntaxDefDict.ContainsKey(syntax.Name))
                        throw Error(syntax, "Duplicate syntax definition name '" + syntax.Name + "'.");

                    def.syntaxDefDict.Add(syntax.Name, new SyntaxDefinition());
                }

                syntax.AcceptElements(this);

                return null;
            }
            #endregion methods
        }
        #endregion RegisterNamedElements

        #region TranslateElements
        sealed class TranslateElementVisitor : IXshtdVisitor
        {
            #region fields
            readonly XmlHighlightingThemeDefinition def;
            #endregion fields

            #region ctors
            public TranslateElementVisitor(XmlHighlightingThemeDefinition def,
                                           IHighlightingThemeDefinitionReferenceResolver resolver)
            {
                Debug.Assert(def != null);
                Debug.Assert(resolver != null);
                this.def = def;
            }
            #endregion ctors

            public object VisitColor(XshtdSyntaxDefinition syntax, XshtdColor color)
            {
                if (color.Name == null)
                    throw Error(color, "Name must not be null");

                if (color.Name.Length == 0)
                    throw Error(color, "Name must not be the empty string");

                if (syntax == null)
                    throw Error(syntax, "Syntax Definition for theme must not be null");

                SyntaxDefinition synDef;
                HighlightingColor highColor;
                if (def.syntaxDefDict.TryGetValue(syntax.Name, out synDef) == false)
                    throw Error(syntax, "Themed Syntax Definition does not exist '" + syntax.Name + "'.");

                highColor = synDef.ColorGet(color.Name);
                if (highColor == null)
                {
                    highColor = new HighlightingColor() { Name = color.Name };
                    synDef.ColorAdd(highColor);
                }

                highColor.Foreground = color.Foreground;
                highColor.Background = color.Background;
                highColor.Underline = color.Underline;
                highColor.FontStyle = color.FontStyle;
                highColor.FontWeight = color.FontWeight;

                return highColor;
            }

            public object VisitSyntaxDefinition(XshtdSyntaxDefinition syntax)
            {
                SyntaxDefinition c;
                if (syntax.Name != null)
                    c = def.syntaxDefDict[syntax.Name];
                else
                {
                    if (syntax.Extensions == null)
                        return null;
                    else
                        c = new SyntaxDefinition(syntax.Name);
                }

                // Copy extensions to highlighting theme object
                foreach (var item in syntax.Extensions)
                    c.Extensions.Add(item);

                syntax.AcceptElements(this);

                return c;
            }
        }
        #endregion TranslateElements
    }
}
