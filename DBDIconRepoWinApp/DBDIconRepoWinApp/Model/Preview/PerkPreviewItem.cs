using IconPack.Model;
using System;
using System.Linq;
using Info = IconPack.Helper.Info;

namespace DBDIconRepo.Model.Preview
{
    public class PerkPreviewItem : BasePreviewItem
    {
        public new string? Name
        {
            get
            {
                if (Perk is null)
                    return null;
                return Perk.Name;
            }
        }

        public PerkInfo? Perk { get; set; }

        public PerkPreviewItem(string path, PackRepositoryInfo repo) : base(path, repo)
        {
            string perkName = path.Split("/\\".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Last().Replace(".png", "");
            Perk = Info.Perks.ContainsKey(perkName) ? Info.Perks[perkName] as PerkInfo : null;
        }
    }
}
