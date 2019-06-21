namespace HL.Manager
{
    using HL.HighlightingTheme;
    using HL.Resources;
    using ICSharpCode.AvalonEdit.Highlighting;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Xml;

    /// <summary>
    /// Implements a default highlighting manager for
    /// AvalonEdit based themable syntax highlighting definitions.
    /// </summary>
    internal sealed class DefaultHighlightingManager : ThemedHighlightingManager
    {
        /// <summary>
        /// Gets an instance of a <see cref="DefaultHighlightingManager"/> object.
        /// </summary>
        public new static readonly DefaultHighlightingManager Instance = new DefaultHighlightingManager();

        /// <summary>
        /// Default class constructor
        /// </summary>
        public DefaultHighlightingManager()
            : base()
        {
            HLResources.RegisterBuiltInHighlightings(this, CurrentTheme);
        }

        /// <summary>
        /// Registering a built-in highlighting including highlighting themes (if any).
        /// </summary>
        /// <param name="theme"></param>
        /// <param name="name"></param>
        /// <param name="extensions"></param>
        /// <param name="resourceName"></param>
        internal void RegisterHighlighting(IHLTheme theme,
                                           string name,
                                           string[] extensions,
                                           string resourceName)
        {
            try
            {
#if DEBUG
                // don't use lazy-loading in debug builds, show errors immediately
                ICSharpCode.AvalonEdit.Highlighting.Xshd.XshdSyntaxDefinition xshd;
                using (Stream s = HLResources.OpenStream(GetPrefix(CurrentTheme.HLBaseKey), resourceName))
                {
                    using (XmlTextReader reader = new XmlTextReader(s))
                    {
                        xshd = HighlightingLoader.LoadXshd(reader, false);
                    }
                }
                Debug.Assert(name == xshd.Name);
                if (extensions != null)
                    Debug.Assert(System.Linq.Enumerable.SequenceEqual(extensions, xshd.Extensions));
                else
                    Debug.Assert(xshd.Extensions.Count == 0);

                var hlTheme = theme.HlTheme;
                SyntaxDefinition themedHighlights = null;

                if (hlTheme != null)
                {
                    themedHighlights = hlTheme.GetNamedSyntaxDefinition(name);
                }

                // round-trip xshd:
                //					string resourceFileName = Path.Combine(Path.GetTempPath(), resourceName);
                //					using (XmlTextWriter writer = new XmlTextWriter(resourceFileName, System.Text.Encoding.UTF8)) {
                //						writer.Formatting = Formatting.Indented;
                //						new Xshd.SaveXshdVisitor(writer).WriteDefinition(xshd);
                //					}
                //					using (FileStream fs = File.Create(resourceFileName + ".bin")) {
                //						new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter().Serialize(fs, xshd);
                //					}
                //					using (FileStream fs = File.Create(resourceFileName + ".compiled")) {
                //						new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter().Serialize(fs, Xshd.HighlightingLoader.Load(xshd, this));
                //					}

                RegisterHighlighting(name, extensions,
                                     HighlightingLoader.Load(themedHighlights, xshd, this));
#else
					RegisterHighlighting(name, extensions, LoadHighlighting(theme, name, resourceName));
#endif
            }
            catch (HighlightingDefinitionInvalidException ex)
            {
                throw new InvalidOperationException("The built-in highlighting '" + name + "' is invalid.", ex);
            }
        }

        /// <summary>
        /// Gets a function that is used to load highlighting definition in a delayed/defered way
        /// (usually active only when 'Release' is configured).
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
                                                         Justification = "LoadHighlighting is used only in release builds")]
        Func<IHighlightingDefinition> LoadHighlighting(IHLTheme theme, string name, string resourceName)
        {
            Func<IHighlightingDefinition> func = delegate {
                ICSharpCode.AvalonEdit.Highlighting.Xshd.XshdSyntaxDefinition xshd;
                using (Stream s = HLResources.OpenStream(GetPrefix(CurrentTheme.HLBaseKey), resourceName))
                {
                    using (XmlTextReader reader = new XmlTextReader(s))
                    {
                        // in release builds, skip validating the built-in highlightings
                        xshd = HighlightingLoader.LoadXshd(reader, true);
                    }
                }

                var hlTheme = theme.HlTheme;
                SyntaxDefinition themedHighlights = null;

                if (hlTheme != null)
                {
                    themedHighlights = hlTheme.GetNamedSyntaxDefinition(name);
                }

                return HighlightingLoader.Load(themedHighlights, xshd, this);
            };

            return func;
        }
    }
}
