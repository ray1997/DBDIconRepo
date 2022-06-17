using CommunityToolkit.Mvvm.ComponentModel;
using DBDIconRepo.Helper;
using IconPack.Model;
using System;
using System.Linq;
using Info = IconPack.Helper.Info;

namespace DBDIconRepo.Model.Preview
{
    public class PortraitPreviewItem : BasePreviewItem
    {
        public PortraitPreviewItem(string path, PackRepositoryInfo repo) : base(path, repo)
        {
            string name = path.Split("/\\".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Last().Replace(".png", "");
            if (Info.Portraits.ContainsKey(name))
            {
                Name = Info.Portraits[name];
            }
        }
    }
}
