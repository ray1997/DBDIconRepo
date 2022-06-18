using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PackInfo = IconPack.Helper.Info;

namespace DBDIconRepo.Model
{
    public class PackSelectionItem : ObservableObject
    {
        string? _file;
        public string? FileName
        {
            get => _file;
            set => SetProperty(ref _file, value);
        }

        IconPack.Model.IBaseItemInfo? _item;
        public IconPack.Model.IBaseItemInfo? Info
        {
            get => _item;
            set => SetProperty(ref _item, value);
        }

        string? _name;
        public string? Name
        {
            get
            {
                if (Info is null)
                    return _name;
                return Info.Name;
            }
            set
            {
                SetProperty(ref _name, value);
            }
        }

        string? _folder;
        public string? FolderName
        {
            get => _folder;
            set => SetProperty(ref _folder, value);
        }

        string? _subFolder;
        public string? SubFolderName
        {
            get => _subFolder;
            set => SetProperty(ref _subFolder, value);
        }

        bool _selected = true;
        public bool IsSelected
        {
            get => _selected;
            set => SetProperty(ref _selected, value);
        }

        public PackSelectionItem(string path)
        {
            //Check for file name that still do start of file as "\Perks\Kate\iconPerks_boilOver.png" 
            //Expect result: Perks\Kate\iconPerks_boilOver.png
            if (path.StartsWith("/") || path.StartsWith("\\"))
                path = path[1..];
            //Keep a copy of original path
            FileName = path;

            //Check if previous result has .png at the end, if so. Remove it
            //Expect result: Perks\Kate\iconPerks_boilOver
            if (path.EndsWith(".png"))
                path = path.Replace(".png", "");

            //Check if previous result use \ as path separator
            //(windows path separator which won't work with URL)
            //Expect result: Perks/Kate/iconPerks_boilOver
            if (path.Contains('\\'))
                path = path.Replace('\\', '/');

            //Substring to only the first folder of its
            //Expect result for "FolderName": Perks
            FolderName = path[..path.IndexOf('/')];
            //Remove the folder bits
            //Expect result: Kate/iconPerks_boilOver.png
            if (path.Contains('/'))
                path = path[(path.IndexOf('/') + 1)..];
            //For sorting
            SubFolderName = path.Contains('/') ? path[..(path.LastIndexOf('/'))] : null;
            //Remove every pathing 
            if (path.Contains("/"))
                path = path[(path.LastIndexOf('/') + 1)..];

            switch (FolderName)
            {
                case "CharPortraits": Name = PackInfo.Portraits.ContainsKey(path) ? PackInfo.Portraits[path]         : null; break;
                case "DailyRituals":  Name = PackInfo.DailyRituals.ContainsKey(path) ? PackInfo.DailyRituals[path]   : null; break;
                case "Emblems":       Name = PackInfo.Emblems.ContainsKey(path) ? PackInfo.Emblems[path]             : null; break;
                case "Favors":        Name = PackInfo.Offerings.ContainsKey(FileName.Replace(".png", "")) ? PackInfo.Offerings[FileName.Replace(".png", "")] : null; break;
                case "ItemAddons":    Info = PackInfo.GetItemAddonsInfo(FileName);                                           break;
                case "items":         Name = PackInfo.Items.ContainsKey(path) ? PackInfo.Items[path]                 : null; break;
                case "Perks":
                    Info = PackInfo.Perks.ContainsKey(path) ? PackInfo.Perks[path] : null;
                    if (Info is null)
                        System.Diagnostics.Debug.WriteLine(FileName);
                    break;
                case "Powers":        Info = PackInfo.Powers.ContainsKey(path) ? PackInfo.Powers[path]               : null; break;
                case "StatusEffects": Name = PackInfo.StatusEffects.ContainsKey(path) ? PackInfo.StatusEffects[path] : null; break;
                default: Name = null; break;
            }
        }
    }
}
