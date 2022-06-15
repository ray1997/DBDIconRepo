namespace IconPack.Model
{
    public class KillerPowerInfo : IBaseItemInfo
    {
        public KillerPowerInfo(string? name, string? owner)
        {
            Name = name;
            if (owner is not null)
                PowerOwner = owner;
        }

        public string? Name { get; set; }
        public string? PowerOwner { get; set; }
    }
}