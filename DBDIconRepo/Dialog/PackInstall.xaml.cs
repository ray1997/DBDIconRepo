using ModernWpf.Controls.Primitives;
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

    public class InstallSelectionTemplator : DataTemplateSelector
    {
        public DataTemplate? GenericNoInfoItem { get; set; }
        public DataTemplate? GenericItemTemplate { get; set; }
        public DataTemplate? PerkItemSelection { get; set; }
        public DataTemplate? KillerPowerSelection { get; set; }
        public DataTemplate? ItemAddonsSelection { get; set; }
        public DataTemplate? KillerPowerAddonsSelection { get; set; }
        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            if (item is Model.PackSelectionItem psi)
            {
                if (psi.Name is null)
                    return GenericNoInfoItem;
                if (psi.Info is null)
                    return GenericItemTemplate;
                else
                {
                    switch (psi.Info)
                    {
                        case IconPack.Model.PerkInfo: return PerkItemSelection;
                        case IconPack.Model.KillerPowerInfo: return KillerPowerSelection;
                        case IconPack.Model.AddOnsInfo addons:
                            if (addons.Owner is null)
                                return ItemAddonsSelection;
                            return KillerPowerAddonsSelection;
                    }
                }
            }
            return GenericItemTemplate;
        }
    }
}
