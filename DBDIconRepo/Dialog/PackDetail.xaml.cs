using DBDIconRepo.ViewModel;
using IconPack.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
            DataContext = new PackDetailViewModel(selected);
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {

        }

        private void WindowUnloaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
