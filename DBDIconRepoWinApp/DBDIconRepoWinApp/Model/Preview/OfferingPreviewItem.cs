using IconPack.Model;
using Info = IconPack.Helper.Info;

namespace DBDIconRepo.Model.Preview
{
    public class OfferingPreviewItem : BasePreviewItem
    {
        public OfferingPreviewItem(string path, PackRepositoryInfo repo) : base(path, repo)
        {
            if (path.StartsWith("/") || path.StartsWith("\\"))
                path = path.Substring(1);
            if (path.Contains("\\"))
                path = path.Replace("\\", "/");
            if (path.EndsWith(".png"))
                path = path.Replace(".png", "");
            if (Info.Offerings.ContainsKey(path))
                Name = Info.Offerings[path];
        }
    }
}
