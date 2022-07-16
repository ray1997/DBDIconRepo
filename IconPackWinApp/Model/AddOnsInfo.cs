namespace IconPack.Model
{
    public class AddOnsInfo : IBaseItemInfo
    {
        public AddOnsInfo(string? name, string? forItem)
        {
            Name = name;
            For = forItem;
            Owner = null;
        }

        public AddOnsInfo(string? name, string? forPower, string? owner, bool isDecommissioned = false)
        {
            Name = name;
            For = forPower;
            Owner = owner;
            IsDecommissioned = isDecommissioned;
        }

        public string? Name { get; set; }
        public string? For { get; set; }
        public string? Owner { get; set; }

        public bool IsDecommissioned { get; set; } = false;
    }
}