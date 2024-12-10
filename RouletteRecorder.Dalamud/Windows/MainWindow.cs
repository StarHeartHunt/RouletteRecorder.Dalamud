using Dalamud.Interface.Windowing;
using ImGuiNET;
using RouletteRecorder.Dalamud.DAO;
using System;
using System.Numerics;

namespace RouletteRecorder.Dalamud.Windows;

public sealed class MainWindow : Window, IDisposable
{
    private readonly Plugin plugin;

    public MainWindow(Plugin plugin)
        : base("RouletteRecorder###rouletteRecorderMainWindow", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        this.plugin = plugin;
    }

    public void Dispose() { }

    public override void Draw()
    {
        ImGui.Text("Current Roulette Properties");
        ImGui.Separator();

        ImGui.BulletText($"RouletteType: {Roulette.Instance?.RouletteType?.ToString() ?? "null"}");
        ImGui.BulletText($"Date: {Roulette.Instance?.Date.ToString() ?? "null"}");
        ImGui.BulletText($"StartedAt: {Roulette.Instance?.StartedAt.ToString() ?? "null"}");
        ImGui.BulletText($"EndedAt: {Roulette.Instance?.EndedAt?.ToString() ?? "null"}");
        ImGui.BulletText($"IsCompleted: {Roulette.Instance?.IsCompleted.ToString() ?? "null"}");
        ImGui.BulletText($"ContentName: {Roulette.Instance?.ContentName?.ToString() ?? "null"}");
        ImGui.BulletText($"JobName: {Roulette.Instance?.JobName?.ToString() ?? "null"}");


        if (ImGui.Button("Show Settings"))
        {
            plugin.ToggleConfigUI();
        }
        ImGui.SameLine();
        if (ImGui.Button("Export as CSV"))
        {
            Plugin.PluginLog.Debug("export as CSV");
        }
    }
}
