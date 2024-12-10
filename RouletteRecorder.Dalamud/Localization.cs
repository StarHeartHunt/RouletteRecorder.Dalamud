using Dalamud.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace RouletteRecorder.Dalamud
{
    public class Localization
    {
        private readonly string currentLanguage;
        private Dictionary<string, string> languageDict = [];
        internal Localization(string currentLanguage)
        {
            this.currentLanguage = currentLanguage;
            LoadLanguage(currentLanguage);
        }

        internal string Localize(string message)
        {
            if (currentLanguage == "en") return message;
            if (languageDict.TryGetValue(message, out var value))
            {
                return value;
            }
#if DEBUG
            else
            {
                languageDict.Add(message, message);
                var locDirectory = Plugin.PluginInterface.GetPluginLocDirectory();
                if (!Directory.Exists(locDirectory)) Directory.CreateDirectory(locDirectory);
                File.WriteAllText(Path.Combine(locDirectory, "zh_CN.json"), JsonConvert.SerializeObject(languageDict));
            }
#endif

            return message;
        }

        internal void LoadLanguage(string language)
        {
            if (language == "en") return;

            try
            {
                string? json = null;

                var locDirectory = Plugin.PluginInterface.GetPluginLocDirectory();
                if (locDirectory.IsNullOrWhitespace()) return;
                if (!Directory.Exists(locDirectory)) Directory.CreateDirectory(locDirectory);

                var langFile = Path.Combine(locDirectory, "zh_CN.json");
                using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"RouletteRecorder.Dalamud.Resources.zh_CN.json");
                if (stream != null)
                {
                    using var streamReader = new StreamReader(stream);
                    json = streamReader.ReadToEnd();
                    File.WriteAllText(langFile, json);
                }

                if (!json.IsNullOrWhitespace())
                {
                    languageDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json) ??
                        throw new Exception("[LoadLanguage] failed to deserialize lang file");
                }
            }
            catch (Exception e)
            {
                Plugin.PluginLog.Error(e, "[LoadLanguage] Error occurred when loading language file");
            }
        }

        internal static string[] GetLanguages()
        {
            string[] languages = { "en", "zh_CN" };
            return languages;
        }
    }
}
