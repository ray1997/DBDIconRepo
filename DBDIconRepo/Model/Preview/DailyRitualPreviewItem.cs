using CommunityToolkit.Mvvm.ComponentModel;
using DBDIconRepo.Helper;
using IconPack.Model;
using System;
using System.Linq;
using Info = IconPack.Helper.Info;

namespace DBDIconRepo.Model.Preview
{
    public class DailyRitualPreviewItem : BasePreviewItem
    {
        public DailyRitualPreviewItem(string path, PackRepositoryInfo repo) : base(path, repo)
        {
            if (Info.DailyRituals.ContainsKey(path))
                Name = Info.DailyRituals[path];
        }
    }
}
