# WLEditor

This a level editor for Super Mario Land 3 / WarioLand 1 (1993).

## Features
- edit level blocks.
- edit object data (enemies / power-ups)
- rom expansion (for level blocks only)
- edit level headers / sectors / warps 
- block copy / paste selection tool

## How to use 
Click on "File" > "Load". Select a valid WL1 rom.
Then, select a course in the combobox.

### Navigating through levels
Each level is made of 2 x 16 = 32 sectors.

Some sectors will warp player to another sector (or the exit) once a door is used. It will also load new tile graphics at the same time.

In the editor, near the sector number, a label "W.." indicate if sector will warp player to another sector.

<kbd>Middle-click</kbd> on a sector to select it. If selected sector is associated with a warp (usually there is a door in it), it will load related tiles.

### Editing level blocks
Toolbox must be active (go to "View" > "Toolbox" or press <kbd>F1</kbd>). Select "16x16 Tiles" view.
- Select a tile in the toolbox by clicking on it.
- Then, click inside level view. 

### Editing objects (enemies / power-ups)
Toolbox must be active. Select "Objects" view.
- Select an enemy or a power up in the toolbox by clicking on it. 
- Then, click inside level view. 

<kbd>Right-click</kbd> can be used to delete objects. Available enemies / objects depends of current sector / warp.


### Editing sectors / warps
Toolbox must be active. Select "Sector" view.
- <kbd>Middle-click</kbd> on a sector to select it.
- Change sector properties in toolbox.

It's only possible to assign a warp to a sector if that warp is not used by any other sector.
To do so, select a sector that has a warp (eg: S04), set warp to "none". Then you can use that warp in another sector (eg: S01).

Enemy sets specify which enemies will be loaded for a given warp (up to 6 enemies loaded). It might also execute some specific code.
(eg: terminate level if boss is already beaten, open skull if treasure is already collected, ...).

### Copy / paste a selection
This is a very powerful feature. Toolbox must be inactive. 
1. Left click on a block, hold mouse button and drag cursor to create a selection (eg: a 3x3 block). Release mouse button.
2. Press <kbd>Ctrl-C</kbd> to copy block.
3. Left click somewhere else (eg: a 1x1 block is selected).
4. Press <kbd>Ctrl-V</kbd> to paste block at that position. 

It's possible to repeat a selection by creating a selection bigger than 1x1 in step 3.