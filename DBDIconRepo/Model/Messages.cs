using IconPack.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBDIconRepo.Model
{
    public class FilterOptionChangedMessage
    {
        public string KeyChanged { get; private set; }
        public FilterOptions Changed { get; private set; }

        public FilterOptionChangedMessage(string key, FilterOptions value)
        {
            KeyChanged = key;
            Changed = value;
        }
    }

    public class SettingChangedMessage
    {
        public string? PropertyName { get; private set; }
        public object? Value { get; private set; }

        public SettingChangedMessage(string? property, object? value)
        {
            PropertyName = property;
            Value = value;
        }
    }

    public class RequestSearchQueryMessage
    {
        public string? Query { get; private set; }

        public RequestSearchQueryMessage(string? _required)
        {
            Query = _required;
        }
    }

    public class RequestDownloadRepo
    {
        public Pack Info { get; private set; }

        public RequestDownloadRepo(Pack _pack)
        {
            Info = _pack;
        }
    }

    public class CloningInProgressMessage
    {

    }
}
