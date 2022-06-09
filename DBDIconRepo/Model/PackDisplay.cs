using CommunityToolkit.Mvvm.ComponentModel;
using IconPack.Model;

namespace DBDIconRepo.Model
{
    public class PackDisplay : ObservableObject
    {
        Pack? _base;
        public Pack? Info
        {
            get => _base;
            set => SetProperty(ref _base, value);
        }

        string _owner;
        public string Owner
        {
            get => _owner;
            set => SetProperty(ref _owner, value);
        }

        string _repo;
        public string Repository
        {
            get => _repo;
            set => SetProperty(ref _repo, value);
        }

        //Include images
        public string FirstPerkSource => $"{System.Environment.CurrentDirectory}\\Cache\\{Repository}\\Perks\\{Setting.Instance.PerkPreviewSelection[0]}.png";

        public string SecondPerkSource => $"{System.Environment.CurrentDirectory}\\Cache\\{Repository}\\Perks\\{Setting.Instance.PerkPreviewSelection[1]}.png";
        public string ThirdPerkSource => $"{System.Environment.CurrentDirectory}\\Cache\\{Repository}\\Perks\\{Setting.Instance.PerkPreviewSelection[2]}.png";
        public string ForthPerkSource => $"{System.Environment.CurrentDirectory}\\Cache\\{Repository}\\Perks\\{Setting.Instance.PerkPreviewSelection[3]}.png";
    }
}
