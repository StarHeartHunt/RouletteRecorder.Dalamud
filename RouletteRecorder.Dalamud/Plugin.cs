using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Lumina.Excel.Sheets;
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

    public static Configuration Configuration { get; private set; } = null!;
    public static Localization Localization { get; private set; } = null!;

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

        PluginInterface.UiBuilder.Draw += DrawUi;
        PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUi;
        PluginInterface.UiBuilder.OpenMainUi += ToggleMainUi;
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

        PluginInterface.UiBuilder.Draw -= DrawUi;
        PluginInterface.UiBuilder.OpenConfigUi -= ToggleConfigUi;
        PluginInterface.UiBuilder.OpenMainUi -= ToggleMainUi;
    }

    private void OnLogin()
    {
        // TODO reconnect roulette recover ui
    }

    private static void OnLogout(int type, int code)
    {
        if (Roulette.Instance != null && Roulette.Instance.RouletteType != null && !Roulette.Instance.IsCompleted)
        {
            Database.SavePendingRoulette();
        }
    }

    private static void OnTerritoryChanged(ushort territoryId)
    {
        var currentContent = DataManager.GetExcelSheet<TerritoryType>().GetRow(territoryId).ContentFinderCondition.ValueNullable;
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

            if (Roulette.Instance.RouletteType != null) Roulette.Instance.Finish();
        }
    }

    private static unsafe void OnCfPop(ContentFinderCondition condition)
    {
        string? rouletteType = null;

        var queueInfo = ContentsFinder.Instance()->QueueInfo;
        var poppedContentType = queueInfo.PoppedQueueEntry.ContentType;
        var poppedContentId = queueInfo.PoppedQueueEntry.ConditionId;

        if (poppedContentType == ContentsId.ContentsType.Roulette)
        {
            var currentRoulette = DataManager.GetExcelSheet<Lumina.Excel.Sheets.ContentRoulette>().GetRow(poppedContentId);
            rouletteType = currentRoulette.Name.ToString();
        }

        Roulette.Init(null, rouletteType);

        PluginLog.Debug(
            $"[OnCfPop] PoppedContentType: {poppedContentType}, PoppedContentId: {poppedContentId}, rouletteName: {rouletteType}"
        );
    }

    private static void OnDutyCompleted(object? sender, ushort territoryId)
    {
        PluginLog.Debug($"[OnDutyCompleted] {territoryId}");
        if (Roulette.Instance == null) return;

        Roulette.Instance.IsCompleted = true;
        if (Roulette.Instance.RouletteType != null)
        {
            Roulette.Instance.Finish();
        }
    }

    private void OnCommand(string command, string args)
    {
        // in response to the slash command, just toggle the display status of our main ui
        ToggleMainUi();
    }

    private void DrawUi() => WindowSystem.Draw();
    public void ToggleConfigUi() => ConfigWindow.Toggle();
    public void ToggleMainUi() => MainWindow.Toggle();

    public static string? GetJobName() => ClientState.LocalPlayer?.ClassJob.ValueNullable?.Name.ToString();
    public static uint? GetJobId() => ClientState.LocalPlayer?.ClassJob.ValueNullable?.RowId;
}
