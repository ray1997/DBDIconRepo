using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DBDIconRepo.Helper;
using DBDIconRepo.Model;
using IconPack.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DBDIconRepo.ViewModel
{
    public class PackDetailViewModel : ObservableObject
    {
        Pack? _selected;
        public Pack? SelectedPack
        {
            get => _selected;
            set => SetProperty(ref _selected, value);
        }

        public PackDetailViewModel() { }
        public PackDetailViewModel(Pack? selected)
        {
            PrepareCommands();
            SelectedPack = selected;
            PrepareDisplayData();
        }

        public void PrepareDisplayData()
        {
            string path = CacheOrGit.GetDisplayContentPath(SelectedPack.Repository.Owner, SelectedPack.Repository.Name);
            HasBanner = File.Exists($"{path}\\.banner.png");
            if (HasBanner)
            {
                BannerURL = URL.GetGithubRawContent(SelectedPack.Repository, ".banner.png");
            }

            //Get any previewable icon
            foreach (var item in Setting.Instance.PerkPreviewSelection)
            {
                if (SelectedPack.ContentInfo.Files.FirstOrDefault(icon => icon.ToLower().Contains(item.ToLower())) is string match)
                {
                    HeroIconURL = URL.GetIconAsGitRawContent(SelectedPack.Repository, match);
                    break;
                }
            }

            //Readme.md
            HasReadmeMD = File.Exists($"{path}\\readme.md");
            if (HasReadmeMD)
            {
                string md = File.ReadAllText($"{path}\\readme.md");
                MdXaml.Markdown translator = new();
                ReadmeMDContent = translator.Transform(md);
            }

            //Perk icons
            if (SelectedPack.ContentInfo.HasPerks)
            {
                var perks = SelectedPack.ContentInfo.Files
                    .Where(file => file.StartsWith("Perks") ||
                    file.StartsWith("/Perks") ||
                    file.StartsWith("\\Perks"))
                    .Select(i => new PerkPreviewItem(i, SelectedPack.Repository))
                    .Where(preview => preview.Perk is not null)
                    .Shuffle();
                PerksPreview = new ObservableCollection<PerkPreviewItem>(perks);
            }
        }

        DetailFocusMode _dm = DetailFocusMode.Overview;
        public DetailFocusMode CurrentDisplayMode
        {
            get => _dm;
            set => SetProperty(ref _dm, value);
        }

        bool _hasReadme;
        public bool HasReadmeMD
        {
            get => _hasReadme;
            set => SetProperty(ref _hasReadme, value);
        }

        FlowDocument _readme = new();
        public FlowDocument ReadmeMDContent
        {
            get => _readme;
            set => SetProperty(ref _readme, value);
        }

        bool _hasBanner;
        public bool HasBanner
        {
            get => _hasBanner;
            set => SetProperty(ref _hasBanner, value);
        }

        string? _bannerURL;
        public string? BannerURL
        {
            get => _bannerURL;
            set => SetProperty(ref _bannerURL, value);
        }

        string? _heroIconURL = null;
        public string? HeroIconURL
        {
            get => _heroIconURL;
            set => SetProperty(ref _heroIconURL, value);
        }

        //Perks display
        ObservableCollection<PerkPreviewItem>? _perks;
        public ObservableCollection<PerkPreviewItem>? PerksPreview
        {
            get => _perks;
            set => SetProperty(ref _perks, value);
        }

        public ObservableCollection<PerkPreviewItem>? SortedPerks
        {
            get
            {
                if (PerksPreview is null)
                    return null;
                switch (CurrentPerkSortingMethod)
                {
                    case PerkSortBy.Name:
                        if (IsPerkSortByAscending)
                            return new ObservableCollection<PerkPreviewItem>(
                                PerksPreview.OrderBy(perk => perk.Perk.Name));
                        else
                            return new ObservableCollection<PerkPreviewItem>(
                                PerksPreview.OrderByDescending(perk => perk.Perk.Name));
                        break;
                    case PerkSortBy.Owner:
                        if (IsPerkSortByAscending)
                            return new ObservableCollection<PerkPreviewItem>(
                                PerksPreview.OrderBy(perk => perk.Perk.PerkOwner));
                        else
                            return new ObservableCollection<PerkPreviewItem>(
                                PerksPreview.OrderByDescending(perk => perk.Perk.PerkOwner));
                        break;
                    case PerkSortBy.Random:
                        return new ObservableCollection<PerkPreviewItem>(
                            PerksPreview.Shuffle());
                        break;
                }
                return new ObservableCollection<PerkPreviewItem>(PerksPreview);
            }
        }

        PerkSortBy _perkSortBy = PerkSortBy.Random;
        public PerkSortBy CurrentPerkSortingMethod
        {
            get => _perkSortBy;
            set
            {
                if (SetProperty(ref _perkSortBy, value))
                    SortingPerkList();
            }
        }

        bool _sortPerkAscending = true;
        public bool IsPerkSortByAscending
        {
            get => _sortPerkAscending;
            set
            {
                if (SetProperty(ref _sortPerkAscending, value))
                    SortingPerkList();
            }
        }

        private void SortingPerkList()
        {
            OnPropertyChanged(nameof(SortedPerks));
            //if (PerksPreview is null)
            //    return;
            //switch (CurrentPerkSortingMethod)
            //{
            //    case PerkSortBy.Name:
            //        if (IsPerkSortByAscending)
            //            PerksPreview = new ObservableCollection<PerkPreviewItem>(
            //                PerksPreview.OrderBy(perk => perk.Perk.Name));
            //        else
            //            PerksPreview = new ObservableCollection<PerkPreviewItem>(
            //                PerksPreview.OrderByDescending(perk => perk.Perk.Name));
            //        break;
            //    case PerkSortBy.Owner:
            //        if (IsPerkSortByAscending)
            //            PerksPreview = new ObservableCollection<PerkPreviewItem>(
            //                PerksPreview.OrderBy(perk => perk.Perk.PerkOwner));
            //        else
            //            PerksPreview = new ObservableCollection<PerkPreviewItem>(
            //                PerksPreview.OrderByDescending(perk => perk.Perk.PerkOwner));
            //        break;
            //    case PerkSortBy.Random:
            //        PerksPreview = new ObservableCollection<PerkPreviewItem>(
            //            PerksPreview.Shuffle());
            //        break;
            //}
        }

        public ICommand? SetDisplayMode { get; private set; }
        public ICommand? SetPerkSortingMethod { get; private set; }
        public ICommand? SetPerkSortingAscendingMethod { get; private set; }
        private void PrepareCommands()
        {
            SetDisplayMode = new RelayCommand<DetailFocusMode>(SetDisplayModeAction);
            SetPerkSortingMethod = new RelayCommand<string?>(SetPerkSortingMethodAction);
            SetPerkSortingAscendingMethod = new RelayCommand<string?>(SetPerkSortingAscendingMethodAction);
        }

        private void SetPerkSortingAscendingMethodAction(string? str)
        {
            if (str is null)
                return;
            IsPerkSortByAscending = str == "true";
        }

        private void SetPerkSortingMethodAction(string? obj)
        {
            if (obj is null)
                return;
            CurrentPerkSortingMethod = Enum.Parse<PerkSortBy>(obj);
        }

        private void SetDisplayModeAction(DetailFocusMode obj)
        {
            CurrentDisplayMode = obj;
        }
    }

    public enum DetailFocusMode
    {
        Overview,
        Readme,
        Perks
    }

    public enum PerkSortBy
    {
        Name,
        Owner,
        Random
    }
}
