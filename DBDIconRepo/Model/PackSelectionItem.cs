using CommunityToolkit.Mvvm.ComponentModel;
using IconPack.Model;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBDIconRepo.Model
{
    public interface IPackSelectionItem
    {
        string? Name { get; set; }
        bool? IsSelected { get; set; }
        bool? IsExpanded { get; set; }
        IBaseItemInfo? Info { get; set; }
        IPackSelectionItem? Parent { get; }
    }

    public class PackSelectionFolder : ObservableObject, IPackSelectionItem
    {
        string? _name;
        /// <summary>
        /// Folder root name
        /// </summary>
        public string? Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public bool? IsExpanded
        {
            get => true;
            set { }
        }

        public bool? IsSelected
        {
            get
            {
                if (Childs is null)
                    return null;
                var collectives = Childs.Select(i => i.IsSelected).Distinct();
                int amount = collectives.Count();
                if (amount == 0)
                    return null; //It's empty
                if (amount > 1) //There's true & false
                    return null; //Indetermine state
                return collectives.First(); //They are all on 1 state
            }
            set
            {
                if (value is null)
                    return;
                if (Childs?.Count < 1)
                    //No child?
                    return;
                //Set all child to the same state as required
                for (int count = 0; count < Childs.Count; count++)
                {
                    Childs[count].IsSelected = value.Value;
                }
            }
        }

        IBaseItemInfo? _info;
        public IBaseItemInfo? Info
        {
            get => _info;
            set => SetProperty(ref _info, value);
        }

        ObservableCollection<IPackSelectionItem>? _childs;
        public ObservableCollection<IPackSelectionItem>? Childs
        {
            get => _childs;
            set => SetProperty(ref _childs, value);
        }

        IPackSelectionItem? _parent;
        public IPackSelectionItem? Parent
        {
            get
            {
                return _parent;
            }
            private set
            {
                _parent = value;
            }
        }

        public PackSelectionFolder(IPackSelectionItem? parent = null)
        {
            Childs = new ObservableCollection<IPackSelectionItem>();
            Info = null;
            Parent = parent ?? parent;
        }
    }

    public class PackSelectionFile : ObservableObject, IPackSelectionItem
    {
        string? _name;
        /// <summary>
        /// Filename
        /// </summary>
        public string? Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        bool? _selected;
        public bool? IsSelected
        {
            get => _selected;
            set => SetProperty(ref _selected, value);
        }

        public bool? IsExpanded
        {
            get => false;
            set { }
        }

        IBaseItemInfo? _info;
        public IBaseItemInfo? Info
        {
            get => _info;
            set => SetProperty(ref _info, value);
        }

        IPackSelectionItem? _parent;
        public IPackSelectionItem? Parent
        {
            get => _parent;
            private set => _parent = value;
        }

        public PackSelectionFile(IPackSelectionItem? parent)
        {
            IsSelected = true;
            Parent = parent ?? parent;
        }
    }
}
