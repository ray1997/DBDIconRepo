using ModernWpf.Controls.Primitives;
using System;
using System.Windows;
using System.Windows.Controls;

namespace DBDIconRepo.Dialog
{
    /// <summary>
    /// Interaction logic for PackInstall.xaml
    /// </summary>
    public partial class PackInstall : Window
    {
        public PackInstall()
        {
            InitializeComponent();
        }

        public PackInstall(IconPack.Model.Pack? selected)
        {
            InitializeComponent();
            DataContext = new ViewModel.PackInstallViewModel(selected);
        }

        private void OpenAttatchedFlyout(object sender, RoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }
    }
}
