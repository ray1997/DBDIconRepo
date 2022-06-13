using CommunityToolkit.Mvvm.ComponentModel;
using DBDIconRepo.Helper;
using DBDIconRepo.Model;
using IconPack.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DBDIconRepo.ViewModel
{
    public class PackDetailViewModel : ObservableObject
    {
        public PackDetailViewModel() { }
        public PackDetailViewModel(Pack? selected)
        {
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
            /*
            // using MdXaml;
            // using System.Windows.Documents;

            Markdown engine = new Markdown();

            string markdownTxt = System.IO.File.ReadAllText("example.md");

            FlowDocument document = engine.Transform(markdownTxt);*/
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

        Pack? _selected;
        public Pack? SelectedPack
        {
            get => _selected;
            set => SetProperty(ref _selected, value);
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
    }
}
