using IconPack.Model;
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
