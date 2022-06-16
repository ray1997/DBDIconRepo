using DBDIconRepo.ViewModel;
using IconPack.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DBDIconRepo.Dialog
{
    /// <summary>
    /// Interaction logic for PackDetail.xaml
    /// </summary>
    public partial class PackDetail : Window
    {
        public PackDetail(Pack? selected) => Initialize(selected);

        private void Initialize(Pack? selected)
        {
            InitializeComponent();
            this.Loaded += WindowLoaded;
            this.Unloaded += WindowUnloaded;
            DataContext = null;
            DataContext = new PackDetailViewModel(selected);
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void WindowUnloaded(object sender, RoutedEventArgs e)
        {

        }

        private void DetermineYScroll(object sender, ScrollChangedEventArgs e)
        {
            if ((DataContext as PackDetailViewModel)?.CurrentDisplayMode != DetailFocusMode.Overview)
            {
                topDownloadButton.Visibility = Visibility.Visible;
                return;
            }
            topDownloadButton.Visibility =
                mainContentScroll.ContentVerticalOffset > packDetailPanel.RenderSize.Height ? 
                Visibility.Visible : Visibility.Collapsed;
            System.Diagnostics.Debug.WriteLine($"Item height: {packDetailPanel.RenderSize.Height}" +
                $"\r\nCurrent position? {mainContentScroll.ContentVerticalOffset} || {mainContentScroll.VerticalOffset}");
        }

        private void SetDetailFocusModeViaTab(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0) 
                if (e.AddedItems[0] is TabItem tab)
                    if (tab.Tag is string str)
                    {
                        if (topDownloadButton != null)
                            topDownloadButton.Visibility = str != "Overview" ? Visibility.Visible :
                                (mainContentScroll.ContentVerticalOffset > packDetailPanel.RenderSize.Height ?
                                Visibility.Visible : Visibility.Collapsed);
                        if ((DataContext as PackDetailViewModel)?.CurrentDisplayMode.ToString() == str)
                            return;
                        switch (str)
                        {
                            case "Overview":
                                (DataContext as PackDetailViewModel).CurrentDisplayMode = DetailFocusMode.Overview;
                                break;
                            case "Readme":
                                (DataContext as PackDetailViewModel).CurrentDisplayMode = DetailFocusMode.Readme;
                                break;
                            case "Perks":
                                (DataContext as PackDetailViewModel).CurrentDisplayMode = DetailFocusMode.Perks;
                                break;
                        }
                    }
        }
    }
}
