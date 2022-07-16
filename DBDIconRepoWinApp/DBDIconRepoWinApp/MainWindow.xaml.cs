using DBDIconRepo.Model;
using DBDIconRepo.ViewModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Messenger = CommunityToolkit.Mvvm.Messaging.WeakReferenceMessenger;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DBDIconRepoWinApp
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public HomeViewModel ViewModel { get; } = new HomeViewModel();
        public MainWindow()
        {
            this.InitializeComponent();
            Messenger.Default.Register<MainWindow, RequestViewPackDetailMessage, string>(this,
                MessageToken.REQUESTVIEWPACKDETAIL, OpenPackDetailWindow);
        }

        private void OpenPackDetailWindow(MainWindow recipient, RequestViewPackDetailMessage message)
        {
            //foreach (var window in Application.Current.Windows)
            //{
            //    if (window is PackDetail pd)
            //    {
            //        if (pd.ViewModel.SelectedPack == message.Selected)
            //        {
            //            pd.Hide();
            //            pd.Show();
            //            return;
            //        }
            //    }
            //}

            //PackDetail detail = new PackDetail(message.Selected);
            //detail.Show();
        }

        private void UnregisterStuff(object sender, RoutedEventArgs e)
        {
            ViewModel.UnregisterMessages();
        }

        private void LoadPacklist(object sender, RoutedEventArgs e)
        {
            ViewModel.InitializeViewModel();
        }

        private void OpenAttatchedFlyout(object sender, RoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }
    }
    public class IconPreviewTemplateSelector : DataTemplateSelector
    {
        public DataTemplate IconDisplay { get; set; }
        public DataTemplate BannerDisplay { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item is IconDisplay)
                return IconDisplay;
            else
                return BannerDisplay;
        }

    }
}
