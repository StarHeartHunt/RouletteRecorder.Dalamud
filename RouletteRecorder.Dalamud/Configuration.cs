using Dalamud.Configuration;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.IO;

namespace RouletteRecorder.Dalamud;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;
    public HashSet<uint> subscribedRouletteIds { get; set; } = [];
    public string CSVExportPath { get; set; } = Path.Combine(Plugin.PluginInterface.ConfigDirectory.FullName, "data.csv");
    public bool IsConfigWindowMovable { get; set; } = true;

    public bool SetSubscribedRouletteId(ContentRoulette roulette, bool selected)
    {
        return selected ? subscribedRouletteIds.Add(roulette.RowId) : subscribedRouletteIds.Remove(roulette.RowId);
    }

    public void Save()
    {
        Plugin.PluginInterface.SavePluginConfig(this);
    }
}
