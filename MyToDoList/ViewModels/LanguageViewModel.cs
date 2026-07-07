using CommunityToolkit.Mvvm.ComponentModel;
using MyToDoList.Resources;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System;
using System.Linq;

namespace MyToDoList.ViewModels
{
    public partial class LanguageViewModel : ViewModelBase
    {
        private const string SettingsFolder = "MyToDoList";
        private const string LanguageFileName = "language.txt";
        public ObservableCollection<LanguageItem> AvailableLanguages { get; } = new()
        {
            new LanguageItem { Code = "en", Name = "English" },
            new LanguageItem { Code = "ru", Name = "Русский" }
        };

        [ObservableProperty]
        private LanguageItem? _selectedLanguage;

        public event Action? LanguageChanged;

        public LanguageViewModel()
        {
            LoadSavedLanguage();
        }
       
        partial void OnSelectedLanguageChanged(LanguageItem? value)
        {
            if (value == null) return;
            Strings.Culture = new CultureInfo(value.Code);
            SaveLanguagePreference(value.Code);
            LanguageChanged?.Invoke();
        }

        private void SaveLanguagePreference(string code)
        {
            try
            {
                var path = GetSettingsPath();
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                File.WriteAllText(path, code);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не получилось сохранить! {ex.Message}");
            }
        }

        private void LoadSavedLanguage()
        {
            try
            {
                var path = GetSettingsPath();
                if (File.Exists(path))
                {
                    var savedCode = File.ReadAllText(path).Trim();
                    var savedLang = AvailableLanguages.FirstOrDefault(l => l.Code == savedCode);
                    if (savedLang != null)
                    {
                        SelectedLanguage = savedLang;
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не получилось загрузить! {ex.Message}");
            }
        }

        private static string GetSettingsPath()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                SettingsFolder,
                LanguageFileName);
        }
    }

    public class LanguageItem
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public override string ToString() => Name;
    }
}
