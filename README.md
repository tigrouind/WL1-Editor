# WLEditor

This a level editor for Super Mario Land 3 / WarioLand 1 (1993).

## Features
- edit level blocks.
- edit object data (enemies/power-ups).
- rom expansion (for level blocks only)
- view sectors/warps
- view scroll boundaries
- switch [!] blocks

## How to use 
Click on "File" > "Load" and select a valid WL1 rom.
Then, select a course in the combobox.

### Navigating through levels
Each level is made of 2 x 16 = 32 sectors.

Some sectors will warp player to another sector (or the exit) once a door is used. It will also load new tile graphics at the same time.

In the editor, near the sector number, a label "W.." indicate if sector will warp player to another sector.

Right-click on a sector to select it. If selected sector is associated with a warp, it will load related tiles.

### Editing level blocks
Toolbox must be active (go to "View" > "Toolbox" or press F1). 
- Select a tile in the toolbox by clicking on it.
- Then, click inside level view. 

You can activate colliders (F5) to view which tiles are blocking player / enemies. 

### Editing objects (enemies / power-ups)
Toolbox must be active. 
- Select an enemy or a power up in the toolbox by clicking on it. 
- Then, click inside level view. 

The "X" sign can be used to delete objects. Available enemies/objects depends of current sector/warp.