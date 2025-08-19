using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;

namespace WLEditor
{
	public class PathForm(PictureBox box) : IDisposable
	{
		WorldPath[] worldData;
		WorldPath[] overWorldData;
		int[] levelToCourseId;
		(int X, int Y)[] flags;

		WorldPath currentPath;
		WorldPathDirection currentDirection;
		int currentLevel;
		bool disposed;

		PathModeEnum pathMode;
		const int gridSnap = 4;
		readonly ImageAttributes transparentImageAttributes = GetTransparentImageAttributes();
		public bool TransparentPath;

		int zoom;
		int currentWorld;
		readonly DirectBitmap tilesFlag = new(16 * 8, 1 * 8);
		int allowedDirection;

		Bitmap pathABitmap, pathBBitmap;
		bool bitmapCache;

		public Func<int> GetAnimationIndex;

		readonly int[][] levels =
		[
			[7, 15, 14, 12, 25, 41],
			[0],
			[6, 16, 13, 5, 17, 9],
			[33, 2, 4, 8, 32, 24],
			[3, 21, 22, 39, 27, 28],
			[0, 30, 31, 11, 20],
			[38, 29, 1, 19, 18, 26],
			[37, 34, 35, 40],
			[0],
			[0, 1, 5, 2, 4, 3, 6, 7]
		];

		readonly Color[] pathColors = [Color.Lime, Color.Red, Color.Blue, Color.MediumSeaGreen, Color.Brown, Color.SteelBlue];

		public event EventHandler PathChanged;

		#region Undo / redo

		readonly Stack<PathHistory> undo = [];
		readonly Stack<PathHistory> redo = [];
		bool stateSaved;

		#endregion

		#region Mouse events

		bool selectedLevel = false, selectedPath = false;
		int selectedFlag = -1;
		(int X, int Y) dragOffset, dragPosition;

		#endregion

		public string GetTitle()
		{
			if (Overworld.IsOverworld(currentWorld))
			{
				return string.Empty;
			}

			return $"Course {levelToCourseId[currentLevel]:D2}";
		}

		void SetChanges()
		{
			PathChanged(this, EventArgs.Empty);
		}

		WorldPath[] PathData => Overworld.IsOverworld(currentWorld) ? overWorldData : worldData;

		public bool SavePaths(Rom rom, out string errorMessage)
		{
			bool result = Overworld.SavePaths(rom, PathData, currentWorld, out errorMessage);
			if (result)
			{
				Overworld.SaveProgressNextDirection(rom, currentWorld, PathData, levels[currentWorld], IsSpecialExit);

				if (Overworld.IsOverworld(currentWorld))
				{
					Overworld.SaveFlags(rom, flags);
				}
				else
				{
					Overworld.SaveTreasures(rom, PathData);
					Overworld.SaveStartPosition(rom, currentWorld, PathData, levels[currentWorld], IsSpecialExit, GetPathPosition);
				}
			}

			return result;
		}

		public void SetZoom(int zoomLevel)
		{
			zoom = zoomLevel;

			pathABitmap?.Dispose();
			pathABitmap = new Bitmap(box.Width, box.Height);

			pathBBitmap?.Dispose();
			pathBBitmap = new Bitmap(box.Width, box.Height);

			bitmapCache = false;
		}

		public void LoadPaths(Rom rom)
		{
			worldData = Overworld.LoadPaths(rom, false);
			overWorldData = Overworld.LoadPaths(rom, true);
			flags = Overworld.LoadFlags(rom);
			Overworld.DumpFlags(rom, tilesFlag);
			levelToCourseId = [.. Level.GetCourseIds(rom).Select(x => x.CourseNo)];
		}

		public void LoadWorld(int world)
		{
			currentWorld = world;
			currentLevel = levels[currentWorld][0];
			currentPath = PathData[currentLevel];
			currentDirection = null;
			pathMode = PathModeEnum.None;
			bitmapCache = false;
			redo.Clear();
			undo.Clear();
		}

		#region Commands

		public void Invalidate()
		{
			bitmapCache = false;
			box.Invalidate();
		}

