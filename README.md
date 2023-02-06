# WLEditor

This a level editor for Super Mario Land 3 / WarioLand 1 (1993).

## Features
- edit levels (tiles / enemies / sectors / warps)
- edit world maps and overworld (tiles / events / paths)
- rom expansion (for level tiles only)

## How to use 
Click on "File" > "Load". Select a valid WL1 rom.
Then, select a course in the combobox.

### Navigating through levels
Each level is made of 2 x 16 = 32 sectors.

Some sectors will warp player to another sector (or the exit) once a door is used. It will also load new tile graphics and enemies at the same time.

In the editor, near the sector number, a label "S.." indicate if sector will warp player to another sector.

<kbd>Middle-click</kbd> on a sector to select it. If selected sector is associated with a warp (usually there is a door in it), it will load related tiles and enemies.

### Editing level tiles
Toolbox must be active (go to "View" > "Toolbox"). Select "16x16 Tiles" view.
- Select a tile in the toolbox by clicking on it.
- Then, right-click inside level view. 

### Editing objects (enemies / power-ups)
Toolbox must be active. Select "Objects" view.
- Select an enemy or a power up in the toolbox by clicking on it. 
- Then, right-click inside level view. 

Available enemies / objects depends of current sector / warp.

## Copy / paste 
This is a powerful feature. Here is an overview of possibilities: 
- Copy / move a block of tiles from one place to another.
- Fill / erase a given block with a single tile.
- Repeat a pattern.

How to :
1. <kbd>Left-click</kbd> on a tile, hold mouse button and drag cursor to create a selection (eg: a 3x3 block). Release mouse button. A yellow rectangle should appear.
2. Press <kbd>Ctrl-C</kbd> / <kbd>Ctrl-X</kbd> to copy / cut selection.
3. <kbd>Left-click</kbd> somewhere else (eg: a 1x1 block is selected).
4. Press <kbd>Ctrl-V</kbd> to paste selection at that position. 

- It's possible to repeat a selection (eg: to create a pattern) by creating a selection bigger than 1x1 in step 3.
- You can delete what is under selection by pressing <kbd>Delete</kbd> key.
- You can undo or redo changes with <kbd>Ctrl-Z</kbd> and <kbd>Ctrl-Y</kbd>.

## Warp / level header
- Open "View" > "Warp / level header".
- <kbd>Middle-click</kbd> on a sector to highlight it.
If you click on a sector already selected, it will unselected sector and show level header (which has similar properties to warps).

Enemy sets specify which enemies will be loaded for a given warp (up to 6 enemies loaded). Some enemy sets are specific to some places (eg: boss or treasure room, ...).
"X" near treasure enemy set means game will check if related treasure key has already been collected. If it is, it will remove key and will already open skull door leading to treasure. It should be used for all warps leading to a sector that have treasure key or door.

## Overworld / world maps
- Open "View" > "Overworld".
- Select a world in combobox. 
- You can zoom in / out using <kbd>Ctrl</kbd> + <kbd>Mouse wheel</kbd>.

### Editing map tiles 

| Key | Description |
| :-: | - |
| <kbd>Right-click</kbd> in right side | Select current tile.
| <kbd>Right-click</kbd> in left side | Add a new tile.
| <kbd>Ctrl</kbd> + <kbd>C</kbd> <br> <kbd>Ctrl</kbd> + <kbd>V</kbd> | [Copy / paste](#copy--paste) tiles (between left and right side or in left side).
| <kbd>Ctrl</kbd> + <kbd>Z</kbd> <br> <kbd>Ctrl</kbd> + <kbd>Y</kbd> | Undo / redo changes.

### Editing events
Events are used to update tiles during gameplay, it's usually triggered after completing a level (eg: event 1 is triggered after completing first level and so on).

| Key | Description |
| :-: | - |
| <kbd>E</kbd> | Enable / disable event mode.
| <kbd>Page-Up</kbd> <br> <kbd>Page-Down</kbd> | Select event.
| <kbd>Home</kbd> <br> <kbd>End</kbd> | Select event step.
| <kbd>Right-click</kbd> in left side | Add event at current step.
| <kbd>Delete</kbd> | Delete event at current step.
| <kbd>Shift</kbd> + <kbd>Delete</kbd> | Delete all steps of current event.
| <kbd>Ctrl</kbd> + <kbd>C</kbd> <br> <kbd>Ctrl</kbd> + <kbd>V</kbd> | [Copy / paste](#copy--paste) events.
| <kbd>Ctrl</kbd> + <kbd>Z</kbd> <br> <kbd>Ctrl</kbd> + <kbd>Y</kbd> | Undo / redo changes.

- Tiles in amber are tiles related to current event.
- Tiles in cyan are current event tiles already applied.
- Tile 255 (which is rightmost tile of the middle row) cannot be used in events because 255 is a special value used as a marker. It's a limitation of the game engine.

### Editing paths
| Key | Description |
| :-: | - |
| <kbd>P</kbd> | Enable / disable path mode.
| <kbd>Page-Up</kbd> <br> <kbd>Page-Down</kbd> | Select a level.
| <kbd>Ctrl</kbd> + <kbd>↑</kbd> <br> <kbd>Ctrl</kbd> + <kbd>↓</kbd> <br> <kbd>Ctrl</kbd> + <kbd>←</kbd> <br> <kbd>Ctrl</kbd> + <kbd>→</kbd> | Move current level. Position will be aligned on a 4 x 4 grid.
| <kbd>←</kbd> <kbd>↑</kbd> <kbd>→</kbd> <kbd>↓</kbd> | Select a direction. There is 4 possible directions. If there is a path in chosen direction, it will be automatically highlighted.
| <kbd>Shift</kbd> + <kbd>↑</kbd> <br> <kbd>Shift</kbd> + <kbd>↓</kbd> <br> <kbd>Shift</kbd> + <kbd>←</kbd> <br> <kbd>Shift</kbd> + <kbd>→</kbd> | Add a new path segment in that direction. 
| <kbd>Delete</kbd> | Delete last path segment.
| <kbd>Shift</kbd> + <kbd>Delete</kbd>  | Delete all paths in all directions.
| <kbd>I</kbd> | Enable / disable hidden path. Next path segment will be set as hidden (eg: player won't be visible when walking on that path segment). Hidden path is shown in red.
| <kbd>W</kbd> | Enable / disable underwater path mode. Underwater path is shown in blue.
| <kbd>F</kbd> | Set progress required to take that path. A direction must be selected. A number is shown near level to indicate progression required. Nothing shown means no progress is required.
| <kbd>X</kbd> | Set end of path exit target. A = Overworld. B = Sherbet Land. C = Mt Teapot.

Tips for paths : 

- Path last segment must end up exactly at same position as level to be connected. It might be necessary to move level first to align it on the grid.
- Events should be edited to match paths. If you don't want to bother with events, remove all events and hardcode paths in map.
- It you are planning to do a new map, it's easier to delete all events and paths first.