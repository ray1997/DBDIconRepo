using CommunityToolkit.Mvvm.ComponentModel;
using DBDIconRepo.Model;
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
        ObservableCollection<Pack> _packs;
        public ObservableCollection<Pack> AllAvailablePack
        {
            get => _packs;
            set => SetProperty(ref _packs, value);
        }


        #region GIT
        public Octokit.GitHubClient client;
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
            var request = new SearchRepositoriesRequest($"topic:{PackTag}");
            var result = await client.Search.SearchRepo(request);
            foreach (var repo in result.Items)
            {
                var repoToPack = new Pack()
                {
                    Name = repo.Name,
                    Description = repo.Description,
                    Author = repo.Owner.Name,
                    URL = repo.Url,
                    LastUpdate = repo.UpdatedAt.DateTime
                };
                AllAvailablePack.Add(repoToPack);
            }
        }

        #endregion
    }
}
