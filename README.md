# SMBBM AssetBundleLoader

A BepInEx plugin for Super Monkey Ball Banana Mania designed to allow patching AssetBundles without changing base game files.

Notes:

1. Currently, any patches disable the leaderboards. Some patches (like UI ones) may not disable it in the future
2. This mod currently only supports patching whole AssetBundles, not indiviual prefabs/etc within an AssetBundle

## Intallation

### Installing BepInEx

This plugin uses [BepInEx](https://github.com/BepInEx/BepInEx) as a mod loader so that needs to be installed first.

1. Download a bleeding edge build of "BepInEx Unity IL2CPP for Windows x64 games" [here](https://builds.bepinex.dev/projects/bepinex_be) (Only the bleeding edge builds support Il2CPP games which is what Banana Mania is
2. Extract it in your game folder)

## Install Plugin Dependencies

1. Install [SMBBM Leaderboard Disabler](https://github.com/bobjrsenior/SMBBMLeaderboardDisabler/releases)

## Installing This Plugin

1. Download the Plugin from the Releases page
2. Extract the downloaded zip file
3. Put the SMBBMAssetBundleLoader folder from the zip contents in your BepInEx/plugins folder

## Plugin Contents

Here is what the plugin resources should look like:

```
├── SMBBMAssetBundleLoader
│   ├── Newtonsoft.Json.dll
│   ├── Newtonsoft.Json.LICENSE.md
│   └── SMBBMAssetBundleLoader.dll
```

## Using The Plugin

This Plugin uses JSON files to configure what AssetBundles should be patched. The Plugin looks for these JSON files under the "UserData/AssetBundles" directory within your games install folder.

It is good practice to name your configuration file after you custom asset pack.

For example, if you wanted a custom asset pack called "MyAwesomeAssets", you could make your configuration file "MyAwesomeAssets.json". Additionally, please place your custom AssetBundles within a folder under "UserData/AssetBundles". For the "MyAwesomeAssets" example, it would be a folder called "UserData/AssetBundlesMyAwesomeAssets".

## JSON Configuration

The JSON configuration file is what determines the AssetBundles to replace. The Plugin supports having multiple configurations so you can have multiple Asset Packs installed at once (although having multiple expansive packs could override each other).

The configuration file format looks like this

```json
{
    "name": "My Awesome Assets",
    "description": "This asset pack adds all my awesome assets to the game!",
    "author": "bobjrsenior",
    "file_format_version": 1,
    "asset_bundles": {
        "in_game_asset_bundle_name_1": "my_custom_asset_bundle_name_1",
        "in_game_asset_bundle_name_2": "my_custom_asset_bundle_name_2",
        "in_game_asset_bundle_name_3": "my_custom_asset_bundle_name_3"
    }
}
```

Going into more detail, here are descriptions for each field in the configuration:

| Field               | Description                                             |
| :-------------------| :------------------------------------------------------ |
| name                | The name of your Asset Pack (informational only)        |
| description         | The description of your Asset Pack (informational only) |
| author              | The author of your Asset Pack (informational only)      |
| file_format_version | Always 1. This will be used if the configuration format is changed in the future for possible backwards compatability |
| asset_bundles       | A dictionary of key value pairs. Each Key should be the name of an existing SMBBM AssetBundle (ex: "stage_smb1_bonus_st1091"). Each value should be the relative path to your replacement bundle. Example: If your replacement bundle for "stage_smb1_bonus_st1091" was "UserData/AssetBundles/MyAwesomeAssets/my_custom_asset_bundle_name_1", you would put `"stage_smb1_bonus_st1091": "MyAwesomeAssets/my_custom_asset_bundle_name_1"` for one of the entries |

To test the mod and create an example of a larger asset pack, I created a configuration file for iswimfly's [Invisiball mod](https://gamebanana.com/mods/367723) [here](https://gist.github.com/bobjrsenior/68975daaef0737fa2e5a39289d747c60).

The configuration should go in the "UserData\AssetBundles" folder. Then copy the "Invisiball BM" folder from Invisiball's release to "UserData\AssetBundles" as well. When you start the game, the mod will then patch in all of the mods custom bundles.


## Building

## Setup

I use Visual Studio 2022  for development although I beleive it can also be compiled via command line. Additionally, make sure you setup your enviroment for BepInEx plugin development: https://docs.bepinex.dev/master/articles/dev_guide/plugin_tutorial/1_setup.html

## Configuration

In the .csproj, there is an element called `<SMBBMDir>`. You should edit this to point to your game installation. The project references are determined based on that. You will need to have run your game at least once with BepInEx installed for the references to be populated on disk.

## Post-build event

The project includes Post-build events that will automatically copy the plugin into "$(SMBBMDir)\BepInEx\plugins". That way you can immediately run the game after building is complete for testing.