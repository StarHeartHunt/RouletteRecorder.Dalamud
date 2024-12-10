using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.GeneratedSheets;
using RouletteRecorder.Dalamud.DAO;
using RouletteRecorder.Dalamud.Utils;
using RouletteRecorder.Dalamud.Windows;

namespace RouletteRecorder.Dalamud;

public sealed class Plugin : IDalamudPlugin
{
    [PluginService] internal static IChatGui ChatGui { get; private set; } = null!;
    [PluginService] internal static IClientState ClientState { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] internal static IDataManager DataManager { get; private set; } = null!;
    [PluginService] internal static IDutyState DutyState { get; private set; } = null!;
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static IPluginLog PluginLog { get; private set; } = null!;

    private const string CommandName = "/prr";

    public Configuration Configuration { get; init; }
    public Localization Localization { get; init; }

    public readonly WindowSystem WindowSystem = new("RouletteRecorder");
    private ConfigWindow ConfigWindow { get; init; }
    private MainWindow MainWindow { get; init; }

    public Plugin()
    {
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        Localization = new Localization(Configuration.Language);
        Database.Load();

        ConfigWindow = new ConfigWindow(this);
        MainWindow = new MainWindow(this);

        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(MainWindow);

        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "Open plugin GUI"
        });

        ClientState.CfPop += OnCfPop;
        ClientState.TerritoryChanged += OnTerritoryChanged;
        ClientState.Logout += OnLogout;

        DutyState.DutyCompleted += OnDutyCompleted;

        PluginInterface.UiBuilder.Draw += DrawUI;
        PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUI;
        PluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;
    }

    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();

        ConfigWindow.Dispose();
        MainWindow.Dispose();

        CommandManager.RemoveHandler(CommandName);

        ClientState.CfPop -= OnCfPop;
        ClientState.TerritoryChanged -= OnTerritoryChanged;
        ClientState.Logout -= OnLogout;

        DutyState.DutyCompleted -= OnDutyCompleted;

        PluginInterface.UiBuilder.Draw -= DrawUI;
        PluginInterface.UiBuilder.OpenConfigUi -= ToggleConfigUI;
        PluginInterface.UiBuilder.OpenMainUi -= ToggleMainUI;
    }

    private void OnLogin()
    {
        // TODO reconnect roulette recover ui
    }

    private void OnLogout()
    {
        if (Roulette.Instance != null && Roulette.Instance.RouletteType != null && !Roulette.Instance.IsCompleted)
        {
            Database.SavePendingRoulette();
        }
    }

    private void OnTerritoryChanged(ushort territoryId)
    {
        var currentContent = DataManager.GetExcelSheet<TerritoryType>()?.GetRow(territoryId)?.ContentFinderCondition?.Value;
        PluginLog.Debug($"[OnTerritoryChanged] currentContent: {currentContent}");

        if (Roulette.Instance == null)
            Roulette.Init();

        // entered the duty territory
        if (Roulette.Instance!.ContentName == null)
        {
            Roulette.Instance.ContentName = currentContent?.Name.ToString();
        }
        else
        {
            PluginLog.Debug("[OnTerritoryChanged] detected exited roulette, force to finish");

            if (Roulette.Instance.RouletteType != null) Roulette.Instance.Finish(Configuration.SubscribedRouletteIds);
        }
    }

    private unsafe void OnCfPop(ContentFinderCondition condition)
    {
        string? rouletteType = null;

        var queueInfo = ContentsFinder.Instance()->QueueInfo;
        if (queueInfo.PoppedContentType == ContentsFinderQueueInfo.PoppedContentTypes.Roulette)
        {
            var currentRoulette = DataManager.GetExcelSheet<ContentRoulette>()?.GetRow(queueInfo.PoppedContentId);
            rouletteType = currentRoulette?.Name.ToString();
        }

        Roulette.Init(null, rouletteType);

        PluginLog.Debug(
            $"[OnCfPop] PoppedContentType: {queueInfo.PoppedContentType}, PoppedContentId: {queueInfo.PoppedContentId}, rouletteName: {rouletteType}"
        );
    }

    private void OnDutyCompleted(object? sender, ushort territoryId)
    {
        PluginLog.Debug($"[OnDutyCompleted] {territoryId}");
        if (Roulette.Instance == null) return;

        Roulette.Instance.IsCompleted = true;
        if (Roulette.Instance.RouletteType != null)
        {
            Roulette.Instance.Finish(Configuration.SubscribedRouletteIds);
        }
    }

    private void OnCommand(string command, string args)
    {
        // in response to the slash command, just toggle the display status of our main ui
        ToggleMainUI();
    }

    private void DrawUI() => WindowSystem.Draw();
    public void ToggleConfigUI() => ConfigWindow.Toggle();
    public void ToggleMainUI() => MainWindow.Toggle();
    public static string? GetJobName() => ClientState.LocalPlayer?.ClassJob?.GameData?.Name.ToString();
    public static uint? GetJobId() => ClientState.LocalPlayer?.ClassJob?.Id;
}
