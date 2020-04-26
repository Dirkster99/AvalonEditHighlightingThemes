namespace ThemedDemo.ViewModels
{
    using MLib.Interfaces;

    public class ThemeDefinitionViewModel : Base.ViewModelBase
    {
        #region private fields
        private bool _IsSelected;
        readonly private IThemeInfo _model;
        #endregion private fields

        #region constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="model"></param>
        public ThemeDefinitionViewModel(IThemeInfo model)
        {
            _model = model;
        }

        protected ThemeDefinitionViewModel()
        {
            _model = null;
            _IsSelected = false;
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Gets the static theme model based data items.
        /// </summary>
        public IThemeInfo Model
        {
            get
            {
                return _model;
            }
        }

        /// <summary>
        /// Determines whether this theme is currently selected or not.
        /// </summary>
        public bool IsSelected
        {
            get { return _IsSelected; }

            set
            {
                if (_IsSelected != value)
                {
                    _IsSelected = value;
                }
            }
        }
        #endregion properties
    }
}
