using IconPack.Model;
using Info = IconPack.Helper.Info;

namespace DBDIconRepo.Model.Preview
{
    public class AddonPreviewItem : BasePreviewItem
    {
        public new string? Name
        {
            get
            {
                if (AddOns is null)
                    return null;
                return AddOns.Name;
            }
        }

        public AddOnsInfo? AddOns { get; set; }

        public AddonPreviewItem(string path, PackRepositoryInfo repo) : base(path, repo)
        {
            if (Info.GetItemAddonsInfo(path) is AddOnsInfo info)
            {
                AddOns = info;
            }
        }
    }
}
