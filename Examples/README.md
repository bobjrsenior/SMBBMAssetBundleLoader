# Examples Walkthrough

This folder contains 2 example JSON configuration files: Bobs_Asset_Pack_Example.json and SMB2 Cutscenes.json.

## Bobs_Asset_Pack_Example.json

```json
{
  "name": "Bob's Asset Pack Example",
  "description": "This pack serves as an example of how to make a configuration file for SMBBM FileRedirector",
  "author": "bobjrsenior",
  "file_format_version": 2,
  "asset_bundles": {
    "ui_sel_logo_en": "AssetBundle/Bobs_Asset_Pack_Example/ui_sel_logo_en"
  },
  "cue_sheets": {
    "adv": {
      "acb": "Sound/Bobs_Asset_Pack_Example/adv.acb",
      "awb": "Sound/Bobs_Asset_Pack_Example/adv.awb"
    },
    "selector": {
      "acb": "Sound/Bobs_Asset_Pack_Example/selector.acb",
      "awb": "Sound/Bobs_Asset_Pack_Example/selector.awb"
    },
    "custom_cue_sheet": {
      "acb": "Sound/Bobs_Asset_Pack_Example/custom_cue_sheet.acb"
    }
  },
  "cue_to_cue_sheet": {
    "se_com_select": "custom_cue_sheet"
  }
}
```

This configuration files included redirects for both AssetBundles and Audio files so I'll explain each part separately.

### asset_bundles

```json
"asset_bundles": {
  "ui_sel_logo_en": "AssetBundle/Bobs_Asset_Pack_Example/ui_sel_logo_en"
}
```

This simply redirects the ui_sel_logo_en AssetBundle to a custom one stored at "UserData/FileReplacements/AssetBundle/Bobs_Asset_Pack_Example/ui_sel_logo_en"

### cue_sheets

```json
"cue_sheets": {
  "adv": {
    "acb": "Sound/Bobs_Asset_Pack_Example/adv.acb",
    "awb": "Sound/Bobs_Asset_Pack_Example/adv.awb"
  },
  "selector": {
    "acb": "Sound/Bobs_Asset_Pack_Example/selector.acb",
    "awb": "Sound/Bobs_Asset_Pack_Example/selector.awb"
  },
  "custom_cue_sheet": {
    "acb": "Sound/Bobs_Asset_Pack_Example/custom_cue_sheet.acb"
  }
}
```

Each object in cue_sheets should be the name of a Cue Sheet. In this example, that would be "adv", "selector", and "custom_cue_sheet".

If you specify a Cue Sheet that does not exist in the game, it will be injected into the game's list of Cue Sheets (why you would do this is explaned in the next section.

Cue Sheets can be composed of both acb and awb files. Because of that, for each Cue Sheet you specify, you can provide an acb filepath and an awb filepath. The awb filepath is optional since not every Cue Sheet has one. This will redirect the Cue Sheet file location to your custom one.

### cue_to_cue_sheet

```json
"cue_to_cue_sheet": {
  "se_com_select": "custom_cue_sheet"
}
```

This is where it gets a little tricky. Every SFX/Audio Clip/Song is a Cue. Cues are collected into Cue Sheets. A Cue Sheet can have many Cues. That brings us to the question: "How can I replace just 1 Cue in a big Cue Sheet?".

That's where cue_to_cue_sheet comes in. It redirects a specific Cue to a different Cue Sheet. In this example, it redirects the "se_com_select" Cue to the "custom_cue_sheet" Cue Sheet (the one that we injected in the previous section).

This make it so that we can inject a custom Cue Sheet with our custom audio. Then redirect the Cues we want to replace to our custom Cue Sheet.

With this strategy, you can have many different audio packs modifying different Cues that come from a single base game Cue Sheet.


## SMB2 Cutscenes.json

Unfortunately I don't know how to make the game video (.usm) files myself so this example uses an existing mod called ["Super Monkey Ball 2 Cutscenes"](https://gamebanana.com/mods/327155).

As an extra step to use this configuration, you need to download the "Super Monkey Ball 2 Cutscenes" mod and put the .usm files from it into the "Movie/SMB2 Cutscenes/" folder yourself.

```json
{
  "name": "Super Monkey Ball 2 Cutscenes",
  "description": "Replaces all cutscenes in the game with those from Super Monkey Ball 2",
  "author": "Minaline",
  "file_format_version": 1,
  "movies": {
    "S01_A.usm": "Movie/SMB2 Cutscenes/S01_A.usm",
    "S01_B.usm": "Movie/SMB2 Cutscenes/S01_B.usm",
    "S02_A.usm": "Movie/SMB2 Cutscenes/S02_A.usm",
    "S02_B.usm": "Movie/SMB2 Cutscenes/S02_B.usm",
    "S03_A.usm": "Movie/SMB2 Cutscenes/S03_A.usm",
    "S03_B.usm": "Movie/SMB2 Cutscenes/S03_B.usm",
    "S04_A.usm": "Movie/SMB2 Cutscenes/S04_A.usm",
    "S04_B.usm": "Movie/SMB2 Cutscenes/S04_B.usm",
    "S05_A.usm": "Movie/SMB2 Cutscenes/S05_A.usm",
    "S05_B.usm": "Movie/SMB2 Cutscenes/S05_B.usm",
    "S06_B.usm": "Movie/SMB2 Cutscenes/S06_B.usm",
    "S06_A.usm": "Movie/SMB2 Cutscenes/S06_A.usm",
    "S07_B.usm": "Movie/SMB2 Cutscenes/S07_B.usm",
    "S07_A.usm": "Movie/SMB2 Cutscenes/S07_A.usm",
    "S08_B.usm": "Movie/SMB2 Cutscenes/S08_B.usm",
    "S08_A.usm": "Movie/SMB2 Cutscenes/S08_A.usm",
    "S09_B.usm": "Movie/SMB2 Cutscenes/S09_B.usm",
    "S09_A.usm": "Movie/SMB2 Cutscenes/S09_A.usm",
    "S10_B.usm": "Movie/SMB2 Cutscenes/S10_B.usm",
    "S10_A.usm": "Movie/SMB2 Cutscenes/S10_A.usm"
  }
}
```

The "SMB2 Cutscenes.json" utilizes the "movies" field and redirects every default .usm movie file to custom ones stored in"UserData/FileReplacements/Movie/SMB2 Cutscenes".