		public void MouseEvent(MouseEventArgs e, TileEventStatus status)
		{
			switch (status)
			{
				case TileEventStatus.MouseDown:
					if (!MouseDown(e))
					{
						return;
					}
					break;

				case TileEventStatus.MouseMove:
					if (!MouseMove(e))
					{
						return;
					}
					break;

				case TileEventStatus.MouseUp:
					MouseUp();
					break;

				case TileEventStatus.MouseWheel:
					MouseWheel(e);
					break;
			}

			bool MouseDown(MouseEventArgs e)
			{
				dragOffset = (e.Location.X, e.Location.Y);
				stateSaved = false;

				//select flag
				if (Overworld.IsOverworld(currentWorld) && currentLevel != 7)
				{
					selectedFlag = flags.FindIndex(pos => GetDistance(e.X, e.Y, pos.X + 8, pos.Y + 8) <= 8);
					if (selectedFlag != -1 && currentLevel == selectedFlag)
					{
						dragPosition = (flags[selectedFlag].X, flags[selectedFlag].Y);
						return false;
					}
				}

				//check level
				int level = levels[currentWorld]
						.Select(x => (int?)x)
						.FirstOrDefault(x => GetDistance(e.X, e.Y, PathData[x.Value].X + 4, PathData[x.Value].Y + 4) <= 4) ?? -1;

				//check paths
				var dirs = new List<WorldPathDirection>();
				var item = PathData[currentLevel];
				if (e.Button != MouseButtons.Right)
				{
					//get all possible dirs
					foreach (var dir in item.Directions.Where(x => x.Path.Count > 0))
					{
						var endPath = GetPathPosition(item, dir);
						if (GetPathPositions(item, dir).Any(pos => GetDistance(e.X, e.Y, pos.X + 4, pos.Y + 4) <= 2)
							|| GetDistance(e.X, e.Y, endPath.X + 4, endPath.Y + 4) <= 4)
						{
							dirs.Add(dir);
						}
					}
				}

				//select level
				if (level != -1
					&& !dirs.Any(dir => GetPathPosition(item, dir) == (PathData[level].X, PathData[level].Y))) //don't select level if sharing position with current path
				{
					if (level != currentLevel)
					{
						SaveState();
						currentLevel = level;
						currentPath = PathData[currentLevel];
						pathMode = PathModeEnum.None;
						Invalidate();
					}

					if (currentDirection != null)
					{
						currentDirection = null;
						Invalidate();
					}

					allowedDirection = 0;
					dragPosition = (currentPath.X, currentPath.Y);
					selectedLevel = true;
					return false;
				}

				//select path
				if (dirs.Any())
				{
					var dir = dirs.Contains(currentDirection) ? currentDirection : dirs.First(); //only allow dir change if new path is not is the list of possible paths
					if (dir != currentDirection)
					{
						currentDirection = dirs.First();
						pathMode = PathModeEnum.None;
						Invalidate();
					}

					var (X, Y) = GetPathPosition(item, dir);
					if (GetDistance(e.X, e.Y, X + 4, Y + 4) <= 4) //only allow drag if end of path is selected
					{
						allowedDirection = 0;
						dragPosition = GetPathPosition(item, currentDirection);
						selectedPath = true;
					}
					return false;
				}

				//deselect path
				if (currentDirection != null)
				{
					currentDirection = null;
					pathMode = PathModeEnum.None;
					Invalidate();
				}



				return true;

				int GetDistance(int mouseX, int mouseY, int x, int y)
				{
					return Math.Max(
						Math.Abs(Math.Max(0, Math.Min(x, CurrentMapX)) * zoom - mouseX),
						Math.Abs(Math.Max(0, Math.Min(y, CurrentMapY)) * zoom - mouseY)) / zoom;
				}
			}

			bool MouseMove(MouseEventArgs e)
			{
				//move level
				if (selectedLevel && e.Button == MouseButtons.Right)
				{
					(var tilePosX, var tilePosY) = GetSnapPosition(e.X, e.Y);
					(int X, int Y) = (tilePosX * gridSnap, tilePosY * gridSnap);

					if ((X, Y) != (currentPath.X, currentPath.Y))
					{
						SaveStateOnce();
						(currentPath.X, currentPath.Y) = (X, Y);
						MoveLevel();
						BindPaths();
						Invalidate();
						SetChanges();
					}

					return false;
				}

				//add or remove paths
				if (selectedPath || selectedLevel)
				{
					var item = PathData[currentLevel];
					var target = currentDirection == null ? (item.X, item.Y) : GetPathPosition(item, currentDirection);
					(var newPosX, var newPosY) = GetSnapPosition(e.X, e.Y);

					SaveStateOnce();
					AddPaths(newPosX - target.X / gridSnap, newPosY - target.Y / gridSnap);
					BindPaths();
					Invalidate();
					SetChanges();
				}

				//move flag
				if (selectedFlag != -1 && currentLevel == selectedFlag)
				{
					(var tilePosX, var tilePosY) = GetSnapPosition(e.X, e.Y);
					var newPos = (tilePosX * gridSnap, tilePosY * gridSnap);

					if (newPos != flags[selectedFlag])
					{
						SaveStateOnce();
						flags[selectedFlag] = newPos;
						Invalidate();
						SetChanges();
					}
				}

				return true;

				(int x, int y) GetSnapPosition(int x, int y)
				{
					int newPosX = ((x - dragOffset.X) / zoom + dragPosition.X + gridSnap / 2) / gridSnap;
					int newPosY = ((y - dragOffset.Y) / zoom + dragPosition.Y + gridSnap / 2) / gridSnap;
					return (newPosX, newPosY);
				}
			}

			void MouseUp()
			{
				selectedLevel = false;
				selectedPath = false;
				selectedFlag = -1;
			}

			void MouseWheel(MouseEventArgs e)
			{
				if ((Control.ModifierKeys & Keys.Alt) == Keys.Alt)
				{
					//set exit
					if (currentDirection != null && IsSpecialExit(currentDirection.Next))
					{
						SaveState();
						SetExit(Math.Sign(e.Delta));
						Invalidate();
						SetChanges();
					}
				}
				else if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
				{
					//set progress
					if (currentDirection != null)
					{
						SaveState();
						SetProgress(Math.Sign(e.Delta));
						Invalidate();
						SetChanges();
					}
				}
				else
				{
					//set pathmode
					pathMode = (PathModeEnum)(((int)pathMode + 1) % 3);
					Invalidate();
					SetChanges();
				}
			}
		}

