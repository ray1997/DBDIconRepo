using CommunityToolkit.Mvvm.ComponentModel;
using DBDIconRepo.Helper;
using IconPack.Model;
using System;

namespace DBDIconRepo.Model.Preview
{
    public class BasePreviewItem : ObservableObject, IBaseItemInfo
    {
        string? _name;
        public string? Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public BasePreviewItem(string path, PackRepositoryInfo repo)
        {
            IconURL = URL.GetIconAsGitRawContent(repo, path);
        }

        string? _url;
        public string? IconURL
        {
            get => _url;
            set => SetProperty(ref _url, value);
        }

        public Uri? IconUri
        {
            get
            {
                if (IconURL is null)
                    return new Uri("");
                return new Uri(IconURL, UriKind.Absolute);
            }
        }
    }
}
