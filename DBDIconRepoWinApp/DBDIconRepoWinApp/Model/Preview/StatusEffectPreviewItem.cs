using IconPack.Model;
using System;
using System.Linq;
using Info = IconPack.Helper.Info;

namespace DBDIconRepo.Model.Preview
{
    public class StatusEffectPreviewItem : BasePreviewItem
    {
        public StatusEffectPreviewItem(string path, PackRepositoryInfo repo) : base(path, repo)
        {
            string status = path.Split("/\\".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Last().Replace(".png", "");
            if (Info.StatusEffects.ContainsKey(status))
                Name = Info.StatusEffects[status];
        }
    }
}
