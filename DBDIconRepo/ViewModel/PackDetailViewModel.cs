using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DBDIconRepo.Helper;
using DBDIconRepo.Model;
using DBDIconRepo.Model.Preview;
using IconPack.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

        public async void PrepareDisplayData()
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
                    .Where(preview => preview.Perk is not null);
                await Task.Run(async () =>
                {
                    foreach (var perk in perks)
                    {
                        await Application.Current.Dispatcher.InvokeAsync(() =>
                        {
                            if (PerksPreview is null)
                                PerksPreview = new ObservableCollection<PerkPreviewItem>();
                            PerksPreview.Add(perk);
                        }, System.Windows.Threading.DispatcherPriority.Background);
                    }
                    SortingPerkList();
                });
            }

            //Portrait icons
            if (SelectedPack.ContentInfo.HasPortraits)
            {
                var portraits = SelectedPack.ContentInfo.Files
                    .Where(file => file.StartsWith("CharPortraits") ||
                    file.StartsWith("/CharPortraits") ||
                    file.StartsWith("\\CharPortraits"))
                    .Select(i => new PortraitPreviewItem(i, SelectedPack.Repository))
                    .Where(preview => preview.Name is not null);
                await Task.Run(async () =>
                {
                    foreach (var portrait in portraits)
                    {
                        await Application.Current.Dispatcher.InvokeAsync(() =>
                        {
                            if (PortraitPreview is null)
                                PortraitPreview = new ObservableCollection<PortraitPreviewItem>();
                            PortraitPreview.Add(portrait);
                        }, System.Windows.Threading.DispatcherPriority.Background);
                    }
                });
                PortraitPreview = new ObservableCollection<PortraitPreviewItem>(portraits);
            }

            //Powers
            if (SelectedPack.ContentInfo.HasPowers)
            {
                var powers = SelectedPack.ContentInfo.Files
                    .Where(file => file.StartsWith("Powers") ||
                    file.StartsWith("/Powers") ||
                    file.StartsWith("\\Powers"))
                    .Select(i => new PowerPreviewItem(i, SelectedPack.Repository))
                    .Where(preview => preview.Name is not null);
                await Task.Run(async () =>
                {
                    foreach (var power in powers)
                    {
                        await Application.Current.Dispatcher.InvokeAsync(() =>
                        {
                            if (PowerPreview is null)
                                PowerPreview = new ObservableCollection<PowerPreviewItem>();
                            PowerPreview.Add(power);
                        }, System.Windows.Threading.DispatcherPriority.Background);
                    }
                }); 
            }

            //Items
            if (SelectedPack.ContentInfo.HasItems)
            {
                var items = SelectedPack.ContentInfo.Files
                    .Where(file => file.StartsWith("items") ||
                    file.StartsWith("/items") ||
                    file.StartsWith("\\items"))
                    .Select(i => new ItemPreviewItem(i, SelectedPack.Repository))
                    .Where(preview => preview.Name is not null);
                await Task.Run(async () =>
                {
                    foreach (var item in items)
                    {
                        await Application.Current.Dispatcher.InvokeAsync(() =>
                        {
                            if (ItemsPreview is null)
                                ItemsPreview = new ObservableCollection<ItemPreviewItem>();
                            ItemsPreview.Add(item);
                        }, System.Windows.Threading.DispatcherPriority.Background);
                    }
                });
            }

            //Status
            if (SelectedPack.ContentInfo.HasStatus)
            {
                var status = SelectedPack.ContentInfo.Files
                    .Where(file => file.StartsWith("StatusEffects") ||
                    file.StartsWith("/StatusEffects") ||
                    file.StartsWith("\\StatusEffects"))
                    .Select(i => new StatusEffectPreviewItem(i, SelectedPack.Repository))
                    .Where(preview => preview.Name is not null);
                await Task.Run(async () =>
                {
                    foreach (var st in status)
                    {
                        await Application.Current.Dispatcher.InvokeAsync(() =>
                        {
                            if (StatusEffectsPreview is null)
                                StatusEffectsPreview = new ObservableCollection<StatusEffectPreviewItem>();
                            StatusEffectsPreview.Add(st);
                        }, System.Windows.Threading.DispatcherPriority.Background);
                    }
                });
            }

            //Offerings
            if (SelectedPack.ContentInfo.HasOfferings)
            {
                var offerings = SelectedPack.ContentInfo.Files
                    .Where(file => file.StartsWith("Favors") ||
                    file.StartsWith("/Favors") ||
                    file.StartsWith("\\Favors"))
                    .Select(i => new OfferingPreviewItem(i, SelectedPack.Repository))
                    .Where(preview => preview.Name is not null);
                await Task.Run(async () =>
                {
                    foreach (var offering in offerings)
                    {
                        await Application.Current.Dispatcher.InvokeAsync(() =>
                        {
                            if (OfferingsPreview is null)
                                OfferingsPreview = new ObservableCollection<OfferingPreviewItem>();
                            OfferingsPreview.Add(offering);
                        }, System.Windows.Threading.DispatcherPriority.Background);
                    }
                });
            }

            //Addons
            if (SelectedPack.ContentInfo.HasAddons)
            {
                var addons = SelectedPack.ContentInfo.Files
                    .Where(file => file.StartsWith("ItemAddons") ||
                    file.StartsWith("/ItemAddons") ||
                    file.StartsWith("\\ItemAddons"))
                    .Select(i => new AddonPreviewItem(i, SelectedPack.Repository))
                    .Where(preview => preview.Name is not null);
                await Task.Run(async () =>
                {
                    foreach (var addon in addons)
                    {
                        await Application.Current.Dispatcher.InvokeAsync(() =>
                        {
                            if (AddonsPreview is null)
                                AddonsPreview = new ObservableCollection<AddonPreviewItem>();
                            AddonsPreview.Add(addon);
                        }, System.Windows.Threading.DispatcherPriority.Background);
                    }
                });
            }


            //TODO:When showing emblems sort it by name, then by type (none, silver, gold, iri etc.)
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

        //Portraits display
        ObservableCollection<PortraitPreviewItem>? _portrait;
        public ObservableCollection<PortraitPreviewItem>? PortraitPreview
        {
            get => _portrait;
            set => SetProperty(ref _portrait, value);
        }

        //Powers display
        ObservableCollection<PowerPreviewItem>? _power;
        public ObservableCollection<PowerPreviewItem>? PowerPreview
        {
            get => _power;
            set => SetProperty(ref _power, value);
        }

        //Items display
        ObservableCollection<ItemPreviewItem>? _items;
        public ObservableCollection<ItemPreviewItem>? ItemsPreview
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        //Addons
        ObservableCollection<AddonPreviewItem>? _addons;
        public ObservableCollection<AddonPreviewItem>? AddonsPreview
        {
            get => _addons;
            set => SetProperty(ref _addons, value);
        }

        //Emblems
        ObservableCollection<EmblemPreviewItem>? _emblem;
        public ObservableCollection<EmblemPreviewItem>? EmblemPreview
        {
            get => _emblem;
            set => SetProperty(ref _emblem, value);
        }

        //Daily ritual
        ObservableCollection<DailyRitualPreviewItem>? _dailyRitual;
        public ObservableCollection<DailyRitualPreviewItem>? DailyRitualPreview
        {
            get => _dailyRitual;
            set => SetProperty(ref _dailyRitual, value);
        }

        //Offering
        ObservableCollection<OfferingPreviewItem>? _offerings;
        public ObservableCollection<OfferingPreviewItem>? OfferingsPreview
        {
            get => _offerings;
            set => SetProperty(ref _offerings, value);
        }

        //Status effects
        ObservableCollection<StatusEffectPreviewItem>? _statusEffects;
        public ObservableCollection<StatusEffectPreviewItem>? StatusEffectsPreview
        {
            get => _statusEffects;
            set => SetProperty(ref _statusEffects, value);
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

        PerkSortBy _perkSortBy = PerkSortBy.Name;
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
        }

        public ICommand? SetDisplayMode { get; private set; }
        public ICommand? SetPerkSortingMethod { get; private set; }
        public ICommand? SetPerkSortingAscendingMethod { get; private set; }
        private void PrepareCommands()
        {
            SetDisplayMode = new RelayCommand<string?>(SetDisplayModeAction);
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

        private void SetDisplayModeAction(string? obj)
        {
            if (string.IsNullOrEmpty(obj))
                return;
            CurrentDisplayMode = Enum.Parse<DetailFocusMode>(obj);
        }
    }

    public enum DetailFocusMode
    {
        Overview,
        Readme,
        Perks,
        Portraits,
        Powers,
        Items,
        Addons,
        Status,
        Offerings
    }

    public enum PerkSortBy
    {
        Name,
        Owner,
        Random
    }
}
