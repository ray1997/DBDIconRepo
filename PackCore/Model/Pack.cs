using CommunityToolkit.Mvvm.ComponentModel;
using IconPack.Helper;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace IconPack.Model
{
    public class Pack : ObservableObject
    {
        string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        string? _desc;
        public string? Description
        {
            get => _desc;
            set => SetProperty(ref _desc, value);
        }

        string? _author;
        public string? Author
        {
            get
            {
                if (_author is null)
                    return "";
                return _author;
            }

            set => SetProperty(ref _author, value);
        }

        string? _url;
        public string? URL
        {
            get => _url;
            set => SetProperty(ref _url, value);
        }

        DateTime _lastUpdate;
        [JsonConverter(typeof(CustomDateTimeFormat))]
        public DateTime LastUpdate
        {
            get => _lastUpdate;
            set => SetProperty(ref _lastUpdate, value);
        }

        PackContentInfo _content;
        public PackContentInfo ContentInfo
        {
            get => _content;
            set => SetProperty(ref _content, value);
        }
    }

    public class PackContentInfo : ObservableObject
    {
        bool _perks;
        public bool HasPerks
        {
            get => _perks;
            set => SetProperty(ref _perks, value);
        }

        bool _portraits;
        public bool HasPortraits
        {
            get => _portraits;
            set => SetProperty(ref _portraits, value);
        }

        bool _powers;
        public bool HasPowers
        {
            get => _powers;
            set => SetProperty(ref _powers, value);
        }

        bool _item;
        public bool HasItems
        {
            get => _item;
            set => SetProperty(ref _item, value);
        }

        bool _status;
        public bool HasStatus
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        bool _offerings;
        public bool HasOfferings
        {
            get => _offerings;
            set => SetProperty(ref _offerings, value);
        }

        bool _addons;
        public bool HasAddons
        {
            get => _addons;
            set => SetProperty(ref _addons, value);
        }

        ObservableCollection<string>? _files;
        public ObservableCollection<string> Files
        {
            get => _files;
            set => SetProperty(ref _files, value);
        }
    }
}