		public bool ProcessPathKey(Keys key)
		{
			var keyCode = key & Keys.KeyCode;
			switch (key)
			{
				case Keys.Shift | Keys.Delete:
					if (currentPath.Directions.Any(x => x.Path.Any()))
					{
						SaveState();
						RemoveAllPaths();

						Invalidate();
						SetChanges();
					}
					return true;

				case Keys.Delete:
					if (currentDirection != null && currentDirection.Path.Any())
					{
						SaveState();
						RemovePath();
						BindPaths();

						Invalidate();
						SetChanges();
					}
					return true;

				case Keys.Control | Keys.Z:
					ApplyChanges(undo, redo);
					return true;

				case Keys.Control | Keys.Y:
					ApplyChanges(redo, undo);
					return true;
			}

			void ApplyChanges(Stack<PathHistory> source, Stack<PathHistory> dest)
			{
				if (source.Any())
				{
					var data = source.Pop();
					dest.Push(Serialize());
					Deserialize(data);

					pathMode = PathModeEnum.None;
					currentPath = PathData[currentLevel];
					currentDirection = null;
					Invalidate();
				}
			}

			switch (keyCode)
			{
				case Keys.Up:
					MouseEvent(new MouseEventArgs(MouseButtons.None, 0, 0, 0, 1), TileEventStatus.MouseWheel);
					return true;

				case Keys.Down:
					MouseEvent(new MouseEventArgs(MouseButtons.None, 0, 0, 0, -1), TileEventStatus.MouseWheel);
					return true;
			}

			return false;
		}

		#region Undo/redo

		void SaveStateOnce()
		{
			if (!stateSaved)
			{
				stateSaved = true;
				SaveState();
			}
		}

		void SaveState()
		{
			undo.Push(Serialize());
			redo.Clear();
		}

		PathHistory Serialize()
		{
			var cloner = new Cloner();
			return cloner.Clone(new PathHistory
			{
				WorldData = PathData[currentLevel],
				CurrentLevel = currentLevel,
				Flags = flags
			});
		}

