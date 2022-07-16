using IconPack.Model;
using System;
using Info = IconPack.Helper.Info;

namespace DBDIconRepo.Model.Preview
{
    public class EmblemPreviewItem : BasePreviewItem
    {
        public EmblemType Type { get; set; }

        public EmblemPreviewItem(string path, PackRepositoryInfo repo) : base(path, repo)
        {
            if (Info.Emblems.ContainsKey(path))
            {
                string[] split = Info.Emblems[path].Split(" ()".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (split.Length > 0)
                {
                    Name = split[0];
                    Type = Enum.Parse<EmblemType>(split[1]);
                }
            }
        }
    }

    public enum EmblemType
    {
        None,
        Silver,
        Bronze,
        Gold,
        Iridescent
    }
}
