namespace IconPack.Model
{
    public class PerkInfo : IBaseItemInfo
    {
        public PerkInfo(string? name, string? owner)
        {
            Name = name;
            if (owner is not null)
                PerkOwner = owner;
        }

        public string? Name { get; set; }

        public string? PerkOwner { get; init; }
    }
}