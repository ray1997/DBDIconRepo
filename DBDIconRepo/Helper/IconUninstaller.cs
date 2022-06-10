using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBDIconRepo.Helper
{
    public static class IconUninstaller
    {
        static string[] ignoreDirectory = new string[]
        {
            "Banners",
            "NewContentSplash"
        };

        static string[] ignoreList = new string[]
        {
            "empty.png",
            "Missing.png",
            "Anniversary_Hidden.png",
            "hidden.png",
            "categoryIcon_outfits_lg.png"
        };
        internal static void Uninstall(string dbdPath)
        {
            DirectoryInfo info = new DirectoryInfo($"{dbdPath}\\DeadByDaylight\\Content\\UI\\Icons");
            var files = info.GetFiles("*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                if (ignoreList.Contains(file.Name))
                    continue;
                if (ignoreDirectory.Contains(file.Directory.Name))
                    continue;

                file.Delete();
            }
        }
    }
}
