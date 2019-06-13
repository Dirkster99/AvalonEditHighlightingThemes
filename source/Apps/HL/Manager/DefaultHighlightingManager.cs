namespace HL.Manager
{
    using HL.Resources;
    using ICSharpCode.AvalonEdit.Highlighting;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Xml;

    internal sealed class DefaultHighlightingManager : ThemedHighlightingManager
    {
        public new static readonly DefaultHighlightingManager Instance = new DefaultHighlightingManager();

        public DefaultHighlightingManager()
        {
            HLResources.RegisterBuiltInHighlightings(this, HL_NAMESPACE_ROOT, CurrentTheme.ThemeName);
        }

        // Registering a built-in highlighting
        internal void RegisterHighlighting(string name, string[] extensions, string resourceName)
        {
            try
            {
#if DEBUG
                // don't use lazy-loading in debug builds, show errors immediately
                ICSharpCode.AvalonEdit.Highlighting.Xshd.XshdSyntaxDefinition xshd;
                using (Stream s = HLResources.OpenStream(GetPrefix(CurrentTheme.ThemeName), resourceName))
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

                RegisterHighlighting(name, extensions, HighlightingLoader.Load(xshd, this));
#else
					RegisterHighlighting(name, extensions, LoadHighlighting(resourceName));
#endif
            }
            catch (HighlightingDefinitionInvalidException ex)
            {
                throw new InvalidOperationException("The built-in highlighting '" + name + "' is invalid.", ex);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
                                                         Justification = "LoadHighlighting is used only in release builds")]
        Func<IHighlightingDefinition> LoadHighlighting(string resourceName)
        {
            Func<IHighlightingDefinition> func = delegate {
                ICSharpCode.AvalonEdit.Highlighting.Xshd.XshdSyntaxDefinition xshd;
                using (Stream s = HLResources.OpenStream(GetPrefix(CurrentTheme.ThemeName), resourceName))
                {
                    using (XmlTextReader reader = new XmlTextReader(s))
                    {
                        // in release builds, skip validating the built-in highlightings
                        xshd = HighlightingLoader.LoadXshd(reader, true);
                    }
                }
                return ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(xshd, this);
            };
            return func;
        }
    }
}
