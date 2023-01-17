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

		WorldPath currentPath;
		WorldPathDirection currentDirection;
		int currentLevel;
		
		int pathMode;		
		const int gridSnap = 4;

		int zoom;		
		int currentWorld;
		readonly PictureBox pictureBox;
				
		readonly int[][] levels = new int[][]
		{
			new int[] { 7, 15, 14, 12, 25, 41 },
			new int[] { 7, 15, 14, 12, 25, 41 },
			new int[] { 6, 16, 13, 5, 17, 9 },
			new int[] { 3, 21, 22, 39, 27, 28 },
			new int[] { 0, 30, 31, 11, 20 },
			new int[] { 38, 29, 1, 19, 18, 26 },
			new int[] { 33, 2, 4, 8, 32, 24 },
			new int[] { 37, 34, 35, 40 },
			new int[] { 0, 1, 2, 4, 3, 5, 6, 7 }
		};
		
		readonly int[][][] startPositionData = new int[][][]
		{
			new int[][] { new int[] { 0x5558, 0x6075, 0 } },
			new int[][] { new int[] { 0x5558, 0x6075, 0 } },
			new int[][] { new int[] { 0x554C, 0x0000, 0 }, new int[] { 0x5552, 0x6095, 1 } },
			new int[][] { new int[] { 0x553F, 0x607D, 0 } },
			new int[][] { new int[] { 0x552D, 0x6089, 0 } },
			new int[][] { new int[] { 0x5533, 0x6085, 0 } },
			new int[][] { new int[] { 0x5527, 0x608D, 0 } },
			new int[][] { new int[] { 0x5539, 0x6081, 0 } }
		};
		
		readonly Color[] pathColors = { Color.Lime, Color.Red, Color.Blue, Color.MediumSeaGreen, Color.Brown, Color.SteelBlue };
		
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
			bool result = Map.SavePaths(rom, pathData, currentWorld == 8, out errorMessage);
			
			if (result && currentWorld != 8)
			{			
				SaveStartPosition(rom);
			}
			
			return result;
		}
		
		public void SetZoom(int zoomLevel)
		{
			zoom = zoomLevel;
		}
		
		public void LoadPaths(Rom rom)
		{
			worldData = Map.LoadPaths(rom, false);
			overWorldData = Map.LoadPaths(rom, true);
		}
		
		public void LoadWorld(int world)
		{			
			currentWorld = world;
			currentLevel = levels[currentWorld][0];
			currentPath = pathData[currentLevel];
			currentDirection = null;
		}
		
		#region Commands
		
		void AddPath(Keys key)
		{		
			//set direction if needed
			if (currentDirection == null)
			{							
				currentDirection = currentPath.Directions[GetDirection(key)];
			}
			
			UnbindPath(currentDirection);
			
			//check previous step
			int newDir = GetDirection(key);
			int previousDir = currentDirection.Path.Count > 0 ? currentDirection.Path.Last().Direction : -1;

			if (previousDir == newDir && GroupPath(currentDirection.Path.Last().Status) == pathMode && currentDirection.Path.Last().Steps < (256 - gridSnap))
			{							
				currentDirection.Path.Last().Steps += gridSnap;
			}						
			else if (previousDir == GetReverseDir(newDir))
			{				
				if (currentDirection.Path.Last().Steps > gridSnap)
				{
					currentDirection.Path.Last().Steps -= gridSnap;
				}
				else
				{
					RemovePath();
				}
			}
			else
			{
				currentDirection.Path.Add(new WorldPathSegment 
             	{
	             	Direction = newDir, 
	             	Status = GetStatus(newDir, new [] { 0, 14, 12 }[pathMode]),
	             	Steps = gridSnap 
				});
			}
		}
		
		void RemovePath()
		{
			UnbindPath(currentDirection);
			currentDirection.Path.RemoveAt(currentDirection.Path.Count - 1);
			
			if (currentDirection.Path.Count == 0)
			{
				currentDirection.Progress = 0xFD;
				currentDirection = null;				
				pathMode = 0;
			}					
			else 
			{
				pathMode = GroupPath(currentDirection.Path.Last().Status);
			}
		}
		
		void RemoveAllPaths()
		{
			foreach(var dir in currentPath.Directions)
			{
				UnbindPath(dir);
				dir.Path.Clear();
				dir.Progress = 0xFD;
			}
			
			currentDirection = null;
			pathMode = 0;
		}
		
		void MoveLevel(Keys key)
		{
			//align on grid		
			int posX = currentPath.X / gridSnap * gridSnap;
			int posY = currentPath.Y / gridSnap * gridSnap;
			
			switch(key)
			{
				case Keys.Up:
					currentPath.Y = Math.Max(posY - gridSnap, 0);
					break;
					
				case Keys.Down:
					currentPath.Y = Math.Min(posY + gridSnap, currentMapY - 8);
					break;
					
				case Keys.Left:
					currentPath.X = Math.Max(posX - gridSnap, 0);
					break;
					
				case Keys.Right:
					currentPath.X = Math.Min(posX + gridSnap, currentMapX - 8);
					break;
			}
			
			if (currentWorld == 8)
			{
				currentPath.FlagX = currentPath.X + 20;
				currentPath.FlagY = currentPath.Y;
			}
			else
			{
				currentPath.TreasureX = currentPath.X + 4;
				currentPath.TreasureY = currentPath.Y + 4;
			}
			
			foreach (var dir in currentPath.Directions)
			{
				UnbindPath(dir);
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
			RemoveReversePath(currentDirection);
			if (currentWorld == 8)
			{
				currentDirection.Next = 0xF8;
			}
			else
			{
				int[] exits = { 0xFD, 0xFA, 0xF9 };
				int next = Array.IndexOf(exits, currentDirection.Next);
				currentDirection.Next = exits[(next + 1) % exits.Length];
			}		
		}
		
		void SetProgress()
		{			
			int[] flags = { 0xFD, 1, 2, 4, 8, 16, 32 };
			int progress = Array.IndexOf(flags, currentDirection.Progress);			
			currentDirection.Progress = flags[(progress + 1) % flags.Length];			
			var reverseDir = GetReverseDir(currentDirection);
			if (reverseDir != null)
			{
				reverseDir.Progress = currentDirection.Progress;			
			}
		}
		
		void ChangeDirection(Keys key)
		{
			var newDir = GetDirection(key);
			currentDirection = currentPath.Directions[newDir];
			
			if (currentDirection.Path.Count > 0)
			{
				pathMode = GroupPath(currentDirection.Path.Last().Status);
			}
			else
			{
				currentDirection = null;
				pathMode = 0;
			}
		}
		
		void NextLevel()
		{
			int level = Array.IndexOf(levels[currentWorld], currentLevel);
			if (level < levels[currentWorld].Length - 1)
			{		
				currentDirection = null;				
				currentLevel = levels[currentWorld][level + 1];
				currentPath = pathData[currentLevel];
				
				pathMode = 0;
				pictureBox.Invalidate();
			}
		}
		
		void PreviousLevel()
		{
			int level = Array.IndexOf(levels[currentWorld], currentLevel);
			if (level > 0)
			{
				currentDirection = null;
				currentLevel = levels[currentWorld][level - 1];
				currentPath = pathData[currentLevel];
				
				pathMode = 0;
				pictureBox.Invalidate();
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
					NextLevel();
					return true;
					
				case Keys.PageDown:
					PreviousLevel();
					return true;
						
				case Keys.Delete:
					if (shift)
					{
						RemoveAllPaths();
						
						pictureBox.Invalidate();
						SetChanges();						
					}
					else if (currentDirection != null)
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
					else if (shift)
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
					pathMode = pathMode == 1 ? 0 : 1;
					return true;
					
				case Keys.W:
					pathMode = pathMode == 2 ? 0 : 2;
					return true;
					
				case Keys.X:
					if (currentDirection != null)
					{
						SetExit();
						
						pictureBox.Invalidate();
						SetChanges();
					}
					return true;
				
				case Keys.F:
					if (currentDirection != null)
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
		
		int GetStatus(int newDir, int status)
		{
			switch (status)
			{
				case 14: //invisible
					return 14;
					
				case 12: //water
				case 13: //water
					if (newDir == 2) //up
					{
						return 13; //water back
					}					
					return 12; //water front
				
				default:
					return newDir + 1;
			}
		}
		
		#endregion
		
		#region Draw
		
		public void Draw(Graphics g)
		{
			DrawPaths(g);
			DrawProgress(g);
			DrawLevels(g);
			DrawExits(g);
		}
		
		void DrawLevels(Graphics g)
		{			
			using (var format = new StringFormat())
			using (var font = new Font("Arial", 5.0f * zoom))			
			using (var pen = new Pen(Color.Black, zoom * 2.0f))		
			{								
				format.LineAlignment = StringAlignment.Center;
				format.Alignment = StringAlignment.Center;

				int next = currentDirection == null ? -1 : currentDirection.Next;
				
				foreach(var level in levels[currentWorld].OrderBy(x => x == currentLevel))
				{
					var item = pathData[level];
					int posX = item.X;
					int posY = item.Y;
							
					bool selected = currentPath == item;
					bool connected = level == next;
					
					g.DrawEllipse(pen, posX * zoom, posY * zoom, 8 * zoom, 8 * zoom);
					g.FillEllipse(selected || connected ? Brushes.Lime : Brushes.MediumSeaGreen, posX * zoom, posY * zoom, 8 * zoom, 8 * zoom);
					
					g.DrawString((Array.IndexOf(levels[currentWorld], level) + 1).ToString(), font, Brushes.Black, (posX + 4) * zoom, (posY + 4) * zoom, format);
				}
			}
		}
		
		void DrawExits(Graphics g)
		{
			using (var format = new StringFormat())
			using (var font = new Font("Arial", 5.0f * zoom))			
			using (var penBorder = new Pen(Color.Black, zoom * 2.0f))					
			{
				format.LineAlignment = StringAlignment.Center;
				format.Alignment = StringAlignment.Center;
				
				foreach (var dir in currentPath.Directions.Where(x => x.Path.Count > 0 && (x.Next == 0xFD || x.Next == 0xFA || x.Next == 0xF9 || x.Next == 0xF8)))
				{			
					int nextX, nextY;
					GetPathPosition(currentPath, dir, out nextX, out nextY);	
					
					int exitX = Math.Max(0, Math.Min(nextX, currentMapX - 8));
                 	int exitY = Math.Max(0, Math.Min(nextY, currentMapY - 8));
                 	int[] flags = { 0xFD, 0xFA, 0xF9, 0xF8 };
					
                 	bool selected = currentDirection == dir;
					g.DrawEllipse(penBorder, exitX * zoom, exitY * zoom, 8 * zoom, 8 * zoom);
					g.FillEllipse(selected ? Brushes.Lime : Brushes.MediumSeaGreen, exitX * zoom, exitY * zoom, 8 * zoom, 8 * zoom);
					g.DrawString(new [] { "A", "B", "C", "C" } [Array.IndexOf(flags, dir.Next)], font, Brushes.Black, (exitX + 4) * zoom, (exitY + 4) * zoom, format);
				}
			}
		}
				
		void DrawPaths(Graphics g)
		{							
			foreach (var dirs in currentPath.Directions.Where(x => x.Path.Count > 0).OrderBy(x => currentDirection == x))
			{					
				int startX = currentPath.X;
				int startY = currentPath.Y;								
				bool selected = currentDirection == dirs;
				
				List<Point> points = new List<Point>();
				List<Color> colors = new List<Color>();

				points.Add(new Point((startX + 4) * zoom, (startY + 4) * zoom));
				foreach (var path in dirs.Path)
				{								
					GetPathPosition(path, ref startX, ref startY);
					points.Add(new Point((startX + 4) * zoom, (startY + 4) * zoom));				
					
					Color color = pathColors[GroupPath(path.Status) + (selected ? 0 : 3)];
					colors.Add(color);
				}	
				
				DrawLine(g, points, colors);					
			}
		}
			
		void DrawLine(Graphics g, List<Point> points, List<Color> colors)
		{			
			using (var pen = new Pen(Color.Black, zoom * 5.0f))
			{
				g.DrawLines(pen, points.ToArray());
			}
			
			using (var brush = new SolidBrush(Color.Black))
			{
				var miterPoints = GetMitterPoints(points, zoom * 3.0f);			
			
				int n = 0;
				foreach(var item in colors.GroupByAdjacent(x => x, (x, y) => new { Color = x, Count = y.Count() } ))
				{
					brush.Color = item.Color;
					g.FillPolygon(brush, GetLineStrip(miterPoints, n, item.Count + 1).ToArray(), FillMode.Winding);
					n += item.Count;
				}
			}
		}
		
		List<PointF> GetMitterPoints(List<Point> points, float thickness)
		{
			var result = new List<PointF>();
			for (int i = 0 ; i < points.Count ; i++)
			{						
				Func<PointF> getLineA = () => new PointF(points[i].X - points[i - 1].X, points[i].Y - points[i - 1].Y).Normalized();
				Func<PointF> getLineB = () => new PointF(points[i + 1].X - points[i].X, points[i + 1].Y - points[i].Y).Normalized();
				
				PointF lineA = (i == 0) ? getLineB() : getLineA();
				PointF lineB = (i == points.Count - 1) ? getLineA() : getLineB();
				
				PointF normal = new PointF(-lineA.Y, lineA.X).Normalized();
				PointF tangent = new PointF(lineA.X + lineB.X, lineA.Y + lineB.Y).Normalized();
				PointF miter = new PointF(-tangent.Y, tangent.X);
				
				float length = (thickness * 0.5f) / (normal.X * miter.X + normal.Y * miter.Y);
				result.Add(new PointF(points[i].X + miter.X * length, points[i].Y + miter.Y * length));
				result.Add(new PointF(points[i].X - miter.X * length, points[i].Y - miter.Y * length));
			}
			
			return result;
		}
		
		IEnumerable<PointF> GetLineStrip(List<PointF> points, int index, int count)
		{
			for (int i = 0 ; i < count ; i++)
			{
				yield return points[(index + i) * 2];				
			}
			
			for (int i = count - 1 ; i >= 0 ; i--)
			{
				yield return points[(index + i) * 2 + 1];				
			}
		}
		
		void DrawProgress(Graphics g)
		{
			using (var format = new StringFormat())
			using (var font = new Font("Arial", 5.0f * zoom))					
			{				
				format.LineAlignment = StringAlignment.Center;
				format.Alignment = StringAlignment.Center;
				
				int[][] offsets =
				{
					new int[] {  8,  0 },
					new int[] { -8,  0 },
					new int[] {  0, -8 },
					new int[] {  0,  8 }
				};
								
				for (int dir = 0 ; dir < 4; dir++)
				{					
					var dirs = currentPath.Directions[dir];					
					if (dirs.Path.Count > 0 && dirs.Progress != 0xFD)
					{
						int[] progressFlags = { 0, 1, 2, 4, 8, 16, 32 };
						int progress = Array.IndexOf(progressFlags, dirs.Progress);
						
						var offset = offsets[dir];
						g.FillRectangle(Brushes.Gray, (currentPath.X + offset[0]) * zoom, (currentPath.Y + offset[1]) * zoom, 8 * zoom, 8 * zoom);
						g.DrawString(progress.ToString(), font, Brushes.White, (currentPath.X + 4 + offset[0]) * zoom, (currentPath.Y + 4 + offset[1]) * zoom, format);
					}
				}
			}
		}
		
		int GroupPath(int status)
		{
			switch (status)
			{				
				case 14: //invisible
					return 1;
					
				case 12: //water front
				case 13: //water back
					return 2;
					
				default:
					return 0;
			}
		}
		
		#endregion
		
		void SaveStartPosition(Rom rom)
		{
			foreach (var pos in startPositionData[currentWorld])
			{
				int posX, posY;
				FindExitPosition(pos[2], out posX, out posY);
				Map.SaveStartPosition(rom, posX, posY, FindClosestSide(posX, posY), pos[0], pos[1]);
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
		
		void FindExitPosition(int startLevel, out int posX, out int posY)
		{
			int level = levels[currentWorld][startLevel];
			
			var item = pathData[level];	
			var dir = item.Directions.FirstOrDefault(x => x.Path.Count > 0 && (x.Next == 0xFD || x.Next == 0xFA || x.Next == 0xF9 || x.Next == 0xF8));
			if (dir != null)
			{
				GetPathPosition(item, dir, out posX, out posY);					
			}
			else
			{			
				posX = item.X;
				posY = item.Y;
			}
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
						if (dir.Next != nextLevel)
						{
							dir.Next = nextLevel;
							CreateReversePath(dir);
						}
					}
				}
			}
		}
		
		void UnbindPath(WorldPathDirection dir)
		{
			if (dir.Path.Count == 0 || (dir.Next != 0xFA && dir.Next != 0xF9 && dir.Next != 0xF8 && dir.Next != 0xFD))
			{
				RemoveReversePath(dir);
				dir.Next = currentWorld == 8 ? 0xF8 : 0xFD;
			}
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
				
		#region Reverse
		
		void CreateReversePath(WorldPathDirection dir)
		{			
			var reverseDir = GetReverseDir(dir);
			if (reverseDir != null && reverseDir != dir)
			{
				reverseDir.Path = dir.Path.AsEnumerable().Reverse()
					.Select(x => new WorldPathSegment
					{
			        	Status = GetStatus(GetReverseDir(x.Direction), x.Status),
			        	Direction = GetReverseDir(x.Direction),
						Steps = x.Steps
			        })
					.ToList();
				
				reverseDir.Next = currentLevel;
				reverseDir.Progress = dir.Progress;
			}
		}								
		
		int GetReverseDir(int dir)
		{
			switch(dir)
			{
				case 0:
					return 1;
				case 1:
					return 0;
				case 2:
					return 3;
				case 3:
					return 2;
			}
			
			return -1;
		}
		
		WorldPathDirection GetReverseDir(WorldPathDirection dir)
		{
			if (dir.Path.Count > 0 && dir.Next != 0xFA && dir.Next != 0xF9 && dir.Next != 0xF8 && dir.Next != 0xFD)
			{
				int reverseDir = GetReverseDir(dir.Path.Last().Direction);
				return pathData[dir.Next].Directions[reverseDir];
			}
			
			return null;
		}
		
		void RemoveReversePath(WorldPathDirection dir)
		{
			var reverseDir = GetReverseDir(dir);
			if (reverseDir != null && reverseDir != dir && dir == GetReverseDir(reverseDir))
			{
				reverseDir.Path.Clear();
			}
		}
		
		#endregion
	}
}
