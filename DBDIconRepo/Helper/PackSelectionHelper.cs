using DBDIconRepo.Model;
using IconPack.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PackInfo = IconPack.Helper.Info;

namespace DBDIconRepo.Helper
{
    public static class PackSelectionHelper
    {
        public static void RootWork(string path, ref ObservableCollection<IPackSelectionItem> collection)
        {
            if (path.Contains('\\'))
                path = path.Replace('\\', '/');
            List<string> paths = path.Split('/', StringSplitOptions.RemoveEmptyEntries).ToList();
            if (collection is null)
                collection = new ObservableCollection<IPackSelectionItem>();
            var item = collection.FirstOrDefault(item => item.Name == paths[0]);
            if (item is null)
            {
                string? folderName = PackInfo.GetMainFoldersName(paths[0]);
                collection.Add(new PackSelectionFolder()
                {
                    Name = paths[0],
                    Info = folderName == paths[0] ? null :
                    new GenericItemInfo(folderName),
                });
                var newItem = collection.FirstOrDefault(i => i.Name == paths[0]);
                int index = collection.IndexOf(newItem);
                collection[index] = Traverse(collection[index], paths.Skip(1).ToList());
            }
            else
            {
                int index = collection.IndexOf(item);
                collection[index] = Traverse(collection[index], paths.Skip(1).ToList());
            }
        }

        private static IPackSelectionItem Traverse(IPackSelectionItem? root, List<string> paths)
        {
            if (paths.Count < 1)
                return root;
            if (!paths[0].EndsWith(".png"))
            {
                //Folder
                //Check if it's already exist
                IPackSelectionItem item = (root as PackSelectionFolder).Childs.FirstOrDefault(i => i.Name == paths[0]);
                if (item is null)
                {
                    //Add new child
                    string? hasInfoName = PackInfo.GetSubfolderAsChapterName(root.Name, paths[0]);
                    (root as PackSelectionFolder).Childs.Add(new PackSelectionFolder(root)
                    {
                        Name = paths[0],
                        Info = hasInfoName is null ? null :
                        new GenericItemInfo(hasInfoName)
                    });
                    var newItem = (root as PackSelectionFolder).Childs.FirstOrDefault(i => i.Name == paths[0]);
                    int index = (root as PackSelectionFolder).Childs.IndexOf(newItem);
                    (root as PackSelectionFolder).Childs[index] = Traverse((root as PackSelectionFolder).Childs[index], paths.Skip(1).ToList());
                    return root;
                }
                else
                {
                    int index = (root as PackSelectionFolder).Childs.IndexOf(item);
                    (root as PackSelectionFolder).Childs[index] = Traverse((root as PackSelectionFolder).Childs[index], paths.Skip(1).ToList());
                    return root;
                }
            }
            else
            {
                //File
                (root as PackSelectionFolder).Childs.Add(new PackSelectionFile(root)
                {
                    Name = paths[0],
                    Info = GetInfo(root, paths[0]),
                    IsSelected = true
                });
                return root;
            }
            return root;
        }

        private static IBaseItemInfo? GetInfo(IPackSelectionItem root, string name)
        {
            List<string> roots = new();
            IPackSelectionItem? traverseBack = root;
            while (traverseBack.Parent != null)
            {
                roots.Add(traverseBack.Name);
                traverseBack = traverseBack.Parent;
            }
            if (traverseBack.Parent is null)
                roots.Add(traverseBack.Name);
            if (name.EndsWith(".png"))
                name = name.Replace(".png", "");
            switch (roots.LastOrDefault())
            {
                case "CharPortraits":
                    return !PackInfo.Portraits.ContainsKey(name) ? null :
                        new GenericItemInfo(PackInfo.Portraits[name]);
                case "Favors":
                    roots.Reverse();
                    roots.Add(name);
                    string fullpath = string.Join('/', roots);
                    return !PackInfo.Offerings.ContainsKey(fullpath) ? null :
                        new GenericItemInfo(PackInfo.Offerings[fullpath]);
                case "ItemAddons":
                    roots.Reverse();
                    return PackInfo.GetItemAddonsInfo(string.Join('/', roots));
                case "items":
                    return !PackInfo.Items.ContainsKey(name) ? null :
                        new GenericItemInfo(PackInfo.Items[name]);
                case "Perks": 
                    return !PackInfo.Perks.ContainsKey(name) ? null : 
                        PackInfo.Perks[name];
                case "Powers":
                    return !PackInfo.Powers.ContainsKey(name) ? null : 
                        PackInfo.Powers[name];
                case "StatusEffects":
                    return !PackInfo.StatusEffects.ContainsKey(name) ? null : 
                        new GenericItemInfo(PackInfo.StatusEffects[name]);
                default: return null;
            }
        }

        public static void Sort(ref ObservableCollection<IPackSelectionItem> collection)
        {
            //Sort root
            collection = new ObservableCollection<IPackSelectionItem>(collection.OrderBy(i => i.Name));

            //Sort childs
            for (int i = 0; i < collection.Count; i++)
            {
                if (collection[i] is PackSelectionFolder)
                {
                    collection[i] = SortChild(collection[i] as PackSelectionFolder);
                }
            }
        }

        private static IPackSelectionItem? SortChild(PackSelectionFolder root)
        {
            //Go as deep as possible
            for (int i = 0; i < root.Childs.Count; i++)
            {
                if (root.Childs[i] is PackSelectionFolder)
                {
                    root.Childs[i] = SortChild(root.Childs[i] as PackSelectionFolder);
                }
            }

            if (root.Name == "Perks" || (root.Parent != null && root.Parent.Name == "Perks"))
            {
                //Custom perk sort order?
                root.Childs = new ObservableCollection<IPackSelectionItem>(
                    root.Childs.OrderBy(i => i is not PackSelectionFolder).
                    ThenBy(i => PerkInfoKillerThenSurvivor(i)).
                    ThenBy(i => PerkOwnerNameOrNothing(i)).
                    ThenBy(i => PerkInfoNameOrNothing(i)));

                return root;
            }

            //Then sort from there
            root.Childs = new ObservableCollection<IPackSelectionItem>(
                root.Childs.OrderBy(i => i is not PackSelectionFolder).
                ThenBy(i => i.Name));
            return root;
        }

        private static object PerkInfoKillerThenSurvivor(IPackSelectionItem i)
        {
            if (i is PackSelectionFolder)
                return false;

            if (i.Info is null)
                return false;
            
            if (i.Info is not PerkInfo)
                return false;

            if ((i.Info as PerkInfo).PerkOwner is null)
                return false;

            if (i.Info is PerkInfo perk)
                return !perk.PerkOwner.StartsWith("The");

            return false;
        }

        private static string? PerkOwnerNameOrNothing(IPackSelectionItem i)
        {
            if (i is PackSelectionFolder)
                return (i as PackSelectionFolder).Name;

            if (i.Info is null)
                return "";
            
            if (i.Info is not PerkInfo)
                return "";

            if ((i.Info as PerkInfo).PerkOwner is null)
                return "";

            if (i.Info is PerkInfo perk)
                return perk.PerkOwner;

            return "";
        }

        private static string? PerkInfoNameOrNothing(IPackSelectionItem i)
        {
            if (i is PackSelectionFolder)
                return "";

            if (i.Info is PerkInfo)
                return i.Info.Name;
            return null;
        }
    }
}
