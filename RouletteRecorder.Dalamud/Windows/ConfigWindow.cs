using Dalamud.Interface.Windowing;
using ImGuiNET;
using RouletteRecorder.Dalamud.Utils;
using System;
using System.Numerics;

namespace RouletteRecorder.Dalamud.Windows;

public sealed class ConfigWindow : Window, IDisposable
{
    private readonly Configuration configuration;
    private readonly Localization localization;

    public ConfigWindow(Plugin plugin) : base("Config###rouletteRecorderConfigWindow")
    {
        Flags = ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse;

        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 425),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        configuration = plugin.Configuration;
        localization = plugin.Localization;
    }

    public void Dispose() { }

    public override void PreDraw()
    {
        // Flags must be added or removed before Draw() is being called, or they won't apply
        if (configuration.IsConfigWindowMovable)
        {
            Flags &= ~ImGuiWindowFlags.NoMove;
        }
        else
        {
            Flags |= ImGuiWindowFlags.NoMove;
        }
    }

    public override void Draw()
    {
        var movable = configuration.IsConfigWindowMovable;
        if (ImGui.Checkbox(localization.Localize("Allow Drag Config Window"), ref movable))
        {
            configuration.IsConfigWindowMovable = movable;
            configuration.Save();
        }


        if (ImGui.CollapsingHeader(localization.Localize("Subscribed Roulette Types")))
        {
            ImGui.Indent();
            foreach (var roulette in Database.CfRoulettes)
            {
                var selected = configuration.SubscribedRouletteIds.Contains(roulette.RowId);
                if (ImGui.Checkbox(roulette.Name.ToString(), ref selected))
                {
                    configuration.SetSubscribedRouletteId(roulette, selected);
                    configuration.Save();
                }
            }
            ImGui.Unindent();
        }

    }
}
