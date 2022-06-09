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
}
