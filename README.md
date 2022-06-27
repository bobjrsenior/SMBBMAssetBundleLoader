# SMBBM File Redirector

A plugin for Super Monkey Ball Banana Mania designed to allow redirecting game assets to custom ones without overwriting base game files.

This plugin supports BOTH Banana Mod Manager (BMM) and BepInEx.

Notes:

1. All AssetBundle patches disable the leaderboards. Some patches (like UI ones) may not disable it in the future
2. Audio and Video patches do not disable the leaderboards
3. This mod currently only supports redirecting whole AssetBundles, not indiviual prefabs/etc within an AssetBundle. There is technically experimental support included but don't expect it to work (the game will probably soft lock on a loading screen)
4. This mod does allow redirecting individual audio Cues within a Cue Sheet with more advanced setup

## Installing for Banana Mod Manager

1. Download the SMBBMFileRedirector.BMM.zip file from the Releases page
2. Extract it in your main game folder (the zip file structure should put the plugin in the right place)
3. Enable it within BMM

## Installing for BepInEx (Required if running through wine/proton)

### Installing BepInEx

This plugin uses [BepInEx](https://github.com/BepInEx/BepInEx) as a mod loader so that needs to be installed first.

1. Download a bleeding edge build of "BepInEx Unity IL2CPP for Windows x64 games" [here](https://builds.bepinex.dev/projects/bepinex_be) (Only the bleeding edge builds support Il2CPP games which is what Banana Mania is)
2. Extract it in your game folder)

## Install Plugin Dependencies

1. Install [SMBBM Leaderboard Disabler](https://github.com/bobjrsenior/SMBBMLeaderboardDisabler/releases)

## Installing This Plugin

1. Download the SMBBMFileRedirector.BepInEx.zip file from the Releases page
2. Extract it in your main game folder (the zip file structure should put the plugin in the right place)

## Using The Plugin

This Plugin uses JSON files to configure what game Assets should be patched. The Plugin looks for these JSON files under the "UserData/FileReplacements" directory within your games install folder.

It is good practice to name your configuration file after your custom file pack.

For example, if you wanted a custom Asset Bundle pack called "MyAwesomeAssets", you could make your configuration file "MyAwesomeAssets.json". Additionally, please place your custom AssetBundles within a folder under "UserData/FileReplacements". For the "MyAwesomeAssets" example, it would be a folder called "UserData/FileReplacements/MyAwesomeAssets". To keep Asset types organized like the game does, you could also choose "UserData/FileReplacements/AssetBundle/MyAwesomeAssets".

## JSON Configuration

The JSON configuration file is what determines the AssetBundles to redirect. The Plugin supports having multiple configurations so you can have multiple Asset Packs installed at once (although having multiple expansive packs could override each other).

The configuration file format looks like this

```json
{
    "name": "Bob's Asset Pack Example",
    "description": "This pack serves as an example of how to make a configuration file for SMBBM FileRedirector",
    "author": "bobjrsenior",
    "file_format_version": 2,
    "asset_bundles": {
        "ui_sel_logo_en": "AssetBundle/ui_sel_logo_en"
    },
    "cue_sheets": {
        "adv": {
            "acb": "Sound/HelloHelloHelloHello/adv.acb",
            "awb": "Sound/HelloHelloHelloHello/adv.awb"
        },
        "custom_cue_sheet": {
            "acb": "Sound/HelloHelloHelloHello/custom_cue_sheet.acb"
        }
    },
    "cue_to_cue_sheet": {
        "se_com_select": "custom_cue_sheet"
    },
    "movies": {
        "S01_A.usm": "Movie/SMB2 Cutscenes/S01_A.usm",
        "S01_B.usm": "Movie/SMB2 Cutscenes/S01_B.usm"
    }
}
```

Going into more detail, here are descriptions for each field in the configuration:
| Field               | Description                                             |
| :-------------------| :------------------------------------------------------ |
| name                | The name of your Asset Pack (informational only)        |
| description         | The description of your Asset Pack (informational only) |
| author              | The author of your Asset Pack (informational only)      |
| file_format_version | Always 2. This will be used if the configuration format is changed in the future for possible backwards compatability |
| asset_bundles       | A dictionary of key value pairs. Each Key should be the name of an existing SMBBM AssetBundle (ex: "stage_smb1_bonus_st1091"). Each value should be the relative path to your replacement bundle. Example: If your replacement bundle for "ui_sel_logo_en" was "UserData/FileReplacements/AssetBundles/MyAwesomeAssets/ui_sel_logo_en", you would put `"stage_smb1_bonus_st1091": "AssetBundle/MyAwesomeAssets/ui_sel_logo_en"` for one of the entries |
| cue_sheets          | A dictionary of key value pairs. Each key is the name of a Cue Sheet (ex: "adv") and each value is an object with fields for "acb" and "awb". The "acb" and "awb" values specify the file locations you want to redirect the Cue Sheet to. The "awb" value is optional since not every Cue Sheet has one. Example: If your replacement acb/awb for the Cue Sheet "adv" was "UserData/FileReplacements/Sound/HelloHelloHelloHello/adv.acb" and "UserData/FileReplacements/Sound/HelloHelloHelloHello/adv.awb", you would put `"adv": { "acb": "Sound/HelloHelloHelloHello/adv.acb", "awb": "Sound/HelloHelloHelloHello/adv.awb"}`     |
| cue_to_cue_sheet    | (Advanced) A dictionary of key value pairs. Each key is the name of a Cue (ex: "se_com_select") and each value is a Cue Sheet to redirect the Cue to. Example: If you wanted to redirect the "se_com_select" Cue to the Cue Sheet "custom_cue_sheet" you would put `"se_com_select": "custom_cue_sheet"`. This is mainly useful to replace individual Cues in a larger Cue Sheet.   |
| movies              | A dictionary of key value pairs. Each Key should be the name of an existing SMBBM Movie (ex: "S01_A.usm"). Each value should be the relative path to your replacement movie. Example: If your replacement movie for "S01_A.usm" was "UserData/FileReplacements/Movie/MyAwesomeAssets/S01_A.usm", you would put `"S01_A.usm": "Movie/MyAwesomeAssets/S01_A.usm"` for one of the entries      |

## Example Configurations

To see example configurations and explanations, see [Examples](/Examples).

## Asset Names

To redirect things, you need the name. This repo has a list of the different names for convenience.

The links to them are:
* [AssetBundles](/Game_Constants/AssetBundles.txt)
* [Cue Sheets](/Game_Constants/CueSheets.txt)
* [Cues](/Game_Constants/Cues.txt)
* [Moves](/Game_Constants/Movies.txt)

## Building

## Setup

I use Visual Studio 2022  for development although I beleive it can also be compiled via command line. Additionally, make sure you setup your enviroment for BepInEx plugin development if building for BepInEx: https://docs.bepinex.dev/master/articles/dev_guide/plugin_tutorial/1_setup.html

## Configuration

In the .csproj, there is an element called `<SMBBMDir>` and `<SMBDirBep>`. If building for BMM, you should edit `<SMBBMDir>` to point to your game installation where BMM is installed. If building for BepInEx, you should edit `<SMBBMDir>` to point to your game installation where BMM is installed. The project references are determined based on that.

There are 2 Visual Studio build configurations:

1. Release_BMM: Builds a DLL for Banana Mod Manager
2. Release_BepInEx: Builds a DLL for BepInEx

Make sure you use the right configuration for your mod loader.
