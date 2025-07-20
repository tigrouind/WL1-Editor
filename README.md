
# WLEditor
This a level editor for Super Mario Land 3 / WarioLand 1 (1993).
You need to install [.NET Desktop Runtime 9.0](https://dotnet.microsoft.com/en-us/download/dotnet/9.0) to run it.

## Features
- Edit levels (tiles / enemies / sectors / warps)
- Edit world maps and overworld (tiles / events / paths)
- Rom expansion (for level tiles only)

## Emulator 🎮
Using [BGB](https://bgb.bircd.org/) is recommended. This [ram map](https://datacrystal.tcrf.net/wiki/Wario_Land:_Super_Mario_Land_3/RAM_map) can be used to quickly skip through levels. The most useful addresses are **A375** (writing 01 will finish a level instantaneously) and **A80B-A814** (write FF in all range to have everything on the map marked has completed). To edit the memory, press <kbd>Esc</kbd> (this will open the debugger) then <kbd>Ctrl</kbd> + <kbd>G</kbd>, enter the address you want to edit, then press <kbd>Enter</kbd>.

## Showcase
Here is projects made using that utility (I will gladly add your ROM hack to this list)
- [Jet Wario Land](https://www.romhacking.net/hacks/9057/)

## How to use
- Click on <kbd>File</kbd> > <kbd>Load</kbd>
- Select a valid WL1 rom.
- Select a course in the combobox.

### Navigating through levels 🧭
Each level is made of 2 x 16 = 32 sectors.

Some sectors will warp player to another sector (or the exit) once a door is used. It will also load new tile graphics and enemies at the same time.

In the editor, near the sector number, a label <kbd>S..</kbd> indicate if sector will warp player to another sector.

<kbd>Middle-click</kbd> on a sector to select it. If that sector is associated with a warp (usually there is a door in it), it will load related tiles and enemies. If you hold <kbd>Shift</kbd> or <kbd>Control</kbd> during click, it will select the first sector that targets the sector under mouse.

### Editing level tiles 🏁
- Open <kbd>Window</kbd> > <kbd>Blocks</kbd>.
- Select a tile in the toolbox by left clicking on it.
- Right-click inside level view. 

### Editing objects (enemies 👾/ power-ups⚡️)
- Open <kbd>Window</kbd> > <kbd>Objects</kbd>.
- Select an enemy or a power up in the toolbox by left clicking on it. 
- Right-click inside level view. 

Available enemies depends of current sector / warp.

| Key | Description |
| :-: | - |
| <kbd>Ctrl</kbd> + <kbd>Mouse wheel</kbd> | Zoom in / out.
| <kbd>Ctrl</kbd> + <kbd>C</kbd> <br> <kbd>Ctrl</kbd> + <kbd>V</kbd> | [Copy / paste](#copy--paste-) tiles and enemies.
| <kbd>Ctrl</kbd> + <kbd>Z</kbd> <br> <kbd>Ctrl</kbd> + <kbd>Y</kbd> | Undo / redo changes.
| <kbd>Delete</kbd> | Clear what is currently under selection.

## Copy / paste 📋
This is a powerful feature. Here is an overview of possibilities: 
- Copy / move a block of tiles from one place to another.
- Fill / erase a given block.
- Repeat a pattern.

How to :
1. <kbd>Left-click</kbd> on a tile, hold left mouse button and drag cursor to create a selection (eg: a 3x3 block). Release mouse button. A yellow rectangle should appear.
2. Press <kbd>Ctrl-C</kbd> / <kbd>Ctrl-X</kbd> to copy / cut selection.
3. <kbd>Left-click</kbd> somewhere else (eg: a 1x1 block is selected).
4. Press <kbd>Ctrl-V</kbd> to paste selection at that position. 

Tips :
- If you paste result into a selection bigger than what has been copied, pattern will be repeated.
- You can delete what is under selection by pressing <kbd>Delete</kbd> key.
- You can undo or redo changes with <kbd>Ctrl-Z</kbd> / <kbd>Ctrl-Y</kbd>.

## Sectors / level header 
- Open <kbd>View</kbd> > <kbd>Sectors / Level header</kbd>
- <kbd>Middle-click</kbd> on a sector to highlight it.
If you click on a sector already selected, it will unselected sector and show level header (which has similar properties to warps).

Enemy sets specify which enemies will be loaded for a given warp (up to 6 enemies loaded). Some enemy sets are specific to some places (eg: boss or treasure room, ...).

Tips : 
- 🟥 Red items are boss enemies sets.
- 🟨 Yellow items are related to treasure rooms. 
- ⬜ Grey items :
  - Exit skull already open (no need to use coins)
  - Guragura with coin sequence (as in 1st level ending)
  - Pouncer that follows player (will try go right, then down, then up)

### Checkpoints ✅
Checkpoints can be enabled or disabled in level header. Each checkpoint has a dedicated warp that should be edited if needed. This warp will be loaded if checkpoint is used (eg: player died and enter level again from world map).

### Treasure rooms 👑
Treasure rooms have their own dedicated warps (one for each treasure, 15 in total).
These warps are used during the transition between treasure totals screen and treasure room.

Warps that lead to an area with a key (or a skull) must use the enemy set with the letter corresponding to that treasure.
Eg: if a warp leads to an area where there is a key for treasure J, an enemy set with the letter J must be used.
When such a warp is loaded, if the treasure has already been collected, the game will automatically open the skull and remove the key.

## Overworld / world maps 🌍
- Open <kbd>View</kbd> > <kbd>Overworld</kbd>
- Select a world in combobox. 

### Editing map tiles 🏁
| Key | Description |
| :-: | - |
| <kbd>Right-click</kbd> in right side | Select current tile.
| <kbd>Right-click</kbd> in left side | Add a new tile.
| <kbd>Ctrl</kbd> + <kbd>Mouse wheel</kbd> | Zoom in / out.
| <kbd>Ctrl</kbd> + <kbd>C</kbd> <br> <kbd>Ctrl</kbd> + <kbd>V</kbd> | [Copy / paste](#copy--paste-) tiles (between left and right side or in left side).
| <kbd>Ctrl</kbd> + <kbd>Z</kbd> <br> <kbd>Ctrl</kbd> + <kbd>Y</kbd> | Undo / redo changes.
| <kbd>Delete</kbd> | Clear what is currently under selection.

Tips : 
It's possible to edit tiles/map in an external tool (eg: YY-CHR, Tilemap Studio, ...) using the export/import functionality : <kbd>Edit</kbd> > <kbd>Tile data</kbd>.

### Editing paths 🏞
| Key | Description |
| :-: | - |
| <kbd>Page-Up</kbd> <br> <kbd>Page-Down</kbd> | Select a level.
| <kbd>Ctrl</kbd> + <kbd>↑<br>← ↓ →</kbd> | Move current level. Position will be aligned on a 4 x 4 grid.
| <kbd>↑<br>← ↓ →</kbd> | Select a direction (N/S/E/W). Path in current direction is highlighted.
| <kbd>Shift</kbd> +  <kbd>↑<br>← ↓ →</kbd> | Add a new path segment in that direction. 
| <kbd>Alt</kbd> + <kbd>↑<br>← ↓ →</kbd> | Move overworld completion flag.
| <kbd>Delete</kbd> | Delete last path segment.
| <kbd>Shift</kbd> + <kbd>Delete</kbd> | Delete all paths in all directions.
| <kbd>M</kbd> | Change path mode. This will only be set for new paths.<br>Green = normal path.<br>Red = player won't be visible when walking on such paths.<br>Blue = underwater.
| <kbd>P</kbd> | Set [progress](#progress-) required to take current path. A direction must be selected.<br>A number is shown near level to indicate progression required.<br>Nothing shown means no progress is required.
| <kbd>E</kbd> | Set end of path exit target.<br>🅞 = Overworld.<br>🅢 = Sherbet Land.<br>🅣 = Mt Teapot.

Tips : 
- Path last segment must end up exactly at same position as level to be connected. It might be necessary to move level to align it on the grid.
- Events should be edited to match paths. If you don't want to bother with events, remove all events and hardcode paths in map.
- It you are planning to do a new overworld map, it's easier to delete all events and paths first.

### Editing events 📅
[Events](#Progress-) are used to update map tiles during gameplay, it's usually triggered after completing a level (eg: event 1 is triggered after completing first level, making a path to second level and so on).

| Key | Description |
| :-: | - |
| <kbd>Page-Up</kbd> <br> <kbd>Page-Down</kbd> | Select event.
| <kbd>Home</kbd> <br> <kbd>End</kbd> | Select event step.
| <kbd>Right-click</kbd> in left side | Add event at current step.
| <kbd>Delete</kbd> | Delete event at current step.
| <kbd>Shift</kbd> + <kbd>Delete</kbd> | Delete all steps of current event.
| <kbd>Ctrl</kbd> + <kbd>C</kbd> <br> <kbd>Ctrl</kbd> + <kbd>V</kbd> | [Copy / paste](#copy--paste-) events.
| <kbd>Ctrl</kbd> + <kbd>Z</kbd> <br> <kbd>Ctrl</kbd> + <kbd>Y</kbd> | Undo / redo changes.
| <kbd>Delete</kbd> | Clear what is currently under selection.

Tips : 
- Tiles in amber are tiles related to current event.
- Tiles in cyan are current event tiles already applied.
- Tile 0x7F (which is rightmost tile of the middle row) cannot be used in events because it's a marker. It's a limitation of the game engine.

### Progress 🚩
This table show which progress unlocked depending the completed level : 

| Progress | Rice beach | Mt. Tea Pot | Sherbet Land | Stove Canyon | SS Tea Cup | Parsely Woods | Syrup Castle | Overworld |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| 1 | 1 | 1 | 1 | 1 | 1 | 1 | 1 | 1 |
| 2 | 2 | 2 | 2 | 2 | 2 | 2❗ | 2❗ | 2 |
| 3 | 3 | 2 ➡️ | 2 ➡️ | 3 | 3 | 3 | 3❗ | 4 |
| 4 | 3 ➡️ | 3 | 3 | 4 | 4 | 4 | | 5 |
| 5 | 4 | 4 | 3 ➡️ | 4 ➡️ | 5 ⚔️ | 5 | | 6 |
| 6 | 5 ⚔️ | 5 | 4 ❓ | 5 ❓ | | 6 ⚔️ | | 3 |
| 7 | 6 ❓ | 6❗ | 5 ❓ | 6 ⚔️ | | | | |
| 8 | | 4 ⚔️ | 6 ⚔️ | | | | | | |

| Icon | Description |
| :--: | :- |
| ➡️ | secret exit (leading to secret level) |
| ❓ | secret level |
| ❗ | special event (huge [!] block) |
| ⚔️ | boss / last level |