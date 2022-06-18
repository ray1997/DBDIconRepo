using CommunityToolkit.Mvvm.ComponentModel;
using DBDIconRepo.Helper;
using DBDIconRepo.Model;
using IconPack.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

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

        ObservableCollection<PackSelectionItem>? _selectionItems;
        public ObservableCollection<PackSelectionItem>? InstallableItems
        {
            get => _selectionItems;
            set => SetProperty(ref _selectionItems, value);
        }

        public ICommand? InstallPack { get; private set; }
        public PackInstallViewModel() { }
        public PackInstallViewModel(Pack? selected)
        {
            SelectedPack = selected;
            var allFiles = SelectedPack.ContentInfo.Files
                .Where(file => file.EndsWith(".png"))
                .Select(file => new PackSelectionItem(file))
                .OrderBy(item => item.FolderName)
                .ThenBy(item => item.SubFolderName is null)
                .ThenBy(item => item.Name);
            InstallableItems = new ObservableCollection<PackSelectionItem>(allFiles);
            Task.Run(async () =>
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    foreach (var item in InstallableItems)
                        item.IsSelected = true;
                }, System.Windows.Threading.DispatcherPriority.Loaded);
            }).Await();
        }

        
    }
}
