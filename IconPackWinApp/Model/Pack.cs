using CommunityToolkit.Mvvm.ComponentModel;
using IconPack.Helper;
using Octokit;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IconPack.Model
{
    public class Pack : ObservableObject
    {
        string? _name;
        public string? Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        string? _desc;
        public string? Description
        {
            get => _desc;
            set => SetProperty(ref _desc, value);
        }

        string? _author;
        public string? Author
        {
            get => _author;
            set => SetProperty(ref _author, value);
        }

        string? _url;
        public string? URL
        {
            get => _url;
            set => SetProperty(ref _url, value);
        }

        DateTime _lastUpdate;
        [JsonConverter(typeof(CustomDateTimeFormat))]
        public DateTime LastUpdate
        {
            get => _lastUpdate;
            set => SetProperty(ref _lastUpdate, value);
        }

        PackRepositoryInfo? _repInfo;
        public PackRepositoryInfo? Repository
        {
            get => _repInfo;
            set => SetProperty(ref _repInfo, value);
        }

        PackContentInfo? _content;
        public PackContentInfo? ContentInfo
        {
            get => _content;
            set => SetProperty(ref _content, value);
        }
    }

    public class PackRepositoryInfo : ObservableObject
    {
        public PackRepositoryInfo() { }
        public PackRepositoryInfo(Repository repo)
        {
            ID = repo.Id;
            Name = repo.Name;
            Owner = repo.Owner.Login;
            DefaultBranch = repo.DefaultBranch;
            CloneUrl = repo.CloneUrl;
        }

        string? _repoName;
        public string? Name
        {
            get => _repoName;
            set => SetProperty(ref _repoName, value);
        }

        long _id;
        public long ID
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        string? _ownerLogin;
        public string? Owner
        {
            get => _ownerLogin;
            set => SetProperty(ref _ownerLogin, value);
        }

        string? _defaultBranch;
        public string? DefaultBranch
        {
            get => _defaultBranch;
            set => SetProperty(ref _defaultBranch, value);
        }

        string? _cloneUrl;
        public string? CloneUrl
        {
            get => _cloneUrl;
            set => SetProperty(ref _cloneUrl, value);
        }
    }

    public class PackContentInfo : ObservableObject
    {
        public PackContentInfo() { }

        public static async Task<PackContentInfo> GetContentInfo(GitHubClient client, Repository repo)
        {
            PackContentInfo info = new PackContentInfo();
            var commits = await client.Repository.Commit.GetAll(repo.Owner.Login, repo.Name);
            var head = commits.First();
            var treeContent = await client.Git.Tree.GetRecursive(repo.Id, head.Sha);
            var treeOnly = treeContent.Tree.Where(i => i.Type.Value == TreeType.Tree);

            info.HasAddons = treeOnly.Any(content => content.Path == "ItemAddons");
            info.HasItems = treeOnly.Any(content => content.Path =="items");
            info.HasOfferings = treeOnly.Any(content => content.Path =="Favors");
            info.HasPerks = treeOnly.Any(content => content.Path =="Perks");
            info.HasPortraits = treeOnly.Any(content => content.Path =="CharPortraits");
            info.HasPowers = treeOnly.Any(content => content.Path =="Powers");
            info.HasStatus = treeOnly.Any(content => content.Path =="StatusEffects");

            info.Files = new ObservableCollection<string>(treeContent.Tree.Where(i => i.Type.Value == TreeType.Blob).Where(i => i.Path.EndsWith(".png")).Select(i => i.Path));
            return info;
        }

        bool _perks;
        public bool HasPerks
        {
            get => _perks;
            set => SetProperty(ref _perks, value);
        }

        bool _portraits;
        public bool HasPortraits
        {
            get => _portraits;
            set => SetProperty(ref _portraits, value);
        }

        bool _powers;
        public bool HasPowers
        {
            get => _powers;
            set => SetProperty(ref _powers, value);
        }

        bool _item;
        public bool HasItems
        {
            get => _item;
            set => SetProperty(ref _item, value);
        }

        bool _status;
        public bool HasStatus
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        bool _offerings;
        public bool HasOfferings
        {
            get => _offerings;
            set => SetProperty(ref _offerings, value);
        }

        bool _addons;
        public bool HasAddons
        {
            get => _addons;
            set => SetProperty(ref _addons, value);
        }

        ObservableCollection<string>? _files;
        public ObservableCollection<string>? Files
        {
            get => _files;
            set => SetProperty(ref _files, value);
        }
    }
}
