# RouletteRecorder.Dalamud

## How To Use

### Prerequisites

RouletteRecorder.Dalamud assumes all the following prerequisites are met:

-   XIVLauncher, FINAL FANTASY XIV, and Dalamud have all been installed and the game has been run with Dalamud at least once.
-   XIVLauncher is installed to its default directories and configurations.
    -   If a custom path is required for Dalamud's dev directory, it must be set with the `DALAMUD_HOME` environment variable.
-   A .NET Core 8 SDK has been installed and configured, or is otherwise available. (In most cases, the IDE will take care of this.)

### Building

1. Open up `RouletteRecorder.Dalamud.sln` in your C# editor of choice (likely [Visual Studio 2022](https://visualstudio.microsoft.com) or [JetBrains Rider](https://www.jetbrains.com/rider/)).
2. Build the solution. By default, this will build a `Debug` build, but you can switch to `Release` in your IDE.
3. The resulting plugin can be found at `RouletteRecorder.Dalamud/bin/x64/Debug/RouletteRecorder.Dalamud.dll` (or `Release` if appropriate.)

### Activating in-game

1. Launch the game and use `/xlsettings` in chat or `xlsettings` in the Dalamud Console to open up the Dalamud settings.
    - In here, go to `Experimental`, and add the full path to the `RouletteRecorder.Dalamud.dll` to the list of Dev Plugin Locations.
2. Next, use `/xlplugins` (chat) or `xlplugins` (console) to open up the Plugin Installer.
    - In here, go to `Dev Tools > Installed Dev Plugins`, and the `RouletteRecorder.Dalamud` should be visible. Enable it.
3. You should now be able to use `/pmycommand` (chat) or `pmycommand` (console)!

Note that you only need to add it to the Dev Plugin Locations once (Step 1); it is preserved afterwards. You can disable, enable, or load your plugin on startup through the Plugin Installer.

### Reconfiguring for your own uses

Basically, just replace all references to `RouletteRecorder.Dalamud` in all of the files and filenames with your desired name, then start building the plugin of your dreams. You'll figure it out 😁

Dalamud will load the JSON file (by default, `RouletteRecorder.Dalamud/RouletteRecorder.Dalamud.json`) next to your DLL and use it for metadata, including the description for your plugin in the Plugin Installer. Make sure to update this with information relevant to _your_ plugin!
