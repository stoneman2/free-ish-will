# Free-ish Will
All credits to the original creator of the Free Will mod.
This fork adds numerous changes to the Free Will mod, adding features that I deem fun, not really going for the original mod's vision, really.
I've brought it more in line with other auto-work management mods.

## Performance
The original mod recalculates everyone's work priorities and modifiers EVERY tick, which I think is quite ridiculous. With this, I've added tick throttling.
Work calculations now happens in intervals of ticks based on your settings (Default 4).
This has improved the performance of the mod by upwards of 80 - 90%.
## Before
<img width="1164" height="249" alt="image" src="https://github.com/user-attachments/assets/f3549283-60f4-41f1-aec7-f220e7ace7ff" />

## After
<img width="1258" height="351" alt="image" src="https://github.com/user-attachments/assets/c345087a-6b80-49e8-8756-554ae8e7e386" />


## Features
- Added the mod setting's button right to the work menu.
- Added a button to refresh the current work calculation.
- Added global focusing. You can make all your pawns focus on one type of work specifically.
- Added pawn focusing. You can make each one of your pawns focus on one type of work specifically.
- Added the ability to turn off pawn's free will directly in the work menu. (Where the original mod had you select them to open a menu, and then turn them off..)
- Added the ability to turn off one specific's work from being free-willable.


# Original Mod Description
# Free Will

Colonists have free will! Adds a precept that influences how willing colonists are to do what they're told.

## Prerequisites

- RimWorld must be installed. By default the build expects the game in `C:\Program Files (x86)\Steam\steamapps\common\RimWorld`. Set the `RIMWORLD_DIR` environment variable if your copy lives elsewhere.
- A .NET SDK capable of targeting **.NET Framework 4.7.2** is required.

## Building the DLL

Use the `dotnet` CLI in the repository root. The build process reads `RIMWORLD_DIR` and copies the result into `Mods/FreeWill` inside the game directory.

```bash
RIMWORLD_DIR="C:\\Games\\RimWorld" dotnet build -c Stable
```

Use `-c Unstable` for a debug build.

## Key files

- `FreeWill_Mod.cs` &ndash; entry point for the mod and Harmony patches.
- `FreeWill_MapComponent.cs` &ndash; `MapComponent` that tracks colony state and updates work priorities.

See [docs/architecture.md](docs/architecture.md) for an overview of how these pieces work together.
See [docs/settings.md](docs/settings.md) for a list of configurable mod settings.

For a detailed overview of features see [description.txt](description.txt).
This file is also the description shown on the Steam Workshop. Update it for each
release and follow the steps in [docs/workshop.md](docs/workshop.md) to keep the
page in sync.
