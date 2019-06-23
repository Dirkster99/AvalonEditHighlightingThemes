namespace ThemedDemo.ViewModels
{
    using HL.Interfaces;
    using ICSharpCode.AvalonEdit.Document;
    using ICSharpCode.AvalonEdit.Highlighting;
    using ICSharpCode.AvalonEdit.Utils;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Text;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;
    using ThemedDemo.ViewModels.Base;

    /// <summary>
    /// Implements a viewmodel that manages the content of an AvalonEdit based text document.
    /// </summary>
    public class DocumentRootViewModel : Base.ViewModelBase
    {
        #region fields
        private string _FilePath;

        private TextDocument _Document;
        private bool _IsDirty;
        private bool _IsReadOnly;
        private string _IsReadOnlyReason = string.Empty;

        private ICommand _HighlightingChangeCommand;
        private IHighlightingDefinition _HighlightingDefinition;
        #endregion fields

        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        public DocumentRootViewModel()
        {
            Document = new TextDocument();
        }
        #endregion ctors

        #region properties
        public TextDocument Document
        {
            get { return _Document; }
            set
            {
                if (_Document != value)
                {
                    _Document = value;
                    NotifyPropertyChanged(() => Document);
                }
            }
        }

        public bool IsDirty
        {
            get { return _IsDirty; }
            set
            {
                if (_IsDirty != value)
                {
                    _IsDirty = value;
                    NotifyPropertyChanged(() => IsDirty);
                }
            }
        }

        public string FilePath
        {
            get { return _FilePath; }
            set
            {
                if (_FilePath != value)
                {
                    _FilePath = value;

                    NotifyPropertyChanged(() => FilePath);
                }
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return _IsReadOnly;
            }

            protected set
            {
                if (_IsReadOnly != value)
                {
                    _IsReadOnly = value;
                    NotifyPropertyChanged(() => IsReadOnly);
                }
            }
        }

        public string IsReadOnlyReason
        {
            get
            {
                return _IsReadOnlyReason;
            }

            protected set
            {
                if (_IsReadOnlyReason != value)
                {
                    _IsReadOnlyReason = value;
                    NotifyPropertyChanged(() => IsReadOnlyReason);
                }
            }
        }

        #region Highlighting Definition
        /// <summary>
        /// Gets an (ordered by Name) list copy of all highlightings defined in this object
        /// or an empty collection if there is no highlighting definition available.
        /// </summary>
        public ReadOnlyCollection<IHighlightingDefinition> HighlightingDefinitions
        {
            get
            {
                var hlManager = GetService<IThemedHighlightingManager>();

                if (hlManager != null)
                  return hlManager.HighlightingDefinitions;

                return null;
            }
        }

        /// <summary>
        /// AvalonEdit exposes a Highlighting property that controls whether keywords,
        /// comments and other interesting text parts are colored or highlighted in any
        /// other visual way. This property exposes the highlighting information for the
        /// text file managed in this viewmodel class.
        /// </summary>
        public IHighlightingDefinition HighlightingDefinition
        {
            get
            {
                return _HighlightingDefinition;
            }

            set
            {
                if (_HighlightingDefinition != value)
                {
                    _HighlightingDefinition = value;
                    NotifyPropertyChanged(() => HighlightingDefinition);
                }
            }
        }

        /// <summary>
        /// Gets a command that changes the currently selected syntax highlighting in the editor.
        /// </summary>
        public ICommand HighlightingChangeCommand
        {
            get
            {
                if (_HighlightingChangeCommand == null)
                {
                    _HighlightingChangeCommand = new RelayCommand<object>((p) =>
                    {
                        var parames = p as object[];

                        if (parames == null)
                            return;

                        if (parames.Length != 1)
                            return;

                        var param = parames[0] as IHighlightingDefinition;
                        if (param == null)
                            return;

                        HighlightingDefinition = param;
                    });
                }

                return _HighlightingChangeCommand;
            }
        }
        #endregion Highlighting Definition
        #endregion properties

        #region methods
        public bool LoadDocument(string paramFilePath)
        {
            if (File.Exists(paramFilePath))
            {
                var hlManager = GetService<IThemedHighlightingManager>();

                Document = new TextDocument();
                string extension = System.IO.Path.GetExtension(paramFilePath);
                HighlightingDefinition = hlManager.GetDefinitionByExtension(extension);

                IsDirty = false;
                IsReadOnly = false;

                // Check file attributes and set to read-only if file attributes indicate that
                if ((System.IO.File.GetAttributes(paramFilePath) & FileAttributes.ReadOnly) != 0)
                {
                    IsReadOnly = true;
                    IsReadOnlyReason = "This file cannot be edit because another process is currently writting to it.\n" +
                                       "Change the file access permissions or save the file in a different location if you want to edit it.";
                }

                using (FileStream fs = new FileStream(paramFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (StreamReader reader = FileReader.OpenStream(fs, Encoding.UTF8))
                    {
                        Document = new TextDocument(reader.ReadToEnd());
                    }
                }

                FilePath = paramFilePath;
                NotifyPropertyChanged(() => HighlightingDefinitions);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Invoke this method to apply a change of theme to the content of the document
        /// (eg: Adjust the highlighting colors when changing from "Dark" to "Light"
        ///      WITH current text document loaded.)
        /// </summary>
        internal void OnAppThemeChanged(IThemedHighlightingManager hlManager)
        {
            if (hlManager == null)
                return;

            // Does this highlighting definition have an associated highlighting theme?
            if (hlManager.CurrentTheme.HlTheme != null)
            {
                // A highlighting theme with GlobalStyles?
                // Apply these styles to the resource keys of the editor
                foreach (var item in hlManager.CurrentTheme.HlTheme.GlobalStyles)
                {
                    switch (item.TypeName)
                    {
                        case "DefaultStyle":
                            ApplyToDynamicResource(TextEditLib.Themes.ResourceKeys.EditorBackground, item.backgroundcolor);
                            ApplyToDynamicResource(TextEditLib.Themes.ResourceKeys.EditorForeground, item.foregroundcolor);
                            break;

                        case "CurrentLineBackground":
                            ApplyToDynamicResource(TextEditLib.Themes.ResourceKeys.EditorCurrentLineBackgroundBrushKey, item.backgroundcolor);
                            ApplyToDynamicResource(TextEditLib.Themes.ResourceKeys.EditorCurrentLineBorderBrushKey, item.bordercolor);
                            break;

                        case "LineNumbersForeground":
                            ApplyToDynamicResource(TextEditLib.Themes.ResourceKeys.EditorLineNumbersForeground, item.foregroundcolor);
                            break;

                        case "Selection":
                            ApplyToDynamicResource(TextEditLib.Themes.ResourceKeys.EditorSelectionBrush, item.backgroundcolor);
                            ApplyToDynamicResource(TextEditLib.Themes.ResourceKeys.EditorSelectionBorder, item.bordercolor);
                            break;

                        case "Hyperlink":
                            ApplyToDynamicResource(TextEditLib.Themes.ResourceKeys.EditorLinkTextBackgroundBrush, item.backgroundcolor);
                            ApplyToDynamicResource(TextEditLib.Themes.ResourceKeys.EditorLinkTextForegroundBrush, item.foregroundcolor);
                            break;

                        case "NonPrintableCharacter":
                            ApplyToDynamicResource(TextEditLib.Themes.ResourceKeys.EditorNonPrintableCharacterBrush, item.foregroundcolor);
                            break;

                        default:
                            throw new System.ArgumentOutOfRangeException("GlobalStyle named '{0}' is not supported.", item.TypeName);
                    }
                }
            }

            // 1st try: Find highlighting based on currently selected highlighting
            // The highlighting name may be the same as before, but the highlighting theme has just changed
            if (HighlightingDefinition != null)
            {
                // Reset property for currently select highlighting definition
                HighlightingDefinition = hlManager.GetDefinition(HighlightingDefinition.Name);
                
                if (HighlightingDefinition != null)
                    return;
            }

            // 2nd try: Find highlighting based on extension of file currenlty being viewed
            if (string.IsNullOrEmpty(FilePath))
                return;

            string extension = System.IO.Path.GetExtension(FilePath);

            if (string.IsNullOrEmpty(extension))
                return;

            // Reset property for currently select highlighting definition
            HighlightingDefinition = hlManager.GetDefinitionByExtension(extension);
        }


        /// <summary>
        /// Re-define an existing <seealso cref="SolidColorBrush"/> and backup the originial color
        /// as it was before the application of the custom coloring.
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="newColor"></param>
        /// <param name="backupDynResources"></param>
        private void ApplyToDynamicResource(ComponentResourceKey key,
                                            Color? newColor)
        {
            if (Application.Current.Resources[key] == null || newColor == null)
                return;

            // Re-coloring works with SolidColorBrushs linked as DynamicResource
            if (Application.Current.Resources[key] is SolidColorBrush)
            {
                //backupDynResources.Add(resourceName);

                var newColorBrush = new SolidColorBrush((Color)newColor);
                newColorBrush.Freeze();

                Application.Current.Resources[key] = newColorBrush;
            }
        }
        #endregion methods
    }
}

