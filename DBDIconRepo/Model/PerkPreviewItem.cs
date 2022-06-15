using CommunityToolkit.Mvvm.ComponentModel;
using DBDIconRepo.Helper;
using IconPack.Model;
using System;
using System.Linq;
using Info = IconPack.Helper.Info;

namespace DBDIconRepo.Model
{
    public class PerkPreviewItem : ObservableObject
    {
        public PerkInfo? Perk { get; set; }

        public PerkPreviewItem(string path, PackRepositoryInfo repo)
        {
            string perkName = "";
            if (path.Contains('/') || path.Contains('\\'))
                perkName = path.Split("/\\".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Last();
            if (perkName.EndsWith(".png"))
                perkName = perkName.Replace(".png", "");
            Perk = Info.Perks.ContainsKey(perkName) ? Info.Perks[perkName] as PerkInfo : null;
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
