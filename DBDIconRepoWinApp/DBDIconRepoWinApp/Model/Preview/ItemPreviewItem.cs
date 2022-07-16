using IconPack.Model;
using System;
using System.Linq;
using Info = IconPack.Helper.Info;

namespace DBDIconRepo.Model.Preview
{
    public class ItemPreviewItem : BasePreviewItem
    {
        public ItemPreviewItem(string path, PackRepositoryInfo repo) : base(path, repo)
        {
            //string perkName = path.Split("/\\".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Last().Replace(".png", "");
            //Perk = Info.Perks.ContainsKey(perkName) ? Info.Perks[perkName] as PerkInfo : null;
            string item = path.Split("/\\".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Last().Replace(".png", "");
            if (Info.Items.ContainsKey(item))
                Name = Info.Items[item];
        }
    }
}
