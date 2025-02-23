# RouletteRecorder.Dalamud

Auto record your daily roulettes including mentor roulettes, [RouletteRecorder](https://github.com/StarHeartHunt/RouletteRecorder) in Dalamud

## Installation

Add `https://raw.githubusercontent.com/StarHeartHunt/RouletteRecorder.Dalamud/refs/heads/master/repo.json` to your plugin repositories and then search for `RouletteRecorder` in the Plugin Installer to install RouletteRecorder.

Main UI can be accessed via the Plugin Installer or using the chat command `/prr`.

## Usage

1. Setup Subscribed Roulette Types
  After using the command `/prr`, click on `Show Settings` button, expand `Subscribed Roulette Types` and select Roulette Types to subscribe

2. Enjoy!

## Develop

### Prerequisites

RouletteRecorder.Dalamud assumes all the following prerequisites are met:

- XIVLauncher, FINAL FANTASY XIV, and Dalamud have all been installed and the game has been run with Dalamud at least once.
- XIVLauncher is installed to its default directories and configurations.
  - If a custom path is required for Dalamud's dev directory, it must be set with the `DALAMUD_HOME` environment variable.
- A .NET Core 8 SDK has been installed and configured, or is otherwise available. (In most cases, the IDE will take care of this.)

### Building

1. Open up `RouletteRecorder.Dalamud.sln` in your C# editor of choice (likely [Visual Studio 2022](https://visualstudio.microsoft.com) or [JetBrains Rider](https://www.jetbrains.com/rider/)).
2. Build the solution. By default, this will build a `Debug` build, but you can switch to `Release` in your IDE.
3. The resulting plugin can be found at `RouletteRecorder.Dalamud/bin/Debug/RouletteRecorder.Dalamud.dll` (or `Release` if appropriate.)

### Activating in-game

1. Launch the game and use `/xlsettings` in chat or `xlsettings` in the Dalamud Console to open up the Dalamud settings.
   - In here, go to `Experimental`, and add the full path to the `RouletteRecorder.Dalamud.dll` to the list of Dev Plugin Locations.
2. Next, use `/xlplugins` (chat) or `xlplugins` (console) to open up the Plugin Installer.
   - In here, go to `Dev Tools > Installed Dev Plugins`, and the `RouletteRecorder.Dalamud` should be visible. Enable it.
3. You should now be able to use `/prr` (chat) or `prr` (console)!

Note that you only need to add it to the Dev Plugin Locations once (Step 1); it is preserved afterwards. You can disable, enable, or load your plugin on startup through the Plugin Installer.
