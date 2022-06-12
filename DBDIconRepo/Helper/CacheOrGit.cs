using DBDIconRepo.Model;
using IconPack.Model;
using IconPack.Resource;
using Octokit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Messenger = CommunityToolkit.Mvvm.Messaging.WeakReferenceMessenger;

namespace DBDIconRepo.Helper
{
    public static class CacheOrGit
    {
        private const string CachedFolderName = "Cache";
        private const string CachedDisplayName = "Display";

        public static async Task<Pack?> GetPack(GitHubClient client, Repository repo)
        {
            //Directory
            string cachedRoot = GetDisplayContentPath(repo.Owner.Login, repo.Name);

            //Local pack.json info
            string packJsonCached = $"{cachedRoot}\\{Terms.PackJson}";
            if (!File.Exists(packJsonCached))
            {
                byte[] jsonRaw = new byte[] { };
                string json = "";
                //Get from git
                //Check if this repo actually have pack.json
                bool hasPackJson = await URL.IsContentExists(repo, Terms.PackJson);
                if (hasPackJson)
                {
                    try
                    {
                        jsonRaw = await client.Repository.Content.GetRawContent(repo.Owner.Login, repo.Name, Terms.PackJson);
                        //Then write to file as UTF-8
                        json = Encoding.UTF8.GetString(jsonRaw);
                    }
                    catch { }
                }
                //Try get pack.json on project
                
                //Fill in other missing details
                var packInfo = string.IsNullOrEmpty(json) ?
                    new Pack() :
                    JsonSerializer.Deserialize<Pack>(json);
                packInfo.Name = packInfo.Name ?? repo.Name;
                packInfo.Description = packInfo.Description ?? repo.Description;
                packInfo.Author = packInfo.Author ?? repo.Owner.Name ?? repo.Owner.Login;
                packInfo.URL = packInfo.URL ?? repo.HtmlUrl;
                packInfo.LastUpdate = repo.UpdatedAt.UtcDateTime;

                if (packInfo.Repository is null) { packInfo.Repository = new PackRepositoryInfo(repo); }
                if (packInfo.ContentInfo is null)
                {
                    packInfo.ContentInfo = await PackContentInfo.GetContentInfo(client, repo).ConfigureAwait(true);
                }

                //update
                json = JsonSerializer.Serialize(packInfo, new JsonSerializerOptions()
                {
                    WriteIndented = true,
                    IncludeFields = false
                });
                using (StreamWriter writer = File.CreateText(packJsonCached))
                {
                    writer.Write(json);
                }

                return packInfo;
            }
            else
            {
                //TODO:Consider here somewhere if file is too old, delete and get a new file instead

                //File has already cached
                using (var reader = File.OpenText(packJsonCached))
                {
                    string json = reader.ReadToEnd();

                    return JsonSerializer.Deserialize<Pack>(json);
                }
            }
        }

        public static async Task GatherPackDisplayData(GitHubClient client, Repository repo, Pack? requested, string[] requestedPreview)
        {
            //Directory
            string cachedRoot = GetDisplayContentPath(repo.Owner.Login, repo.Name);

            string readmePath = $"{cachedRoot}\\readme.md";
            string noReadmeMarker = $"{cachedRoot}\\NOREADME";

            //Readme.md
            if (!File.Exists(readmePath) && !File.Exists(noReadmeMarker))
            {
                //Check if this repo have readme.md
                bool readmeState = await URL.IsContentExists(repo, "readme.md");
                //if so, get it
                if (readmeState)
                {
                    //Get readme.md
                    var readme = await client.Repository.Content.GetReadme(repo.Id);
                    File.WriteAllText(readmePath, readme.Content);
                }
                else //otherwise, leave this empty file
                {
                    File.Create(noReadmeMarker);
                }
            }

            string bannerPath = $"{cachedRoot}\\.banner.png";
            string noBannerMarker = $"{cachedRoot}\\NOBANNER";

            //Banner existance
            if (!File.Exists($"{bannerPath}") && !File.Exists($"{noBannerMarker}")) 
            {
                //Get banner if it's exist
                bool bannerState = await URL.IsContentExists(repo, ".banner.png");
                if (bannerState)
                {
                    //Get banner
                    //var banner = await client.Repository.Content.GetRawContent(repo.Owner.Login, repo.Name, ".banner.png");
                    //using FileStream fs = new(path: $"{cachedRoot}\\.banner.png", mode: System.IO.FileMode.Create, FileAccess.Write);
                    //fs.Write(banner, 0, banner.Length);
                    //Make dummy banner to let "PackDisplay" know, it's exist on repo
                    File.Create(bannerPath);
                }
                else
                    File.Create(noBannerMarker);
            }
        }

        public static string GetDisplayContentPath(string owner, string name)
        {
            string path = $"{Environment.CurrentDirectory}\\{CachedFolderName}\\{CachedDisplayName}\\" +
                $"{owner}\\{name}";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }

        //TODO:Add support for pack banner
        //    //TODO:Deal with a pack that didn't have these perks
        //    //TODO:Deal with pack override perk display
        //    //TODO:Deal with pack with less than 4 perks
        //    //TODO:Deal with other pack type (Portrait, etc)

        public static async Task DownloadPack(GitHubClient client, Repository repo, Pack pack)
        {
            string repoCachedFolder = $"{Environment.CurrentDirectory}\\{CachedFolderName}\\{repo.Owner.Login}\\{repo.Name}";

            if (!Directory.Exists(repoCachedFolder))
            {
                Directory.CreateDirectory(repoCachedFolder);
            }

            var user = await client.User.Current();
            var token = client.Credentials.GetToken(); 
            
            //pull from remote
            LibGit2Sharp.Repository.Clone(pack.Repository.CloneUrl, repoCachedFolder, new LibGit2Sharp.CloneOptions()
            {
                CredentialsProvider = (_url, _user, _cred) => GetCredential(user.Login, token),
                OnCheckoutProgress = CheckoutProgressHandlerMethod,
                OnProgress = ProgressHandlerMethod,
                OnTransferProgress = ProgressTransferProgressHandlerMethod,
                IsBare = false
            });
        }

        private static LibGit2Sharp.UsernamePasswordCredentials GetCredential(string username, string token)
        {
            return new LibGit2Sharp.UsernamePasswordCredentials()
            {
                Username = username,
                Password = token
            };
        }

        private static bool ProgressTransferProgressHandlerMethod(LibGit2Sharp.TransferProgress progress)
        {
            System.Diagnostics.Debug.Print($"{nameof(ProgressTransferProgressHandlerMethod)}: \r\n" +
                $"{nameof(progress.IndexedObjects)}: {progress.IndexedObjects}\r\n" +
                $"{nameof(progress.TotalObjects)}: {progress.TotalObjects}\r\n" +
                $"{nameof(progress.ReceivedObjects)}: {progress.ReceivedObjects}\r\n" +
                $"{nameof(progress.ReceivedBytes)}: {progress.ReceivedBytes}");
            return true;
        }

        private static bool ProgressHandlerMethod(string serverProgressOutput)
        {
            System.Diagnostics.Debug.Print($"{nameof(ProgressHandlerMethod)}: {serverProgressOutput}");
            return true;
        }

        private static void CheckoutProgressHandlerMethod(string path, int completedSteps, int totalSteps)
        {
            System.Diagnostics.Debug.Print($"{nameof(CheckoutProgressHandlerMethod)}: {path} ({completedSteps}/{totalSteps})");
        }


    }
}
