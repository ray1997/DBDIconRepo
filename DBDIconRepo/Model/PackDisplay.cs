using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DBDIconRepo.Dialog;
using DBDIconRepo.Helper;
using IconPack.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Messenger = CommunityToolkit.Mvvm.Messaging.WeakReferenceMessenger;

namespace DBDIconRepo.Model
{
    //Use for commands and parameter for bindings
    public class PackDisplay : ObservableObject
    {
        public PackDisplay(Pack _info)
        {
            Info = _info;

            InitializeCommand();
        }

        Pack? _base;
        public Pack? Info
        {
            get => _base;
            set => SetProperty(ref _base, value);
        }

        //Include images
        ObservableCollection<IDisplayItem>? _previewSauces;
        //Limit to just 4 items!
        public ObservableCollection<IDisplayItem>? PreviewSources
        {
            get => _previewSauces;
            set => SetProperty(ref _previewSauces, value);
        }

        //
        public ICommand? SearchForThisAuthor { get; private set; }
        public ICommand? InstallThisPack { get; private set; }
        public ICommand? OpenPackDetailWindow { get; private set; }

        private void InitializeCommand()
        {
            SearchForThisAuthor = new RelayCommand<RoutedEventArgs>(SearchForThisAuthorAction); 
            InstallThisPack = new RelayCommand<RoutedEventArgs>(InstallThisPackAction);
            OpenPackDetailWindow = new RelayCommand<RoutedEventArgs>(OpenPackDetailWindowAction);
        }

        private void OpenPackDetailWindowAction(RoutedEventArgs? obj)
        {
            Messenger.Default.Send(new RequestViewPackDetailMessage(Info), MessageToken.REQUESTVIEWPACKDETAIL);
        }

        private void InstallThisPackAction(RoutedEventArgs? obj)
        {
            //Show selection
            PackInstall install = new(Info);
            if (install.ShowDialog() == true)
            {
                //
                //var allInstallSummary = install.ViewModel.InstallableItems.Source.Select(i => i.IsSelected).Distinct();
                //if (allInstallSummary.Count() > 1) //There's true & false?
                //{
                //    //Partial download
                //}
                //else if (allInstallSummary.Count() == 1 && allInstallSummary.First() == true)
                //{
                //    //If all selection is true, just clone repo
                //    IsDownloading = true;
                //    Messenger.Default.Register<PackDisplay, DownloadRepoProgressReportMessage, string>(this,
                //        $"{MessageToken.REPOSITORYDOWNLOADREPORTTOKEN}{Info.Repository.Name}",
                //        HandleDownloadProgress);

                //    Messenger.Default.Send(new RequestDownloadRepo(Info), MessageToken.REQUESTDOWNLOADREPOTOKEN);
                //}
            }            
        }

        private void SearchForThisAuthorAction(RoutedEventArgs? obj)
        {
            Messenger.Default.Send(new RequestSearchQueryMessage(Info.Author), MessageToken.REQUESTSEARCHQUERYTOKEN);
        }

        bool _downloading;
        public bool IsDownloading
        {
            get => _downloading;
            set => SetProperty(ref _downloading, value);
        }

        double _totalProgress;
        public double TotalDownloadProgress
        {
            get => _totalProgress;
            set => SetProperty(ref _totalProgress, value);
        }

        private void HandleDownloadProgress(PackDisplay recipient, DownloadRepoProgressReportMessage message)
        {
            //Update progress
            /*Progress detail: Enumerating => 0-5%,
             * Compressing => 5-20%,
             * Transfering => 20-90%,
             * CheckingOut => 90-100%,
             * Done => 100%*/
            switch (message.CurrentState)
            {
                case DownloadState.Enumerating:
                    TotalDownloadProgress = Math.Round(message.EstimateProgress * 5d);
                    break;
                case DownloadState.Compressing:
                    TotalDownloadProgress = 5d + Math.Round(message.EstimateProgress * 15d);
                    break;
                case DownloadState.Transfering:
                    TotalDownloadProgress = 20d + Math.Round(message.EstimateProgress * 70d);
                    break;
                case DownloadState.CheckingOut:
                    TotalDownloadProgress = 90d + Math.Round(message.EstimateProgress * 10d);
                    break;
            }

            //Check if it's done
            if ((TotalDownloadProgress >= 99 && message.CurrentState <= DownloadState.CheckingOut) 
                || message.CurrentState == DownloadState.Done)
            {
                Messenger.Default.Unregister<DownloadRepoProgressReportMessage, string>(recipient, $"{MessageToken.REPOSITORYDOWNLOADREPORTTOKEN}{Info.Repository.Name}");
                TotalDownloadProgress = 100;
                IsDownloading = false;
            }
        }

        public void HandleURLs()
        {
            if (PreviewSources is null)
                PreviewSources = new ObservableCollection<IDisplayItem>();
            //Is this pack have banner?
            string path = CacheOrGit.GetDisplayContentPath(Info.Repository.Owner, Info.Repository.Name);
            if (File.Exists($"{path}\\.banner.png")) //Banner exist, link to it on github
                PreviewSources.Add(new BannerDisplay(URL.GetGithubRawContent(Info.Repository, ".banner.png")));
            else //banner not exist, get URLs for perk icons that required to display on setting
            {
                foreach (var icon in Setting.Instance.PerkPreviewSelection)
                {
                    if (Info.ContentInfo.Files.FirstOrDefault(i => i.ToLower().Contains(icon.ToLower())) is string match)
                    {
                        //This pack have this exact icon
                        PreviewSources.Add(new IconDisplay(URL.GetIconAsGitRawContent(Info.Repository, match)));
                    }
                }
                if (PreviewSources.Count < 1)
                {
                    //No content match the preview? Pick 4 random!
                    //If the pack has less than 10 icons? Don't try to random or it's gonna stuck too long on while loop!
                    string[] allPngs = Info.ContentInfo.Files.Where(i => i.EndsWith(".png")).ToArray();
                    if (allPngs.Length <= 10)
                    {
                        //Show all that its has or first four
                        PreviewSources = new ObservableCollection<IDisplayItem>(
                            Info.ContentInfo.Files.Select(file => new IconDisplay(URL.GetIconAsGitRawContent(Info.Repository, file))));
                        return;
                    }
                    List<int> AllRandomNumbers = new List<int>();
                    Random randomizer = new Random();
                    while (AllRandomNumbers.Count < 5)
                    {
                        int randomNumber = randomizer.Next(0, allPngs.Length);
                        if (!AllRandomNumbers.Contains(randomNumber))
                            AllRandomNumbers.Add(randomNumber);
                    }
                    foreach (var random in AllRandomNumbers)
                    {
                        PreviewSources.Add(new IconDisplay(URL.GetIconAsGitRawContent(Info.Repository, allPngs[random])));
                    }
                }
            }
        }

    }

    public interface IDisplayItem
    {
        string URL { get; set; }
    }

    public class OnlineSourceDisplay : ObservableObject, IDisplayItem
    {
        public OnlineSourceDisplay() { }

        public OnlineSourceDisplay(string url) { URL = url; }

        string? _url;
        public string? URL
        {
            get => _url;
            set => SetProperty(ref _url, value);
        }
    }

    public class IconDisplay : OnlineSourceDisplay 
    { 
        public IconDisplay() { }
        public IconDisplay(string url) : base(url) { }
    }

    public class BannerDisplay : OnlineSourceDisplay
    {
        public BannerDisplay() { }
        public BannerDisplay(string url) : base(url) { }
    }
}
