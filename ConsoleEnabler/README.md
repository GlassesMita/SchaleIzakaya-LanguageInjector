# Console Enabler

A BepInEx plugin to enable the EA Debug Console in Touhou Mystia Izakaya.

## Features

- Enables the EA Debug Console (PrototypingManagers.EADebugConsole)
- Allows access to game debug functions and parameter adjustment
- Normally disabled in release builds

## Configuration

The plugin creates a configuration file at:
`BepInEx/config/com.consoleenabler.cfg`

### Settings

| Setting | Default | Description |
|---------|----------|-------------|
| EnableDebugConsole | true | Enable the EA Debug Console |
| ShowConsoleOnStartup | false | Show console immediately on game startup |

## How It Works

1. The plugin uses reflection to find the `PrototypingManagers.EADebugConsole` class
2. It modifies the following fields to enable the console:
   - `showConsoleText` = "1" (show console text field)
   - `hideConsoleText` = "0" (hide console text field)
   - `shouldOnGUIBuffConsoleShown` = true (enable GUI display)
   - `newGameMode` = false (disable new game mode)
3. These changes are applied when the game starts

## Installation

1. Build the project: `dotnet build`
2. The DLL will be copied to: `F:\SteamLibrary\steamapps\common\Touhou Mystia Izakaya\BepInEx\plugins\`
3. Restart the game

## Usage

After installation, the debug console will be enabled automatically when the game starts.

To disable the console, edit the config file and set `EnableDebugConsole = false`.

## Requirements

- BepInEx 6.0.0 or later
- Touhou Mystia Izakaya game
- .NET 6.0 SDK

## Development

Built with:
- BepInEx.Unity.IL2CPP 6.0.0-be.*
- HarmonyX 2.*
- dnSpy analysis of game assemblies

## Credits

Created using dnSpy and IDA analysis of the game's internal console system.
