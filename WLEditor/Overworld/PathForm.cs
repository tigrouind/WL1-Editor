using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace WLEditor
{
	public class PathForm
	{
		WorldPath[] worldData;	
		WorldPath[] overWorldData;

		int currentPath;
		int currentDirection = -1;
		bool invisiblePath;		

		int zoom;		
		int currentWorld;
		readonly PictureBox pictureBox;
				
		readonly int[][] levels = new int[][]
		{
			new int[] { 7, 15, 14, 12, 25, 41 },
			new int[] { 7, 15, 14, 12, 25, 41 },
			new int[] { 6, 16, 13, 5, 17, 9 },
			new int[] { 33, 2, 4, 8, 32, 24 },
			new int[] { 3, 21, 22, 39, 27, 28 },
			new int[] { 0, 30, 31, 11, 20 },
			new int[] { 38, 29, 1, 19, 18, 26 },
			new int[] { 37, 34, 35, 40 },
			new int[] { 0, 1, 2, 3, 4, 5, 6, 7 }
		};
		
		readonly int[][][] startPositionData = new int[][][]
		{
			new int[][] { new int[] { 0x5558, 0x6075, 0 } },
			new int[][] { new int[] { 0x5558, 0x6075, 0 } },
			new int[][] { new int[] { 0x554C, 0x0000, 0 }, new int[] { 0x5552, 0x6095, 1 } },
			new int[][] { new int[] { 0x5527, 0x608D, 0 } },
			new int[][] { new int[] { 0x553F, 0x607D, 0 } },
			new int[][] { new int[] { 0x552D, 0x6089, 0 } },
			new int[][] { new int[] { 0x5533, 0x6085, 0 } },
			new int[][] { new int[] { 0x5539, 0x6081, 0 } }
		};
		
		int[][] flagsPosition = 
		{
			new int[] { 0x56F5, 0x56E8 },
			new int[] { 0x5719, 0x570C },
			new int[] { 0x5738, 0x572B },
			new int[] { 0x575C, 0x574F },
			new int[] { 0x5780, 0x5773 },
			new int[] { 0x57A4, 0x5797 },
			new int[] { 0x57C3, 0x57B6 },
		};
		
		public event EventHandler PathChanged;
		
		public PathForm(PictureBox box)
		{
			pictureBox = box;
		}
		
		void SetChanges()
		{
			PathChanged(this, EventArgs.Empty);
		}
		
		WorldPath[] pathData
		{
			get
			{
				return currentWorld == 8 ? overWorldData : worldData;
			}
		}
		
		public bool SavePaths(Rom rom, out string errorMessage)
		{
			bool result = Map.SaveWorldPaths(rom, pathData, currentWorld == 8, out errorMessage);
			
			if (result)
			{			
				if(currentWorld != 8)
				{
					SaveStartPosition(rom);
				}
				else
				{
					Map.SaveFlagsPosition(rom, pathData, flagsPosition);
				}
			}
			
			return result;
		}
		
		public void SetZoom(int zoomLevel)
		{
			zoom = zoomLevel;
		}
		
		public void LoadPaths(Rom rom)
		{
			worldData = Map.LoadWorldPaths(rom, false);
			overWorldData = Map.LoadWorldPaths(rom, true);
			Map.ReadFlagsPosition(rom, overWorldData, flagsPosition);
		}
		
		public void LoadWorld(int world)
		{			
			currentWorld = world;
			currentPath = 0;
			currentDirection = -1;
		}
		
		#region Commands
		
		void AddPath(Keys key)
		{		
			int level = levels[currentWorld][currentPath];
		
			//set direction if needed
			if (currentDirection == -1)
			{							
				currentDirection = GetDirection(key);
			}
			
			var dir = pathData[level].Directions[currentDirection];	
			if (dir.Next != 0xFA && dir.Next != 0xF9 && dir.Next != 0xF8)
			{
				UnbindPath(dir);
			}
			
			//check previous step
			int newDir = GetDirection(key);
			int previousDir = dir.Path.Count > 0 ? dir.Path.Last().Direction : -1;

			if (previousDir == newDir && (dir.Path.Last().Status == 14) == invisiblePath && dir.Path.Last().Steps < (256 - gridSnap))
			{							
				dir.Path.Last().Steps += gridSnap;
			}						
			else if ((previousDir == 0 && newDir == 1) || (previousDir == 1 && newDir == 0) || //opposite direction
			         (previousDir == 2 && newDir == 3) || (previousDir == 3 && newDir == 2)) 
			{				
				if (dir.Path.Last().Steps > gridSnap)
				{
					dir.Path.Last().Steps -= gridSnap;
				}
				else
				{
					RemovePath();
				}
			}
			else
			{
				dir.Path.Add(new WorldPathSegment 
             	{
	             	Direction = newDir, 
	             	Status = invisiblePath ? 14 : newDir + 1,
	             	Steps = gridSnap 
				});
			}
		}
		
		void RemovePath()
		{
			int level = levels[currentWorld][currentPath];
			var dir = pathData[level].Directions[currentDirection];
						
			dir.Path.RemoveAt(dir.Path.Count - 1);
			if (dir.Path.Count == 0)
			{
				currentDirection = -1;
				UnbindPath(dir);
				dir.Progress = 0xFD;
			}					
			else if (dir.Next != 0xFA && dir.Next != 0xF9 && dir.Next != 0xF8)
			{
				UnbindPath(dir);
			}
		}
		
		void RemoveAllPaths()
		{
			int level = levels[currentWorld][currentPath];
			foreach(var dir in pathData[level].Directions)
			{
				dir.Path.Clear();
				UnbindPath(dir);
				dir.Progress = 0xFD;
			}
		}
		
		void MoveLevel(Keys key)
		{
			//align on grid
			int currentLevel = levels[currentWorld][currentPath];
			var item = pathData[currentLevel];
			int posX = item.X / gridSnap * gridSnap;
			int posY = item.Y / gridSnap * gridSnap;
			
			switch(key)
			{
				case Keys.Up:
					item.Y = Math.Max(posY - gridSnap, 0);
					break;
					
				case Keys.Down:
					item.Y = Math.Min(posY + gridSnap, currentMapY - 8);
					break;
					
				case Keys.Left:
					item.X = Math.Max(posX - gridSnap, 0);
					break;
					
				case Keys.Right:
					item.X = Math.Min(posX + gridSnap, currentMapX - 8);
					break;
			}
			
			//update flag position
			if (currentWorld == 8)
			{
				item.FlagX = item.X + 20;
				item.FlagY = item.Y;
			}
			
			foreach (var dir in pathData[currentLevel].Directions)
			{
				if (dir.Next != 0xFA && dir.Next != 0xF9 && dir.Next != 0xF8)
				{
					UnbindPath(dir);
				}
			}

			//unbind paths linked to that level
			foreach (int level in levels[currentWorld])
			{
				foreach(var dir in pathData[level].Directions)
				{
					if (dir.Next == currentLevel)
					{
						UnbindPath(dir);
					}
				}
			}
		}
		
		void SetExit()
		{
			int level = levels[currentWorld][currentPath];
			var dir = pathData[level].Directions[currentDirection];	
			if (currentWorld == 8)
			{
				dir.Next = 0xF8;
			}
			else
			{
				int[] exits = { 0xFD, 0xFA, 0xF9 };
				int next = Array.IndexOf(exits, dir.Next);
				dir.Next = exits[(next + 1) % exits.Length];
			}		
		}
		
		void SetProgress()
		{
			int level = levels[currentWorld][currentPath];
			var dir = pathData[level].Directions[currentDirection];
				
			int[] flags = { 0xFD, 1, 2, 4, 8, 16, 32 };
			int progress = Array.IndexOf(flags, dir.Progress);			
			dir.Progress = flags[(progress + 1) % flags.Length];
		}
		
		void ChangeDirection(Keys key)
		{
			int newDir = GetDirection(key);
			int level = levels[currentWorld][currentPath];
			if (pathData[level].Directions[newDir].Path.Count > 0)
			{
				currentDirection = newDir;							
			}
			else
			{
				currentDirection = -1;
			}
		}
					
		public bool ProcessPathKey(Keys key)
		{	
			bool shift = (key & Keys.Shift) != 0;
			bool control = (key & Keys.Control) != 0;
			
			key = key & Keys.KeyCode;
						
			switch(key)
			{
				case Keys.PageUp:
					if (currentPath < levels[currentWorld].Length - 1)
					{
						currentPath++;
						currentDirection = -1;
						pictureBox.Invalidate();
					}
					return true;
					
				case Keys.PageDown:
					if (currentPath > 0)
					{
						currentPath--;
						currentDirection = -1;
						pictureBox.Invalidate();
					}
					return true;
					
				case Keys.Home:
				case Keys.End:
					return true;
						
				case Keys.Delete:
					if (shift)
					{
						RemoveAllPaths();
						
						pictureBox.Invalidate();
						SetChanges();						
					}
					else if (currentDirection != -1)
					{
						RemovePath();
						BindPaths();						
												
						pictureBox.Invalidate();
						SetChanges();						
					}				
					return true;
										
				case Keys.Up:
				case Keys.Down:
				case Keys.Left:
				case Keys.Right:
					if (control)
					{
						MoveLevel(key);
						BindPaths();
						
						pictureBox.Invalidate();
						SetChanges();
					}
					if (shift)
					{		
						AddPath(key);
						BindPaths();						
						
						pictureBox.Invalidate();
						SetChanges();
					}
					else
					{	
						ChangeDirection(key);						
						pictureBox.Invalidate();
					}					
					return true;
					
				case Keys.I:
					invisiblePath = !invisiblePath;
					return true;
					
				case Keys.T:
					if (currentDirection != -1)
					{
						SetExit();
						
						pictureBox.Invalidate();
						SetChanges();
					}
					return true;
				
				case Keys.R:
					if (currentDirection != -1)
					{
						SetProgress();
						
						pictureBox.Invalidate();
						SetChanges();
					}
					return true;
			}
						
			return false;
		}
				
		int GetDirection(Keys key)
		{
			switch(key)
			{
				case Keys.Right:
					return 0;
					
				case Keys.Left:
					return 1;
				
				case Keys.Up:
					return 2;
					
				case Keys.Down:
					return 3;
			}
			
			return -1;
		}
		
		#endregion
		
		#region Draw
		
		public void DrawLevels(Graphics g)
		{			
			using (var brush = new SolidBrush(Color.FromArgb(255, 64, 192, 192)))
			using (var format = new StringFormat())
			using (var font = new Font("Arial", 5.0f * zoom))			
			{								
				format.LineAlignment = StringAlignment.Center;
				format.Alignment = StringAlignment.Center;

				int currentLevel = levels[currentWorld][currentPath];
				
				for (int i = 0 ; i < levels[currentWorld].Length ; i++)
				{
					var level = levels[currentWorld][i];
					var item = pathData[level];
					int posX = item.X;
					int posY = item.Y;
										
					bool connected = false;	
					foreach(var dir in pathData[currentLevel].Directions.Where(x => x.Path.Count > 0))
					{
						connected |= dir.Next == level;
					}
					
					bool selected = currentPath == i;
					g.FillRectangle(selected ? brush : Brushes.Blue, posX * zoom, posY * zoom, 8 * zoom, 8 * zoom);
					g.DrawString((i + 1).ToString(), font, connected ? brush : Brushes.White, (posX + 4) * zoom, (posY + 4) * zoom, format);
				}
			}
		}
		
		public void DrawExits(Graphics g)
		{
			using (var format = new StringFormat())
			using (var font = new Font("Arial", 5.0f * zoom))			
			{
				format.LineAlignment = StringAlignment.Center;
				format.Alignment = StringAlignment.Center;
				
				int level = levels[currentWorld][currentPath];
				var item = pathData[level];
				foreach (var dir in item.Directions.Where(x => x.Path.Count > 0 && (x.Next == 0xFD || x.Next == 0xFA || x.Next == 0xF9 || x.Next == 0xF8)))
				{			
					int nextX, nextY;
					GetPathPosition(item, dir, out nextX, out nextY);	
					
					int exitX = Math.Max(0, Math.Min(nextX, currentMapX - 8));
                 	int exitY = Math.Max(0, Math.Min(nextY, currentMapY - 8));
                 	int[] flags = { 0xFD, 0xFA, 0xF9, 0xF8 };
					
					g.FillRectangle(Brushes.Green, exitX * zoom, exitY * zoom, 8 * zoom, 8 * zoom);
					g.DrawString(new [] { "A", "B", "C", "C" } [Array.IndexOf(flags, dir.Next)], font, Brushes.White, (exitX + 4) * zoom, (exitY + 4) * zoom, format);
				}
			}
		}
			
		public void DrawPaths(Graphics g)
		{			
			using (var penDefault = new Pen(Color.Blue, zoom * 2.0f))	
			using (var penSelected = new Pen(Color.FromArgb(255, 64, 192, 192), zoom * 2.0f))				
			using (var graphicPath = new GraphicsPath())
			{								
				int level = levels[currentWorld][currentPath];
				var item = pathData[level];
						
				foreach (var dirs in item.Directions.Where(x => x.Path.Count > 0))
				{					
					int startX = item.X;
					int startY = item.Y;
					bool selected = currentDirection != -1 && item.Directions[currentDirection] == dirs;
										
					foreach (var groupedPaths in dirs.Path.GroupByAdjacent(x => x.Status == 14, (x, y) => new { Key = x, Items = y } ))
					{	
						graphicPath.Reset();
						foreach (var path in groupedPaths.Items)
						{			
							int nextX = startX;
							int nextY = startY;
							
							GetPathPosition(path, ref nextX, ref nextY);
							graphicPath.AddLine((startX + 4) * zoom, (startY + 4) * zoom, (nextX + 4) * zoom, (nextY + 4) * zoom);
							
							startX = nextX;
							startY = nextY;
						}
						
						Pen pen = selected ? penSelected : penDefault;
						pen.DashStyle = groupedPaths.Key ? DashStyle.Dot : DashStyle.Solid;
						g.DrawPath(pen, graphicPath);
					}					
				}
			}
		}
			
		public void DrawProgress(Graphics g)
		{
			using (var format = new StringFormat())
			using (var font = new Font("Arial", 5.0f * zoom))					
			{				
				format.LineAlignment = StringAlignment.Center;
				format.Alignment = StringAlignment.Center;
				
				int level = levels[currentWorld][currentPath];
				var item = pathData[level];
				
				int[][] offsets =
				{
					new int[] {  8,  0 },
					new int[] { -8,  0 },
					new int[] {  0, -8 },
					new int[] {  0,  8 }
				};
								
				for (int dir = 0 ; dir < 4; dir++)
				{					
					var dirs = item.Directions[dir];					
					if (dirs.Path.Count > 0 && dirs.Progress != 0xFD)
					{
						int[] progressFlags = { 0, 1, 2, 4, 8, 16, 32 };
						int progress = Array.IndexOf(progressFlags, dirs.Progress);
						
						var offset = offsets[dir];
						g.FillRectangle(Brushes.Gray, (item.X + offset[0]) * zoom, (item.Y + offset[1]) * zoom, 8 * zoom, 8 * zoom);
						g.DrawString(progress.ToString(), font, Brushes.White, (item.X + 4 + offset[0]) * zoom, (item.Y + 4 + offset[1]) * zoom, format);
					}
				}
			}
		}
		
		#endregion
		
		void SaveStartPosition(Rom rom)
		{
			foreach (var pos in startPositionData[currentWorld])
			{
				int posX, posY;
				if (FindExitPosition(pos[2], out posX, out posY))
				{
					Map.SaveWorldStartPosition(rom, posX, posY, FindClosestSide(posX, posY), pos[0], pos[1]);
				}
			}
		}
		
		int FindClosestSide(int x, int y)
		{
			int[] borders = { x, 160 - x, y, 144 - y }; 			
			int bestSide = -1, min = int.MaxValue;
			
			for (int i = 0 ; i < borders.Length ; i++)
			{						
				if (borders[i] < min)
				{
					min = borders[i];
					bestSide = i;
				}						
			}
			
			return bestSide;
		}		
		
		bool FindExitPosition(int startLevel, out int posX, out int posY)
		{
			int level = levels[currentWorld][startLevel];
			
			var item = pathData[level];				
			foreach(var dir in item.Directions
			        .Where(x => x.Path.Count > 0 && (x.Next == 0xFD || x.Next == 0xFA || x.Next == 0xF9 || x.Next == 0xF8)))
			{
				GetPathPosition(item, dir, out posX, out posY);					
				return true;
			}
			
			posX = -1;
			posY = -1;
			return false;
		}
		
		void BindPaths()
		{
			var levelPositions = new Dictionary<int, int>();
			foreach(var level in levels[currentWorld])
			{
				levelPositions[pathData[level].X + pathData[level].Y * 256] = level;
			}
			
			foreach(var level in levels[currentWorld])
			{
				var item = pathData[level];
				foreach(var dir in item.Directions.Where(x => x.Path.Count > 0))
				{
					int posX, posY;
					GetPathPosition(item, dir, out posX, out posY);
					
					int nextLevel;
					if (levelPositions.TryGetValue(posX + posY * 256, out nextLevel))
					{
						dir.Next = nextLevel;
					}
				}
			}
		}
		
		void UnbindPath(WorldPathDirection dir)
		{
			dir.Next = currentWorld == 8 ? 0xF8 : 0xFD;
		}
		
		void GetPathPosition(WorldPath level, WorldPathDirection direction, out int posX, out int posY)
		{
			posX = level.X;
			posY = level.Y;
			
			foreach(var path in direction.Path)
			{
				GetPathPosition(path, ref posX, ref posY);
			}
		}
		
		void GetPathPosition(WorldPathSegment path, ref int posX, ref int posY)
		{
			switch (path.Direction)
			{
				case 0:
					posX += path.Steps;
					break;
					
				case 1:
					posX -= path.Steps;
					break;
					
				case 2:
					posY -= path.Steps;
					break;
					
				case 3:
					posY += path.Steps;
					break;														
			}
		}
		
		int currentMapX
		{
			get
			{
				return currentWorld == 8 ? 256 : 160;
			}
		}
		
		int currentMapY
		{
			get
			{
				return currentWorld == 8 ? 256 : 144;
			}			
		}
		
		int gridSnap
		{
			get
			{
				return currentWorld == 8 ? 4 : 8;
			}
		}
	}
}
