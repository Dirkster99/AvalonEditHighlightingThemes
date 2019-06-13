namespace HL.Resources
{
    using System.IO;
    using HL.Manager;

    internal class HLResources
    {
        /// <summary>
        /// Open a <see cref="Stream"/> object to an internal resource (eg: xshd file)
        /// to load its contents from an 'Embedded Resource'.
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Stream OpenStream(string prefix, string name)
        {
            Stream s = typeof(HLResources).Assembly.GetManifestResourceStream(prefix + "." + name);
            if (s == null)
                throw new FileNotFoundException("The resource file '" + name + "' was not found.");

            return s;
        }

        /// <summary>
        /// Registers the built-in highlighting definitions on first time request for a definition
        /// or when the application changes its WPF Theme (eg. from 'Light' to 'Dark') to load the
        /// appropriate highlighting resource when queried for it.
        /// </summary>
        /// <param name="hlm"></param>
        /// <param name="namespaceBase"></param>
        /// <param name="currentTheme"></param>
        internal static void RegisterBuiltInHighlightings(
            DefaultHighlightingManager hlm,
            string namespaceBase,
            string currentTheme)
        {
            hlm.RegisterHighlighting("XmlDoc", null, "XmlDoc.xshd");

            hlm.RegisterHighlighting("C#", new[] { ".cs" }, "CSharp-Mode.xshd");

            hlm.RegisterHighlighting("JavaScript", new[] { ".js" }, "JavaScript-Mode.xshd");
            hlm.RegisterHighlighting("HTML", new[] { ".htm", ".html" }, "HTML-Mode.xshd");
            hlm.RegisterHighlighting("ASP/XHTML", new[] { ".asp", ".aspx", ".asax", ".asmx", ".ascx", ".master" }, "ASPX.xshd");

            hlm.RegisterHighlighting("Boo", new[] { ".boo" }, "Boo.xshd");
            hlm.RegisterHighlighting("Coco", new[] { ".atg" }, "Coco-Mode.xshd");
            hlm.RegisterHighlighting("CSS", new[] { ".css" }, "CSS-Mode.xshd");
            hlm.RegisterHighlighting("C++", new[] { ".c", ".h", ".cc", ".cpp", ".hpp" }, "CPP-Mode.xshd");
            hlm.RegisterHighlighting("Java", new[] { ".java" }, "Java-Mode.xshd");
            hlm.RegisterHighlighting("Patch", new[] { ".patch", ".diff" }, "Patch-Mode.xshd");
            hlm.RegisterHighlighting("PowerShell", new[] { ".ps1", ".psm1", ".psd1" }, "PowerShell.xshd");
            hlm.RegisterHighlighting("PHP", new[] { ".php" }, "PHP-Mode.xshd");
            hlm.RegisterHighlighting("Python", new[] { ".py", ".pyw" }, "Python-Mode.xshd");
            hlm.RegisterHighlighting("TeX", new[] { ".tex" }, "Tex-Mode.xshd");
            hlm.RegisterHighlighting("TSQL", new[] { ".sql" }, "TSQL-Mode.xshd");
            hlm.RegisterHighlighting("VB", new[] { ".vb" }, "VB-Mode.xshd");
            hlm.RegisterHighlighting("XML", (".xml;.xsl;.xslt;.xsd;.manifest;.config;.addin;" +
                                             ".xshd;.wxs;.wxi;.wxl;.proj;.csproj;.vbproj;.ilproj;" +
                                             ".booproj;.build;.xfrm;.targets;.xaml;.xpt;" +
                                             ".xft;.map;.wsdl;.disco;.ps1xml;.nuspec").Split(';'),
                                     "XML-Mode.xshd");
            hlm.RegisterHighlighting("MarkDown", new[] { ".md" }, "MarkDown-Mode.xshd");

            // Additional Highlightings
            
            hlm.RegisterHighlighting("ActionScript3", new[] { ".as" }, "AS3.xshd");
            hlm.RegisterHighlighting("BAT", new[] { ".bat",".dos" }, "DOSBATCH.xshd");
            hlm.RegisterHighlighting("F#", new[] { ".fs" }, "FSharp-Mode.xshd");
            hlm.RegisterHighlighting("HLSL", new[] { ".fx" }, "HLSL.xshd");
            hlm.RegisterHighlighting("INI", new[] { ".cfg", ".conf", ".ini", ".iss" }, "INI.xshd");
            hlm.RegisterHighlighting("LOG", new[] { ".log" }, "Log.xshd");
            hlm.RegisterHighlighting("Pascal", new[] { ".pas" }, "Pascal.xshd");
            hlm.RegisterHighlighting("PLSQL", new[] { ".plsql" }, "PLSQL.xshd");
            hlm.RegisterHighlighting("Ruby", new[] { ".rb" }, "Ruby.xshd");
            hlm.RegisterHighlighting("Scheme", new[] { ".sls", ".sps", ".ss", ".scm" }, "scheme.xshd");
            hlm.RegisterHighlighting("Squirrel", new[] { ".nut" }, "squirrel.xshd");
            hlm.RegisterHighlighting("TXT", new[] { ".txt" }, "TXT.xshd");
            hlm.RegisterHighlighting("VTL", new[] { ".vtl", ".vm" }, "vtl.xshd");
        }
    }
}
