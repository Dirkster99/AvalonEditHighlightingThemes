namespace HL.HighlightingTheme
{
    using HL.Xshtd.interfaces;
    using System.Windows.Media;

    public class GlobalStyle : AbstractFreezable, IFreezable
    {
        #region fields
        private string _TypeName;
        private Color? _Foregroundcolor;
        private Color? _Backgroundcolor;
        private Color? _Bordercolor;
        #endregion fields

        #region ctors
        /// <summary>
        /// Construct a named (eg. 'Comment') WordStyle object
        /// </summary>
        /// <param name="typeName"></param>
        public GlobalStyle(string typeName)
          : this()
        {
            this.TypeName = typeName;
        }

        /// <summary>
        /// Hidden standard constructor
        /// </summary>
        protected GlobalStyle()
        {
            this.TypeName = string.Empty;
            _Foregroundcolor = null;
            _Backgroundcolor = null;
            _Bordercolor = null;
        }
        #endregion ctors

        #region properties
        /// <summary>
        /// Typed name of <seealso cref="GlobalStyle"/> object
        /// 
        /// (eg 'DefaultStyle', 'NonPrintableCharacter' ...,
        /// (this is usually the key in a collection of these styles)
        /// </summary>
        public string TypeName
        {
            get
            {
                return _TypeName;
            }
            set
            {
                if (IsFrozen)
                    throw new System.InvalidOperationException("Property is already frozen.");

                _TypeName = value;
            }
        }

        /// <summary>
        /// Get/set brush definition for the foreground used in this style
        /// </summary>
        public Color? foregroundcolor
        {
            get
            {
                return _Foregroundcolor;
            }
            set
            {
                if (IsFrozen)
                    throw new System.InvalidOperationException("Property is already frozen.");

                _Foregroundcolor = value;
            }
        }

        /// <summary>
        /// Get/set brush definition for the background used in this style
        /// </summary>
        public Color? backgroundcolor
        {
            get
            {
                return _Backgroundcolor;
            }
            set
            {
                if (IsFrozen)
                    throw new System.InvalidOperationException("Property is already frozen.");

                _Backgroundcolor = value;
            }
        }

        /// <summary>
        /// Get/set brush definition for the border used in this style
        /// </summary>
        public Color? bordercolor
        {
            get
            {
                return _Bordercolor;
            }
            set
            {
                if (IsFrozen)
                    throw new System.InvalidOperationException("Property is already frozen.");

                _Bordercolor = value;
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
            return "[" + (string.IsNullOrEmpty(this.TypeName) ? string.Empty : this.TypeName) + "]";
        }
        #endregion methods
    }
}