		void Deserialize(PathHistory data)
		{
			PathData[data.CurrentLevel] = data.WorldData;
			currentLevel = data.CurrentLevel;
			flags = data.Flags;
		}

		#endregion

		void SetProgress(int delta)
		{
			int progress = Array.IndexOf(Overworld.ProgressFlags, currentDirection.Progress);
			if (progress != -1)
			{
				currentDirection.Progress = Overworld.ProgressFlags[(progress + delta + Overworld.ProgressFlags.Length) % Overworld.ProgressFlags.Length];
				var reverseDir = GetReverseWorldPathDir(currentDirection);
				if (reverseDir != null)
				{
					reverseDir.Progress = currentDirection.Progress;
				}
			}
		}

		void SetExit(int delta)
		{
			RemoveReversePath(currentDirection);
			if (Overworld.IsOverworld(currentWorld))
			{
				currentDirection.Next = WorldPathNextEnum.TeapotOverworld;
			}
			else
			{
				WorldPathNextEnum[] exits = [WorldPathNextEnum.Overworld, WorldPathNextEnum.Sherbet, WorldPathNextEnum.Teapot];
				int next = Array.IndexOf(exits, currentDirection.Next);
				currentDirection.Next = exits[(next + delta + exits.Length) % exits.Length];
			}
		}

		void MoveLevel()
		{
			foreach (var dir in currentPath.Directions)
			{
				UnbindPath(dir);
			}

			//unbind paths linked to that level
			foreach (int level in levels[currentWorld])
			{
				foreach (var dir in PathData[level].Directions)
				{
					if (dir.Next == (WorldPathNextEnum)currentLevel)
					{
						UnbindPath(dir);
					}
				}
			}
		}

		void AddPaths(int x, int y)
		{
			while ((x, y) != (0, 0))
			{
				WorldPathDirectionEnum direction;
				if (x != 0)
				{
					direction = x > 0 ? WorldPathDirectionEnum.Right : WorldPathDirectionEnum.Left;
					x -= Math.Sign(x);
					if (allowedDirection != 2) //prevent changing direction
					{
						allowedDirection = 1;
						AddPath(direction);
					}
				}
				else
				{
					direction = y > 0 ? WorldPathDirectionEnum.Down : WorldPathDirectionEnum.Up;
					y -= Math.Sign(y);
					if (allowedDirection != 1) //prevent changing direction
					{
						allowedDirection = 2;
						AddPath(direction);
					}
				}
			}
		}

		void AddPath(WorldPathDirectionEnum newDir)
		{
			//set direction if needed
			if (currentDirection == null)
			{
				var dir = currentPath.Directions[(int)newDir];
				if (dir.Path.Any())
				{
					return; //don't allow to change direction to a non empty path
				}

				currentDirection = dir;
			}

			UnbindPath(currentDirection);

			//check previous step
			WorldPathDirectionEnum previousDir = currentDirection.Path.Count > 0 ? currentDirection.Path.Last().Direction : (WorldPathDirectionEnum)(-1);

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
					allowedDirection = 0;
					RemovePath();
				}
			}
			else
			{
				WorldPathStatusEnum status = GetStatus();

				currentDirection.Path.Add(new WorldPathSegment
				{
					Direction = newDir,
					Status = PathForm.GetStatus(newDir, status),
					Steps = gridSnap
				});
			}

