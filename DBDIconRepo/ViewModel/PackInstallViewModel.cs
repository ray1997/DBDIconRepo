using CommunityToolkit.Mvvm.ComponentModel;
using DBDIconRepo.Helper;
using DBDIconRepo.Model;
using IconPack.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using PackInfo = IconPack.Helper.Info;

namespace DBDIconRepo.ViewModel
{
    public class PackInstallViewModel : ObservableObject
    {
        Pack? _selected;
        public Pack? SelectedPack
        {
            get => _selected;
            set => SetProperty(ref _selected, value);
        }

        ObservableCollection<IPackSelectionItem>? _selections;
        public ObservableCollection<IPackSelectionItem>? InstallableItems
        {
            get => _selections;
            set => SetProperty(ref _selections, value);
        }

        public ICommand? InstallPack { get; private set; }
        public PackInstallViewModel() { }
        public PackInstallViewModel(Pack? selected)
        {
            SelectedPack = selected;
            InstallableItems = new ObservableCollection<IPackSelectionItem>();
            var pack = new ObservableCollection<IPackSelectionItem>();
            foreach (var file in SelectedPack.ContentInfo.Files)
            {
                PackSelectionHelper.RootWork(file, ref pack);
            }
            //Sorting
            PackSelectionHelper.Sort(ref pack);

            InstallableItems = new ObservableCollection<IPackSelectionItem>(pack);
        }
    }
}
