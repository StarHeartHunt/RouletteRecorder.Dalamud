using Dalamud.Interface.Windowing;
using ImGuiNET;
using RouletteRecorder.Dalamud.DAO;
using RouletteRecorder.Dalamud.Utils;
using System;
using System.Numerics;
using System.Threading.Tasks;

namespace RouletteRecorder.Dalamud.Windows;

public sealed class MainWindow : Window, IDisposable
{
    private readonly Plugin plugin;
    private readonly Localization localization;

    public MainWindow(Plugin plugin)
        : base("RouletteRecorder###rouletteRecorderMainWindow", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        this.plugin = plugin;
        localization = plugin.Localization;
    }

    public void Dispose() { }

    public override void Draw()
    {
        ImGui.Text(localization.Localize("Current Roulette Properties"));
        ImGui.Separator();

        ImGui.BulletText(PrintProperty("RouletteType: {0}", Roulette.Instance?.RouletteType));
        ImGui.BulletText(PrintProperty("Date: {0}", Roulette.Instance?.Date));
        ImGui.BulletText(PrintProperty("StartedAt: {0}", Roulette.Instance?.StartedAt));
        ImGui.BulletText(PrintProperty("EndedAt: {0}", Roulette.Instance?.EndedAt));
        ImGui.BulletText(PrintProperty("IsCompleted: {0}", Roulette.Instance?.IsCompleted.ToString()));
        ImGui.BulletText(PrintProperty("ContentName: {0}", Roulette.Instance?.ContentName));
        ImGui.BulletText(PrintProperty("JobName: {0}", Roulette.Instance?.JobName));

        if (ImGui.Button(localization.Localize("Show Settings")))
        {
            plugin.ToggleConfigUI();
        }
        ImGui.SameLine();
        if (ImGui.Button(localization.Localize("Export as CSV")))
        {
            Task.Run(() => Database.ExportAsCsv(plugin.Configuration.CsvExportPath));
        }
    }

    public string PrintProperty(string messageTemplate, string? value)
    {
        return string.Format(localization.Localize(messageTemplate), value ?? "null");
    }
}