			WorldPathStatusEnum GetStatus()
			{
				WorldPathStatusEnum status = WorldPathStatusEnum.None;
				switch (pathMode)
				{
					case PathModeEnum.Invisible:
						status = WorldPathStatusEnum.Invisible;
						break;

					case PathModeEnum.Water:
						status = WorldPathStatusEnum.WaterFront;
						break;
				}

				return status;
			}
		}

		void RemovePath()
		{
			UnbindPath(currentDirection);
			currentDirection.Path.RemoveAt(currentDirection.Path.Count - 1);

			if (currentDirection.Path.Count == 0)
			{
				currentDirection.Progress = WorldPathProgressEnum.None;
				currentDirection = null;
				pathMode = PathModeEnum.None;
			}
			else
			{
				pathMode = GroupPath(currentDirection.Path.Last().Status);
			}
		}

		void RemoveAllPaths()
		{
			foreach (var dir in currentPath.Directions)
			{
				UnbindPath(dir);
				dir.Path.Clear();
				dir.Progress = WorldPathProgressEnum.None;
			}

			currentDirection = null;
			pathMode = PathModeEnum.None;
		}

		void BindPaths()
		{
			var levelPositions = new Dictionary<int, int>();
			foreach (var level in levels[currentWorld])
			{
				levelPositions[PathData[level].X + PathData[level].Y * 256] = level;
			}

			foreach (var level in levels[currentWorld])
			{
				var item = PathData[level];
				foreach (var dir in item.Directions.Where(x => x.Path.Count > 0))
				{
					var (posX, posY) = GetPathPosition(item, dir);

					if (levelPositions.TryGetValue(posX + posY * 256, out int nextLevel))
					{
						if (dir.Next != (WorldPathNextEnum)nextLevel)
						{
							dir.Next = (WorldPathNextEnum)nextLevel;
							CreateReversePath(dir, (WorldPathNextEnum)level);
						}
					}
				}
			}

			void CreateReversePath(WorldPathDirection dir, WorldPathNextEnum next)
			{
				var reverseDir = GetReverseWorldPathDir(dir);
				if (reverseDir != null && reverseDir != dir)
				{
					reverseDir.Path = [.. dir.Path.AsEnumerable().Reverse()
							.Select(x => new WorldPathSegment
							{
								Status = GetStatus(GetReverseDir(x.Direction), x.Status),
								Direction = GetReverseDir(x.Direction),
								Steps = x.Steps
							})];

					reverseDir.Next = next;
					reverseDir.Progress = dir.Progress;
				}
			}
		}

		void UnbindPath(WorldPathDirection dir)
		{
			if (dir.Path.Count == 0 || !IsSpecialExit(dir.Next))
			{
				RemoveReversePath(dir);
				dir.Next = Overworld.IsOverworld(currentWorld) ? WorldPathNextEnum.TeapotOverworld : WorldPathNextEnum.Overworld;
			}
		}

		#region Reverse

		static WorldPathDirectionEnum GetReverseDir(WorldPathDirectionEnum dir)
		{
			return dir switch
			{
				WorldPathDirectionEnum.Right => WorldPathDirectionEnum.Left,
				WorldPathDirectionEnum.Left => WorldPathDirectionEnum.Right,
				WorldPathDirectionEnum.Up => WorldPathDirectionEnum.Down,
				WorldPathDirectionEnum.Down => WorldPathDirectionEnum.Up,
				_ => throw new InvalidOperationException(),
			};
		}

		WorldPathDirection GetReverseWorldPathDir(WorldPathDirection dir)
		{
			if (dir.Path.Count > 0 && !IsSpecialExit(dir.Next))
			{
				WorldPathDirectionEnum reverseDir = GetReverseDir(dir.Path.Last().Direction);
				return PathData[(int)dir.Next].Directions[(int)reverseDir];
			}

			return null;
		}

		void RemoveReversePath(WorldPathDirection dir)
		{
			var reverseDir = GetReverseWorldPathDir(dir);
			if (reverseDir != null && reverseDir != dir && dir == GetReverseWorldPathDir(reverseDir))
			{
				reverseDir.Path.Clear();
				reverseDir.Progress = WorldPathProgressEnum.None;
			}
		}

		#endregion

		static WorldPathStatusEnum GetStatus(WorldPathDirectionEnum newDir, WorldPathStatusEnum status)
		{
			switch (status)
			{
				case WorldPathStatusEnum.Invisible:
					return WorldPathStatusEnum.Invisible;

				case WorldPathStatusEnum.WaterFront:
				case WorldPathStatusEnum.WaterBack:
					if (newDir == WorldPathDirectionEnum.Up)
					{
						return WorldPathStatusEnum.WaterBack;
					}

					return WorldPathStatusEnum.WaterFront;

				default:
					return (WorldPathStatusEnum)newDir + 1;
			}
		}

		#endregion

		#region Draw

		public void Draw(Graphics g)
		{
			if (!bitmapCache)
			{
				bitmapCache = true;
				RenderPath();
			}

			DrawPath();
			DrawFlag();

			void DrawPath()
			{
				if (TransparentPath)
				{
					g.DrawImage(pathBBitmap, new Rectangle(0, 0, pathBBitmap.Width, pathBBitmap.Height),
						0, 0, pathBBitmap.Width, pathBBitmap.Height, GraphicsUnit.Pixel, transparentImageAttributes);
				}
				else
				{
					g.DrawImage(pathBBitmap, 0, 0);
				}

			}

			void DrawFlag()
			{
				if (Overworld.IsOverworld(currentWorld) && currentLevel != 7)
				{
					var item = flags[currentLevel];
					int animationIndex = GetAnimationIndex();
					for (int n = 0; n < 4; n++)
					{
						var src = new Rectangle((n + animationIndex % 3 * 4) * 8, 0, 8, 8);
						var dest = new Rectangle((item.X + n % 2 * 8) * zoom, (item.Y + n / 2 * 8) * zoom, 8 * zoom, 8 * zoom);
						g.DrawImage(tilesFlag.Bitmap, dest, src, GraphicsUnit.Pixel);
					}
				}
			}

			void RenderPath()
			{
				//bitmaps are needed because path layer can be rendered with transparency
				//and because path are rendered in two steps
				using Graphics gPathA = Graphics.FromImage(pathABitmap);
				using Graphics gPathB = Graphics.FromImage(pathBBitmap);
				gPathA.Clear(Color.Transparent);
				gPathB.Clear(Color.Transparent);

				DrawPaths();
				DrawLevels();
				DrawExits();
				if (!TransparentPath)
				{
					DrawProgress();
				}

				gPathB.DrawImage(pathABitmap, 0, 0);

				void DrawLevels()
				{
					using var format = new StringFormat();
					using var font = new Font("Arial", 5.0f * zoom);
					using var fontBold = new Font(font.FontFamily, font.Size, FontStyle.Bold);
					using var pen = new Pen(Color.Black, zoom * 2.0f);
					using var brush = new SolidBrush(Color.Black);
					format.LineAlignment = StringAlignment.Center;
					format.Alignment = StringAlignment.Center;

					WorldPathNextEnum next = currentDirection == null ? (WorldPathNextEnum)(-1) : currentDirection.Next;

					foreach (var level in levels[currentWorld].OrderBy(x => x == currentLevel || next == (WorldPathNextEnum)x))
					{
						var item = PathData[level];
						int posX = item.X;
						int posY = item.Y;

						bool selected = currentPath == item || next == (WorldPathNextEnum)level;
						var dest = new Rectangle(posX * zoom, posY * zoom, 8 * zoom, 8 * zoom);

						using (GraphicsPath path = RoundedRect(dest, zoom * 2))
						{
							brush.Color = pathColors[(selected && currentDirection == null ? (int)pathMode : 0) + (selected ? 0 : 3)];
							gPathB.DrawPath(pen, path);
							gPathA.FillPath(brush, path);
						}

						if (!TransparentPath)
						{
							gPathA.DrawString((Array.IndexOf(levels[currentWorld], level) + 1).ToString(), currentPath == item ? fontBold : font, Brushes.Black, dest, format);
						}
					}
				}

				void DrawExits()
				{
					using var format = new StringFormat();
					using var font = new Font("Arial", 5.0f * zoom);
					using var penBorder = new Pen(Color.Black, zoom * 2.0f);
					using var brush = new SolidBrush(Color.Black);
					format.LineAlignment = StringAlignment.Center;
					format.Alignment = StringAlignment.Center;
					var paths = new[] { "O", "S", "T", "T" };

					foreach (var dir in currentPath.Directions.Where(x => x.Path.Count > 0 && IsSpecialExit(x.Next))
							.OrderBy(x => x == currentDirection))
					{
						var (nextX, nextY) = GetPathPosition(currentPath, dir);

						var lastPath = dir.Path[^1..].FirstOrDefault(x =>
							x.Status == WorldPathStatusEnum.WaterFront ||
							x.Status == WorldPathStatusEnum.WaterBack ||
							x.Status == WorldPathStatusEnum.Invisible);

						int exitX = Math.Max(0, Math.Min(nextX, CurrentMapX - 8));
						int exitY = Math.Max(0, Math.Min(nextY, CurrentMapY - 8));
						WorldPathNextEnum[] flags = [WorldPathNextEnum.Overworld, WorldPathNextEnum.Sherbet, WorldPathNextEnum.Teapot, WorldPathNextEnum.TeapotOverworld];

						bool selected = dir == currentDirection;
						var dest = new Rectangle(exitX * zoom, exitY * zoom, 8 * zoom, 8 * zoom);

						using (GraphicsPath path = RoundedRect(dest, zoom * 2))
						{
							brush.Color = pathColors[(selected ? (int)pathMode : (lastPath != null ? (int)GroupPath(lastPath.Status) : 0)) + (selected ? 0 : 3)];
							gPathB.DrawPath(penBorder, path);
							gPathA.FillPath(brush, path);
						}

						if (!TransparentPath)
						{
							gPathA.DrawString(paths[Array.IndexOf(flags, dir.Next)], font, Brushes.Black, dest, format);
						}
					}
				}

				void DrawPaths()
				{
					foreach (var dirs in currentPath.Directions.Where(x => x.Path.Count > 0).OrderBy(x => currentDirection == x))
					{
						int startX = currentPath.X;
						int startY = currentPath.Y;
						bool selected = currentDirection == dirs;

						List<Point> points = [];
						List<Color> colors = [];

						points.Add(new Point((startX + 4) * zoom, (startY + 4) * zoom));
						foreach (var path in dirs.Path)
						{
							(startX, startY) = GetPathPosition(path, startX, startY);
							points.Add(new Point((startX + 4) * zoom, (startY + 4) * zoom));

							var color = pathColors[(int)GroupPath(path.Status) + (selected ? 0 : 3)];
							colors.Add(color);
						}

						DrawLine(points, colors);
					}

					void DrawLine(List<Point> points, List<Color> colors)
					{
						using (var pen = new Pen(Color.Black, zoom * 6.0f))
						{
							gPathB.DrawLines(pen, points.ToArray());
						}

						using var brush = new SolidBrush(Color.Black);
						var miterPoints = GetMitterPoints(zoom * 4.0f);

						int n = 0;
						foreach (var item in colors.GroupByAdjacent(x => x, (x, y) => (Color: x, Count: y.Count())))
						{
							brush.Color = item.Color;
							gPathA.FillPolygon(brush, GetLineStrip(n, item.Count + 1).ToArray(), FillMode.Winding);
							n += item.Count;
						}

						IEnumerable<PointF> GetLineStrip(int index, int count)
						{
							for (int i = 0; i < count; i++)
							{
								yield return miterPoints[(index + i) * 2];
							}

							for (int i = count - 1; i >= 0; i--)
							{
								yield return miterPoints[(index + i) * 2 + 1];
							}
						}

						List<PointF> GetMitterPoints(float thickness)
						{
							var result = new List<PointF>();
							for (int i = 0; i < points.Count; i++)
							{
								PointF getLineA() => new PointF(points[i].X - points[i - 1].X, points[i].Y - points[i - 1].Y).Normalized();
								PointF getLineB() => new PointF(points[i + 1].X - points[i].X, points[i + 1].Y - points[i].Y).Normalized();

								PointF lineA = (i == 0) ? getLineB() : getLineA();
								PointF lineB = (i == points.Count - 1) ? getLineA() : getLineB();

								PointF normal = new PointF(-lineA.Y, lineA.X).Normalized();
								PointF tangent = new PointF(lineA.X + lineB.X, lineA.Y + lineB.Y).Normalized();
								PointF miter = new(-tangent.Y, tangent.X);

								float length = thickness * 0.5f / (normal.X * miter.X + normal.Y * miter.Y);
								result.Add(new PointF(points[i].X + miter.X * length, points[i].Y + miter.Y * length));
								result.Add(new PointF(points[i].X - miter.X * length, points[i].Y - miter.Y * length));
							}

							return result;
						}
					}
				}

				void DrawProgress()
				{
					using var format = new StringFormat();
					using var font = new Font("Arial", 5.0f * zoom);
					format.LineAlignment = StringAlignment.Center;
					format.Alignment = StringAlignment.Center;

					int[,] offsets =
					{
							{  8,  0 },
							{ -8,  0 },
							{  0, -8 },
							{  0,  8 }
						};

					for (int dir = 0; dir < 4; dir++)
					{
						var dirs = currentPath.Directions[dir];
						if (dirs.Path.Count > 0 && dirs.Progress != WorldPathProgressEnum.None)
						{
							int progress = Array.IndexOf(Overworld.ProgressFlags, dirs.Progress);
							if (progress != -1)
							{
								var dest = new Rectangle((currentPath.X + offsets[dir, 0]) * zoom, (currentPath.Y + offsets[dir, 1]) * zoom, 8 * zoom, 8 * zoom);
								gPathA.FillRectangle(Brushes.Gray, dest);
								gPathA.DrawString(progress.ToString(), font, Brushes.White, dest, format);
							}
						}
					}
				}

				GraphicsPath RoundedRect(Rectangle bounds, int offset)
				{
					GraphicsPath path = new();

					path.AddLines(
					[
						new Point(bounds.Left + offset, bounds.Top),
						new Point(bounds.Right - offset, bounds.Top),
						new Point(bounds.Right, bounds.Top + offset),
						new Point(bounds.Right, bounds.Bottom - offset),
						new Point(bounds.Right - offset, bounds.Bottom),
						new Point(bounds.Left + offset, bounds.Bottom),
						new Point(bounds.Left, bounds.Bottom - offset),
						new Point(bounds.Left, bounds.Top + offset)
					]);

					path.CloseFigure();
					return path;
				}
			}
		}

		static ImageAttributes GetTransparentImageAttributes()
		{
			var ptsArray = new float[][]
			{
				[1, 0, 0, 0, 0],
				[0, 1, 0, 0, 0],
				[0, 0, 1, 0, 0],
				[0, 0, 0, 0.5f, 0],
				[0, 0, 0, 0, 1]
			};
			var clrMatrix = new ColorMatrix(ptsArray);
			var imgAttributes = new ImageAttributes();
			imgAttributes.SetColorMatrix(clrMatrix);
			return imgAttributes;
		}

		static PathModeEnum GroupPath(WorldPathStatusEnum status)
		{
			return status switch
			{
				WorldPathStatusEnum.Invisible => PathModeEnum.Invisible,
				WorldPathStatusEnum.WaterFront or WorldPathStatusEnum.WaterBack => PathModeEnum.Water,
				_ => PathModeEnum.None,
			};
		}

		#endregion

		static (int X, int Y) GetPathPosition(WorldPath level, WorldPathDirection direction)
		{
			int posX = level.X;
			int posY = level.Y;

			foreach (var path in direction.Path)
			{
				(posX, posY) = GetPathPosition(path, posX, posY);
			}

			return (posX, posY);
		}

		static IEnumerable<(int X, int Y)> GetPathPositions(WorldPath level, WorldPathDirection direction)
		{
			int posX = level.X;
			int posY = level.Y;

			foreach (var path in direction.Path)
			{
				(int X, int Y) = GetPathPosition(path, posX, posY);
				while (X != posX || Y != posY)
				{
					posX += Math.Sign(X - posX);
					posY += Math.Sign(Y - posY);
					yield return (posX, posY);
				}
			}

			yield return (posX, posY);
		}

		static (int X, int Y) GetPathPosition(WorldPathSegment path, int posX, int posY)
		{
			switch (path.Direction)
			{
				case WorldPathDirectionEnum.Right:
					posX += path.Steps;
					break;

				case WorldPathDirectionEnum.Left:
					posX -= path.Steps;
					break;

				case WorldPathDirectionEnum.Up:
					posY -= path.Steps;
					break;

				case WorldPathDirectionEnum.Down:
					posY += path.Steps;
					break;
			}

			return (posX, posY);
		}

		static bool IsSpecialExit(WorldPathNextEnum next)
		{
			return next switch
			{
				WorldPathNextEnum.Overworld or WorldPathNextEnum.Sherbet or WorldPathNextEnum.Teapot or WorldPathNextEnum.TeapotOverworld => true,
				_ => false,
			};
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				disposed = true;
				if (disposing)
				{
					pathABitmap.Dispose();
					pathBBitmap.Dispose();
					tilesFlag.Dispose();
					transparentImageAttributes.Dispose();
				}
			}
		}

		int CurrentMapX => Overworld.IsOverworld(currentWorld) ? 256 : 160;

		int CurrentMapY => Overworld.IsOverworld(currentWorld) ? 256 : 144;
	}
}
