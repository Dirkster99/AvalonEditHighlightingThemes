using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using Settings.UserProfile;
using System.IO;
using System.Xml;
using ThemedDemo.ViewModels;

namespace ThemedDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MWindowLib.MetroWindow
                                     , IViewSize  // Implements saving and loading/repositioning of Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Loaded -= MainWindow_Loaded;
        }
    }
}
