namespace HL.HighlightingTheme
{
    using HL.Xshtd.interfaces;
    using ICSharpCode.AvalonEdit.Highlighting;
    using ICSharpCode.AvalonEdit.Utils;
    using System;
    using System.Collections.Generic;

    public class SyntaxDefinition : IFreezable
    {
        #region fields
        string name;
        bool frozen;
        private readonly Dictionary<string, HighlightingColor> _colors;
        #endregion fields

        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        public SyntaxDefinition(string paramName)
            : this()
        {
            this.name = paramName;
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        public SyntaxDefinition()
        {
            this.Extensions = new NullSafeCollection<string>();
            _colors = new Dictionary<string, HighlightingColor>();
        }
        #endregion ctors

        #region properties
        /// <summary>
        /// Gets/Sets the name of the color.
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (frozen)
                    throw new InvalidOperationException();

                name = value;
            }
        }

        /// <summary>
        /// Gets the associated extensions.
        /// </summary>
        public IList<string> Extensions { get; private set; }

        /// <summary>
        /// Gets whether this HighlightingColor instance is frozen.
        /// </summary>
        public bool IsFrozen
        {
            get { return frozen; }
        }

        /// <summary>
        /// Gets an enumeration of all highlighting colors that are defined
        /// for this highlighting pattern (eg. C#) as part of a highlighting theme (eg 'True Blue').
        /// </summary>
        public IEnumerable<HighlightingColor> NamedHighlightingColors
        {
            get
            {
                return _colors.Values;
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Prevent further changes to this highlighting color.
        /// </summary>
        public virtual void Freeze()
        {
            frozen = true;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return "[" + GetType().Name + " " + (string.IsNullOrEmpty(this.Name) ? string.Empty : this.Name) + "]";
        }

        public HighlightingColor ColorGet(string name)
        {
            HighlightingColor color;
            if (_colors.TryGetValue(name, out color))
                return color;

            return null;
        }

        public void ColorAdd(HighlightingColor color)
        {
            _colors.Add(color.Name, color);
        }

        internal void ColorReplace(string name, HighlightingColor themeColor)
        {
            _colors.Remove(name);
            _colors.Add(name, themeColor);
        }
        #endregion methods
    }
}
