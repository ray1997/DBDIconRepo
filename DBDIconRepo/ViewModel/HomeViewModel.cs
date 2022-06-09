using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DBDIconRepo.Helper;
using DBDIconRepo.Model;
using IconPack.Model;
using Octokit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Messenger = CommunityToolkit.Mvvm.Messaging.WeakReferenceMessenger;

namespace DBDIconRepo.ViewModel
{
    public class HomeViewModel : ObservableObject
    {
        public void InitializeViewModel()
        {
            //Initialize commands
            InitializeCommands();

            //Monitor settings
            Config.PropertyChanging += UpdateWhenChanged;

            InitializeGit();

            if (AllAvailablePack is null)
                AllAvailablePack = new ObservableCollection<PackDisplay>();
            FindPack().Await(() =>
            {
                //Filters
                ApplyFilter();
                //Monitor settings
                Messenger.Default.Register<HomeViewModel, FilterOptionChangedMessage, string>(this, MessageToken.FILTEROPTIONSCHANGETOKEN, HandleFilterOptionChanged);
            });
        }
        private void HandleFilterOptionChanged(HomeViewModel recipient, FilterOptionChangedMessage message)
        {
            OnPropertyChanged(nameof(FilteredList));
        }

        public void UnregisterMessages()
        {
            Messenger.Default.Unregister<FilterOptionChangedMessage, string>(this, MessageToken.FILTEROPTIONSCHANGETOKEN);
        }

        private void ApplyFilter()
        {
            OnPropertyChanged(nameof(FilteredList));
        }

        private void UpdateWhenChanged(object? sender, System.ComponentModel.PropertyChangingEventArgs e)
        {
            Setting.SaveSettings(Config);
        }

        ObservableCollection<PackDisplay>? _packs;
        public ObservableCollection<PackDisplay> AllAvailablePack
        {
            get => _packs;
            set => SetProperty(ref _packs, value);
        }

        bool _isEmpty;
        public bool IsFilteredListEmpty
        {
            get => _isEmpty;
            set => SetProperty(ref _isEmpty, value);
        }

        public ObservableCollection<PackDisplay> FilteredList
        {
            get
            {
                if (AllAvailablePack is null)
                    return new ObservableCollection<PackDisplay>();
                var newList = new List<PackDisplay>();

                //Filter by perks
                var perks = AllAvailablePack.Where(x => x.Info.ContentInfo.HasPerks);
                if (Config.FilterOptions.HasPerks)
                    newList = newList.Union(perks).ToList();

                //Filter by add-ons
                var addons = AllAvailablePack.Where(x => x.Info.ContentInfo.HasAddons);
                if (Config.FilterOptions.HasAddons)
                    newList = newList.Union(addons).ToList();

                //Filter by items
                var items = AllAvailablePack.Where(x => x.Info.ContentInfo.HasItems);
                if (Config.FilterOptions.HasItems)
                    newList = newList.Union(items).ToList();

                //Filter by offerings
                var offerings = AllAvailablePack.Where(x => x.Info.ContentInfo.HasOfferings);
                if (Config.FilterOptions.HasOfferings)
                    newList = newList.Union(offerings).ToList();

                //Filter by powers
                var powers = AllAvailablePack.Where(x => x.Info.ContentInfo.HasPowers);
                if (Config.FilterOptions.HasPowers)
                    newList = newList.Union(powers).ToList();

                //Filter by status
                var status = AllAvailablePack.Where(x => x.Info.ContentInfo.HasStatus);
                if (Config.FilterOptions.HasStatus)
                    newList = newList.Union(status).ToList();

                //Filter by portraits
                var portraits = AllAvailablePack.Where(x => x.Info.ContentInfo.HasPortraits);
                if (Config.FilterOptions.HasPortraits)
                    newList = newList.Union(portraits).ToList();

                IsFilteredListEmpty = newList.Count == 0;
                return new ObservableCollection<PackDisplay>(newList);
            }
        }

        public Setting? Config => Setting.Instance;

        #region GIT
        public Octokit.GitHubClient? client;
        private string token = "";

        public void InitializeGit()
        {
            client = new GitHubClient(new ProductHeaderValue("ballz"));
            if (string.IsNullOrEmpty(token))
            {
                string tokenFile = $"{Environment.CurrentDirectory}\\token.txt";
                if (File.Exists(tokenFile))
                {
                    token = File.ReadAllText(tokenFile);
                }
            }
            if (!string.IsNullOrEmpty(token))
            {
                var tokenAuth = new Credentials(token);
                client.Credentials = tokenAuth;
            }
        }

        private const string PackTag = "dbd-icon-pack";
        public async Task FindPack()
        {
            //TODO:Somewhere before this, search if all result already cached
            var request = new SearchRepositoriesRequest($"topic:{PackTag}");
            var result = await client.Search.SearchRepo(request);
            foreach (var repo in result.Items)
            {
                PackDisplay? gatheredPack = new PackDisplay()
                {
                    Info = await CacheOrGit.GetPack(client, repo),
                    Owner = repo.Owner.Login,
                    Repository = repo.Name
                };

                if (gatheredPack is null)
                    continue;
                //Preview icons
                await CacheOrGit.GatherPackPreviewImage(client, repo, gatheredPack.Info, Config.PerkPreviewSelection.ToArray());
                AllAvailablePack.Add(gatheredPack);
            }
        }

        #endregion

        #region Commands
        public ICommand SetFilterOnlyPerks { get; private set; }
        public ICommand SetFilterOnlyPortraits { get; private set; }
        public ICommand SetFilterShowAll { get; private set; }

        private void InitializeCommands()
        {
            SetFilterOnlyPerks = new RelayCommand<RoutedEventArgs>(SetFilterOnlyPerksAction);
            SetFilterOnlyPortraits = new RelayCommand<RoutedEventArgs>(SetFilterOnlyPortraitsAction);
            SetFilterShowAll = new RelayCommand<RoutedEventArgs>(SetFilterCompletePackAction);
        }

        private void SetFilterOnlyPerksAction(RoutedEventArgs? obj)
        {
            Config.FilterOptions.HasPerks = true;
            Config.FilterOptions.HasOfferings =
                Config.FilterOptions.HasStatus =
                Config.FilterOptions.HasPowers =
                Config.FilterOptions.HasPortraits =
                Config.FilterOptions.HasAddons = false;
            OnPropertyChanged(nameof(FilteredList));
        }

        private void SetFilterOnlyPortraitsAction(RoutedEventArgs? obj)
        {
            Config.FilterOptions.HasPortraits = true;
            Config.FilterOptions.HasOfferings =
                Config.FilterOptions.HasStatus =
                Config.FilterOptions.HasPowers =
                Config.FilterOptions.HasPerks =
                Config.FilterOptions.HasAddons = false;
            OnPropertyChanged(nameof(FilteredList));
        }

        private void SetFilterCompletePackAction(RoutedEventArgs? obj)
        {
            Config.FilterOptions.HasOfferings =
                Config.FilterOptions.HasStatus =
                Config.FilterOptions.HasPowers =
                Config.FilterOptions.HasPortraits =
                Config.FilterOptions.HasAddons =
                Config.FilterOptions.HasPerks = true;
            OnPropertyChanged(nameof(FilteredList));
        }
        #endregion
    }
}
