namespace HL.HighlightingTheme
{
    using HL.Xshtd.interfaces;
    using ICSharpCode.AvalonEdit.Highlighting;
    using ICSharpCode.AvalonEdit.Utils;
    using System;
    using System.Collections.Generic;

    public class SyntaxDefinition : AbstractFreezable, IFreezable
    {
        #region fields
        string _Name;
        private readonly Dictionary<string, HighlightingColor> _NamedHighlightingColors;
        #endregion fields

        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        public SyntaxDefinition(string paramName)
            : this()
        {
            this._Name = paramName;
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        public SyntaxDefinition()
        {
            this.Extensions = new NullSafeCollection<string>();
            _NamedHighlightingColors = new Dictionary<string, HighlightingColor>();
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
                return _Name;
            }
            set
            {
                if (IsFrozen)
                    throw new InvalidOperationException();

                _Name = value;
            }
        }

        /// <summary>
        /// Gets the associated extensions.
        /// </summary>
        public IList<string> Extensions { get; private set; }

        /// <summary>
        /// Gets an enumeration of all highlighting colors that are defined
        /// for this highlighting pattern (eg. C#) as part of a highlighting theme (eg 'True Blue').
        /// </summary>
        public IEnumerable<HighlightingColor> NamedHighlightingColors
        {
            get
            {
                return _NamedHighlightingColors.Values;
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Prevent further changes to this highlighting color.
        /// </summary>
        protected override void FreezeInternal()
        {
            base.FreezeInternal();
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return "[" + GetType().Name + " " + (string.IsNullOrEmpty(this.Name) ? string.Empty : this.Name) + "]";
        }

        public HighlightingColor ColorGet(string name)
        {
            HighlightingColor color;
            if (_NamedHighlightingColors.TryGetValue(name, out color))
                return color;

            return null;
        }

        public void ColorAdd(HighlightingColor color)
        {
            _NamedHighlightingColors.Add(color.Name, color);
        }

        internal void ColorReplace(string name, HighlightingColor themeColor)
        {
            _NamedHighlightingColors.Remove(name);
            _NamedHighlightingColors.Add(name, themeColor);
        }
        #endregion methods
    }
}
