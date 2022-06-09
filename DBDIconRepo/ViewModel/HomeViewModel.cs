using CommunityToolkit.Mvvm.ComponentModel;
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

namespace DBDIconRepo.ViewModel
{
    public class HomeViewModel : ObservableObject
    {
        public async Task InitializeViewModel()
        {
            InitializeGit();

            if (AllAvailablePack is null)
                AllAvailablePack = new ObservableCollection<PackDisplay>();
            await FindPack();
        }

        ObservableCollection<PackDisplay>? _packs;
        public ObservableCollection<PackDisplay> AllAvailablePack
        {
            get => _packs;
            set => SetProperty(ref _packs, value);
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
    }
}
