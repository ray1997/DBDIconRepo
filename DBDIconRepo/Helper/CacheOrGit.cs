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

namespace DBDIconRepo.Helper
{
    public static class CacheOrGit
    {
        private const string CachedFolderName = "Cache";

        public static async Task<Pack?> GetPack(GitHubClient client, Repository repo)
        {
            //Find from cached folder first
            string repoCachedFolder = $"{Environment.CurrentDirectory}\\{CachedFolderName}\\{repo.Name}";

            string packJsonCached = $"{repoCachedFolder}\\{Terms.PackJson}";
            if (!Directory.Exists(repoCachedFolder))
            {
                Directory.CreateDirectory(repoCachedFolder);
            }
            if (!File.Exists(packJsonCached))
            {
                //Get from git
                byte[] jsonRaw = await client.Repository.Content.GetRawContent(repo.Owner.Login, repo.Name, Terms.PackJson);
                //Then write to file as UTF-8
                string json = Encoding.UTF8.GetString(jsonRaw);
                using (StreamWriter writer = File.CreateText(packJsonCached))
                {
                    writer.Write(json);
                }

                return JsonSerializer.Deserialize<Pack>(json);
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

        //TODO:Add support for pack banner
        public static async Task GatherPackPreviewImage(GitHubClient client, Repository repo, Pack? requested, string[] requestedPreview)
        {
            string repoCachedFolder = $"{Environment.CurrentDirectory}\\{CachedFolderName}\\{repo.Name}";
            if (!Directory.Exists(repoCachedFolder))
                Directory.CreateDirectory(repoCachedFolder);

            string cachedPerkDirectory = $"{repoCachedFolder}\\Perks";
            if (!Directory.Exists(cachedPerkDirectory))
                Directory.CreateDirectory(cachedPerkDirectory);

            foreach (var item in requestedPreview)
            {
                //Do something with pack without perk
                if (!requested.ContentInfo.HasPerks)
                    return;

                string pathToFile = $"{cachedPerkDirectory}\\{item}.png";
                if (!File.Exists(pathToFile))
                {
                    //Load from git and cached it
                    byte[] rawImage = await client.Repository.Content.GetRawContent(repo.Owner.Login, repo.Name, $"/Perks/{item}.png");

                    using (FileStream fs = new FileStream(path: pathToFile, mode: System.IO.FileMode.Create, FileAccess.Write))
                    {
                        fs.Write(rawImage, 0, rawImage.Length);
                    }
                }
            }

            //TODO:Deal with a pack that didn't have these perks
            //TODO:Deal with pack override perk display
            //TODO:Deal with pack with less than 4 perks
            //TODO:Deal with other pack type (Portrait, etc)
        }
    }
}
