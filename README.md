# WLEditor

This a level editor for Super Mario Land 3 / WarioLand 1 (1993).

## Features
- edit level blocks
- edit object data (enemies / power-ups)
- rom expansion (for level blocks only)
- edit level headers / sectors / warps 
- edit world maps and overworld (tiles / events / paths)

## How to use 
Click on "File" > "Load". Select a valid WL1 rom.
Then, select a course in the combobox.

### Navigating through levels
Each level is made of 2 x 16 = 32 sectors.

Some sectors will warp player to another sector (or the exit) once a door is used. It will also load new tile graphics and enemies at the same time.

In the editor, near the sector number, a label "S.." indicate if sector will warp player to another sector.

<kbd>Middle-click</kbd> on a sector to select it. If selected sector is associated with a warp (usually there is a door in it), it will load related tiles and enemies.

### Editing level blocks
Toolbox must be active (go to "View" > "Toolbox" or press <kbd>F1</kbd>). Select "16x16 Tiles" view.
- Select a tile in the toolbox by clicking on it.
- Then, click inside level view. 

#### Copy / paste 

It's a very powerful feature. Here is an overview of possibilities: 
- Copy a block of tiles from one place to another.
- Fill / erase a given block with single tile 
- Repeat a pattern

Toolbox must be inactive. 
1. Left click on a block, hold mouse button and drag cursor to create a selection (eg: a 3x3 block). Release mouse button.
2. Press <kbd>Ctrl-C</kbd> to copy block.
3. Left click somewhere else (eg: a 1x1 block is selected).
4. Press <kbd>Ctrl-V</kbd> to paste block at that position. 

It's possible to repeat a selection (eg: to create a pattern) by creating a selection bigger than 1x1 in step 3.

### Editing objects (enemies / power-ups)
Toolbox must be active. Select "Objects" view.
- Select an enemy or a power up in the toolbox by clicking on it. 
- Then, click inside level view. 

<kbd>Right-click</kbd> can be used to delete objects. Available enemies / objects depends of current sector / warp.

### Editing sectors / warps
Toolbox must be active. Select "Sector" view.
- <kbd>Middle-click</kbd> on a sector to select it.
- Change sector properties in toolbox.

If you select a sector that has no warp, you will be editing level header (which has similar properties to warps).

Enemy sets specify which enemies will be loaded for a given warp (up to 6 enemies loaded). Some enemy sets are specific to some places (eg: boss or treasure room, ...).
"X" near treasure enemy set means game will check if related treasure key has already been collected. If it is, it will remove key and will already open skull door leading to treasure. It should be used for all warps leading to a sector that have treasure key or door.

## Overworld / world maps

- Open "View" > "Overworld" (or press <kbd>F7</kbd>). 
- Select a world in combobox. 
- You can zoom in / out using <kbd>Ctrl</kbd> + <kbd>Mouse wheel</kbd>.

### Editing map tiles 

1. Select one or more tiles by clicking on tiles on the right side. Press <kbd>Ctrl-C</kbd>.
2. Select one or more tiles on the left side. Press <kbd>Ctrl-V</kbd>.

You can undo or redo changes with <kbd>Ctrl-Z</kbd> and <kbd>Ctrl-Y</kbd>.

### Editing events

Events allows to update map tiles depending current game progress (eg: paths, lake, ...).

1. Press <kbd>E</kbd> to activate event mode.
2. Select an event by pressing <kbd>Page-Up</kbd> / <kbd>Page-Down</kbd>.
3. Select event step by pressing <kbd>Home</kbd> / <kbd>End</kbd>.
4. To add a new event, paste a new tile in left side. Event will be added at current step position.
5. To remove an event, right-click on that event.

- Tiles in amber are tiles related to current event.
- Tiles in cyan are tiles of current event already applied.
- Tile 255 (which is rightmost tile of the middle row) cannot be used in events because 255 is a special value used as a marker. It's a limitation of the game engine.

### Editing paths

| Key | Description |
| :-: | - |
| <kbd>P</kbd> | Activate path mode.
| <kbd>Page-Up</kbd> / <kbd>Page-Down</kbd> | Select a level.
| <kbd>Ctrl</kbd> + arrow keys | Move a level. Position will be aligned on a grid.
| arrows keys | Select a direction. There is 4 possible directions. If there is a path in chosen direction, last path segment will be automatically highlighted.
| <kbd>Shift</kbd> + arrow keys | Add a new path segment in that direction. 
| <kbd>Delete</kbd>  | Delete current path segment.
| <kbd>Shift</kbd> + <kbd>Delete</kbd>  | Delete all paths in all directions.
| <kbd>I</kbd>  | Enable / disable hidden mode. Next path segment will be set as hidden (eg: player won't be visible when walking on that path segment).
| <kbd>R</kbd> | Set progress required to take that path. A direction must be selected. A number is shown near level to indicate progression required. Nothing means no progress is required.
| <kbd>T</kbd> | Set end of path exit target. A = Overworld. B = Sherbet Land. C = Mt Teapot.

Tips for paths : 

- A path must always end with a level or an exit. If path target is a level, last segment must end up exactly at same position as level center to be connected (not just collide with it). If paths does not work as expected, make sure last segment ends up at right place. It might be necessary to move level to align it on the grid.
- It's possible to put an exit outside screen boundaries (to create the illusion it take some time to get there).
- It might be a good idea to clear all events and paths before starting a new map.
- Events should be edited to match paths. If you don't want to bother with events, remove all events and hardcode paths in map.
