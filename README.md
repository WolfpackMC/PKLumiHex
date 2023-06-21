PKLumiHeX
==
## Download: [Latest Release](https://github.com/TalonSabre/PKLumiHex/releases/latest)
### Fork of [PKHeX](https://github.com/kwsch/PKHeX) with added support for Luminescent Platinum!
### PKLumiHeX is a Work in progress. Some features will not work when editing a Luminescent Platinum save.
##

### Features working on Luminescent Platinum saves
- Pokémon Editor (Box and Party)
  - Stat and Ability changes will display
  - New Pokémon Luminescent forms included!
  - Individual Pokémon can be imported (\*.pb8, \*.pb8lumi) and exported (\*.pb8lumi)
  - Can still use Save Box Data++
- Bag Editor
  - Luminescent-specific items will show
- Box Layout Editor
- Seal Stickers Editor
- Pokédex Editor

### Unconfirmed if working
- Poffin Editor
- Trainer Info Editor
  - Specifically Gender, Version, Language, Adventure Info, and Stats
    - Luminescent Platinum only supports Brilliant Diamond Version, and the English Language for now!
- Underground (Bag) Editor

### Unimplemented features
- Event Flags Editor
  - Does not contain new flags

### Known 'Issues'
- Shiny Pokémon and certain forms display as the default pixel art sprites
- Sprites for Luminescent forms do not change regardless of shinyness or sprite preference
    - The shiny symbol *will* still display on the top left, though.
- Some moves will display an incorrect amount of PP

##
<h1 align=center>Screenshots</h1>
<div align=center>
    <p align=center>
        <img src="https://i.imgur.com/UoyTq53.png" width="47.5%">
        <img src="https://i.imgur.com/wYu3RFK.png" width="47.5%" padding-right=5>
    </p>
    <p>
        <img align=center src="https://i.imgur.com/8wgwPlS.png" width="47.5%">
        <img align=center src="https://i.imgur.com/311Nlkd.png" width="47.5%">
    </p>
    <div align=center>
        <p>
            <img src="https://i.imgur.com/gGJ0PFK.png" width="22.635%">
            <img src="https://i.imgur.com/t0HF7f3.png" width="22.5%">
            <img src="https://i.imgur.com/xpkJb0l.png" width="45.11%">
        </p>
    </div>
</div>

-----
# PKHeX
<div>
  <span>English</span> / <a href=".github/README-es.md">Español</a> / <a href=".github/README-fr.md">Français</a> / <a href=".github/README-de.md">Deutsch</a> / <a href=".github/README-it.md">Italiano</a> / <a href=".github/README-zhHK.md">繁體中文</a> / <a href=".github/README-zh.md">简体中文</a>
</div>

![License](https://img.shields.io/badge/License-GPLv3-blue.svg)

Pokémon core series save editor, programmed in [C#](https://en.wikipedia.org/wiki/C_Sharp_%28programming_language%29).

Supports the following files:
* Save files ("main", \*.sav, \*.dsv, \*.dat, \*.gci, \*.bin)
* GameCube Memory Card files (\*.raw, \*.bin) containing GC Pokémon savegames.
* Individual Pokémon entity files (.pk\*, \*.ck3, \*.xk3, \*.pb7, \*.sk2, \*.bk4, \*.rk4)
* Mystery Gift files (\*.pgt, \*.pcd, \*.pgf, .wc\*) including conversion to .pk\*
* Importing GO Park entities (\*.gp1) including conversion to .pb7
* Importing teams from Decrypted 3DS Battle Videos
* Transferring from one generation to another, converting formats along the way.

Data is displayed in a view which can be edited and saved.
The interface can be translated with resource/external text files so that different languages can be supported.

Pokémon Showdown sets and QR codes can be imported/exported to assist in sharing.

PKHeX expects save files that are not encrypted with console-specific keys. Use a savedata manager to import and export savedata from the console ([Checkpoint](https://github.com/FlagBrew/Checkpoint), save_manager, [JKSM](https://github.com/J-D-K/JKSM), or SaveDataFiler).

**We do not support or condone cheating at the expense of others. Do not use significantly hacked Pokémon in battle or in trades with those who are unaware hacked Pokémon are in use.**

## Building

PKHeX is a Windows Forms application which requires [.NET 7.0](https://dotnet.microsoft.com/download/dotnet/7.0).

The executable can be built with any compiler that supports C# 11.

### Build Configurations

Use the Debug or Release build configurations when building. There isn't any platform specific code to worry about!

## Dependencies

PKHeX's QR code generation code is taken from [QRCoder](https://github.com/codebude/QRCoder), which is licensed under [the MIT license](https://github.com/codebude/QRCoder/blob/master/LICENSE.txt).

PKHeX's shiny sprite collection is taken from [pokesprite](https://github.com/msikma/pokesprite), which is licensed under [the MIT license](https://github.com/msikma/pokesprite/blob/master/LICENSE).

PKHeX's Pokémon Legends: Arceus sprite collection is taken from the [National Pokédex - Icon Dex](https://www.deviantart.com/pikafan2000/art/National-Pokedex-Version-Delta-Icon-Dex-824897934) project and its abundance of collaborators and contributors.

### IDE

PKHeX can be opened with IDEs such as [Visual Studio](https://visualstudio.microsoft.com/downloads/) by opening the .sln or .csproj file.
