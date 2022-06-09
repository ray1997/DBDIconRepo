using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DBDIconRepo.Model
{
    public class Setting : ObservableObject
    {
        string _dbdPath = "";
        public string DBDInstallationPath
        {
            get => _dbdPath;
            set => SetProperty(ref _dbdPath, value);
        }

        bool _useUncuratedContent = true;
        //TODO:Eventually load content only from specific repo pointed to other packs
        public bool UseUncuratedContent
        {
            get => _useUncuratedContent;
            set => SetProperty(ref _useUncuratedContent, value);
        }

        ObservableCollection<string> _selectedPreview = new()
        {
            "iconPerks_spineChill",
            "iconPerks_adrenaline",
            "iconPerks_sloppyButcher",
            "iconPerks_lightborn"
        };
        public ObservableCollection<string> PerkPreviewSelection
        {
            get => _selectedPreview;
            set => SetProperty(ref _selectedPreview, value);
        }

        [JsonIgnore]
        private const string SettingFilename = "settings.json";
        public static void SaveSettings(Setting instance)
        {
            //Replace existing
            string settingFilePath = $"{Environment.CurrentDirectory}\\{SettingFilename}";
            if (File.Exists(settingFilePath))
            {
                File.Delete(settingFilePath);
            }

            string setting = JsonSerializer.Serialize(instance, new JsonSerializerOptions()
            {
                WriteIndented = true,
                IncludeFields = false
            });

            //Write to file
            using (StreamWriter writer = new(File.Create(settingFilePath), Encoding.UTF8))
            {
                writer.Write(setting);
            }
        }

        public static Setting? LoadSettings()
        {
            string settingFilePath = $"{Environment.CurrentDirectory}\\{SettingFilename}";

            if (!File.Exists(settingFilePath))
            {
                return null;
            }

            using (StreamReader reader = File.OpenText(settingFilePath))
            {
                return JsonSerializer.Deserialize<Setting>(reader.ReadToEnd());
            }
        }

        [JsonIgnore]
        private static Setting? _instance;
        public static Setting? Instance
        {
            get
            {
                if (_instance is null)
                {                    
                    _instance = LoadSettings();
                    if (_instance is null)
                        _instance = new Setting();
                }
                return _instance;
            }
        }
    }
}
