using CommunityToolkit.Mvvm.ComponentModel;
using DBDIconRepo.Helper;
using IconPack.Model;
using System;
using System.Linq;
using Info = IconPack.Helper.Info;

namespace DBDIconRepo.Model.Preview
{
    public class PowerPreviewItem : BasePreviewItem
    {
        public new string? Name
        {
            get 
            { 
                if (PowerInfo is null)
                    return null;
                return PowerInfo.Name;
            }
        }

        public KillerPowerInfo? PowerInfo { get; set; }

        public PowerPreviewItem(string path, PackRepositoryInfo repo) : base(path, repo)
        {
            string power = path.Split("/\\".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Last().Replace(".png", "");
            PowerInfo = Info.Powers.ContainsKey(power) ? Info.Powers[power] as KillerPowerInfo : null;
        }
    }
}
