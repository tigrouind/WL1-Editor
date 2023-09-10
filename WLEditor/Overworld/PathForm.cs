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

		readonly int[][] levels =
		{
			new [] { 7, 15, 14, 12, 25, 41 },
			new [] { 7, 15, 14, 12, 25, 41 },
			new [] { 6, 16, 13, 5, 17, 9 },
			new [] { 33, 2, 4, 8, 32, 24 },
			new [] { 3, 21, 22, 39, 27, 28 },
			new [] { 0, 30, 31, 11, 20 },
			new [] { 38, 29, 1, 19, 18, 26 },
			new [] { 37, 34, 35, 40 },
			new [] { 0, 1, 5, 2, 4, 3, 6, 7 }
		};

		readonly int[][][] startPositionData =
		{
			new [] { new [] { 0x5558, 0x6075, 0 } },
			new [] { new [] { 0x5558, 0x6075, 0 } },
			new [] { new [] { 0x554C, 0x0000, 0 }, new [] { 0x5552, 0x6095, 1 } },
			new [] { new [] { 0x5527, 0x608D, 0 } },
			new [] { new [] { 0x553F, 0x607D, 0 } },
			new [] { new [] { 0x552D, 0x6089, 0 } },
			new [] { new [] { 0x5533, 0x6085, 0 } },
			new [] { new [] { 0x5539, 0x6081, 0 } }
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

		WorldPath[] PathData => currentWorld == 8 ? overWorldData : worldData;

		public bool SavePaths(Rom rom, out string errorMessage)
		{
			bool result = Overworld.SavePaths(rom, PathData, currentWorld == 8, out errorMessage);
			if (result)
			{
				int[] worldIndex = { 0, 0, 1, 5, 2, 3, 4, 6, 7 };
				Overworld.SaveProgressNextDirection(rom, PathData, levels[currentWorld], worldIndex[currentWorld]);

				if (currentWorld != 8)
				{
					SaveStartPosition();
				}
			}

			return result;

			void SaveStartPosition()
			{
				foreach (var pos in startPositionData[currentWorld])
				{
					FindExitPosition(pos[2], out int posX, out int posY);
					Overworld.SaveStartPosition(rom, posX, posY, FindClosestSide(posX, posY), pos[0], pos[1]);
				}

				int FindClosestSide(int x, int y)
				{
					int[] borders = { x, 160 - x, y, 144 - y };
					int bestSide = -1, min = int.MaxValue;

					for (int i = 0; i < borders.Length; i++)
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

					var item = PathData[level];
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
			}

		}

		public void SetZoom(int zoomLevel)
		{
			zoom = zoomLevel;
		}

		public void LoadPaths(Rom rom)
		{
			worldData = Overworld.LoadPaths(rom, false);
			overWorldData = Overworld.LoadPaths(rom, true);
		}

		public void LoadWorld(int world)
		{
			currentWorld = world;
			currentLevel = levels[currentWorld][0];
			currentPath = PathData[currentLevel];
			currentDirection = null;
		}

		#region Commands

		public bool ProcessPathKey(Keys key)
		{
			var keyCode = key & Keys.KeyCode;
			switch (key)
			{
				case Keys.PageUp:
					NextLevel();
					return true;

				case Keys.PageDown:
					PreviousLevel();
					return true;

				case Keys.Shift | Keys.Delete:
					RemoveAllPaths();

					pictureBox.Invalidate();
					SetChanges();
					return true;

				case Keys.Delete:
					if (currentDirection != null)
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
					ChangeDirection();
					pictureBox.Invalidate();
					return true;

				case Keys.Up | Keys.Control:
				case Keys.Down | Keys.Control:
				case Keys.Left | Keys.Control:
				case Keys.Right | Keys.Control:
					MoveLevel();
					BindPaths();

					pictureBox.Invalidate();
					SetChanges();
					return true;

				case Keys.Up | Keys.Shift:
				case Keys.Down | Keys.Shift:
				case Keys.Left | Keys.Shift:
				case Keys.Right | Keys.Shift:
					AddPath();
					BindPaths();

					pictureBox.Invalidate();
					SetChanges();
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

			#region Commands

			void AddPath()
			{
				//set direction if needed
				if (currentDirection == null)
				{
					currentDirection = currentPath.Directions[GetDirection()];
				}

				UnbindPath(currentDirection);

				//check previous step
				int newDir = GetDirection();
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
						Status = GetStatus(newDir, new[] { 0, 14, 12 }[pathMode]),
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
				foreach (var dir in currentPath.Directions)
				{
					UnbindPath(dir);
					dir.Path.Clear();
					dir.Progress = 0xFD;
				}

				currentDirection = null;
				pathMode = 0;
			}

			void SetProgress()
			{
				int[] flags = { 0xFD, 1, 2, 4, 8, 16, 32, 64, 128 };
				int progress = Array.IndexOf(flags, currentDirection.Progress);
				currentDirection.Progress = flags[(progress + 1) % flags.Length];
				var reverseDir = GetReverseWorldPathDir(currentDirection);
				if (reverseDir != null)
				{
					reverseDir.Progress = currentDirection.Progress;
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

			void MoveLevel()
			{
				//align on grid
				int posX = currentPath.X / gridSnap * gridSnap;
				int posY = currentPath.Y / gridSnap * gridSnap;

				switch (keyCode)
				{
					case Keys.Up:
						currentPath.Y = Math.Max(posY - gridSnap, 0);
						break;

					case Keys.Down:
						currentPath.Y = Math.Min(posY + gridSnap, CurrentMapY - 8);
						break;

					case Keys.Left:
						currentPath.X = Math.Max(posX - gridSnap, 0);
						break;

					case Keys.Right:
						currentPath.X = Math.Min(posX + gridSnap, CurrentMapX - 8);
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
					foreach (var dir in PathData[level].Directions)
					{
						if (dir.Next == currentLevel)
						{
							UnbindPath(dir);
						}
					}
				}
			}

			void ChangeDirection()
			{
				var newDir = GetDirection();
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
					currentPath = PathData[currentLevel];

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
					currentPath = PathData[currentLevel];

					pathMode = 0;
					pictureBox.Invalidate();
				}
			}

			#endregion

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
						GetPathPosition(item, dir, out int posX, out int posY);

						if (levelPositions.TryGetValue(posX + posY * 256, out int nextLevel))
						{
							if (dir.Next != nextLevel)
							{
								dir.Next = nextLevel;
								CreateReversePath(dir);
							}
						}
					}
				}

				void CreateReversePath(WorldPathDirection dir)
				{
					var reverseDir = GetReverseWorldPathDir(dir);
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
			}

			void UnbindPath(WorldPathDirection dir)
			{
				if (dir.Path.Count == 0 || (dir.Next != 0xFA && dir.Next != 0xF9 && dir.Next != 0xF8 && dir.Next != 0xFD))
				{
					RemoveReversePath(dir);
					dir.Next = currentWorld == 8 ? 0xF8 : 0xFD;
				}
			}

			int GetDirection()
			{
				switch (keyCode)
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

			#region Reverse

			int GetReverseDir(int dir)
			{
				switch (dir)
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

			WorldPathDirection GetReverseWorldPathDir(WorldPathDirection dir)
			{
				if (dir.Path.Count > 0 && dir.Next != 0xFA && dir.Next != 0xF9 && dir.Next != 0xF8 && dir.Next != 0xFD)
				{
					int reverseDir = GetReverseDir(dir.Path.Last().Direction);
					return PathData[dir.Next].Directions[reverseDir];
				}

				return null;
			}

			void RemoveReversePath(WorldPathDirection dir)
			{
				var reverseDir = GetReverseWorldPathDir(dir);
				if (reverseDir != null && reverseDir != dir && dir == GetReverseWorldPathDir(reverseDir))
				{
					reverseDir.Path.Clear();
				}
			}

			#endregion
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
			DrawPaths();
			DrawProgress();
			DrawLevels();
			DrawExits();

			void DrawLevels()
			{
				using (var format = new StringFormat())
				using (var font = new Font("Arial", 5.0f * zoom))
				using (var pen = new Pen(Color.Black, zoom * 2.0f))
				{
					format.LineAlignment = StringAlignment.Center;
					format.Alignment = StringAlignment.Center;

					int next = currentDirection == null ? -1 : currentDirection.Next;

					foreach (var level in levels[currentWorld].OrderBy(x => x == currentLevel))
					{
						var item = PathData[level];
						int posX = item.X;
						int posY = item.Y;

						bool selected = currentPath == item;

						using (GraphicsPath path = RoundedRect(new Rectangle(posX * zoom, posY * zoom, 8 * zoom, 8 * zoom), zoom * 2))
						{
							g.DrawPath(pen, path);
							g.FillPath(selected ? Brushes.Lime : Brushes.MediumSeaGreen, path);
						}

						g.DrawString((Array.IndexOf(levels[currentWorld], level) + 1).ToString(), font, Brushes.Black, (posX + 4) * zoom, (posY + 4) * zoom, format);
					}
				}
			}

			void DrawExits()
			{
				using (var format = new StringFormat())
				using (var font = new Font("Arial", 5.0f * zoom))
				using (var penBorder = new Pen(Color.Black, zoom * 2.0f))
				{
					format.LineAlignment = StringAlignment.Center;
					format.Alignment = StringAlignment.Center;

					foreach (var dir in currentPath.Directions.Where(x => x.Path.Count > 0 && (x.Next == 0xFD || x.Next == 0xFA || x.Next == 0xF9 || x.Next == 0xF8))
							.OrderBy(x => x == currentDirection))
					{
						GetPathPosition(currentPath, dir, out int nextX, out int nextY);

						int exitX = Math.Max(0, Math.Min(nextX, CurrentMapX - 8));
						int exitY = Math.Max(0, Math.Min(nextY, CurrentMapY - 8));
						int[] flags = { 0xFD, 0xFA, 0xF9, 0xF8 };

						using (GraphicsPath path = RoundedRect(new Rectangle(exitX * zoom, exitY * zoom, 8 * zoom, 8 * zoom), zoom * 2))
						{
							g.DrawPath(penBorder, path);
							g.FillPath(Brushes.MediumSeaGreen, path);
						}

						g.DrawString(new[] { "A", "B", "C", "C" }[Array.IndexOf(flags, dir.Next)], font, Brushes.Black, (exitX + 4) * zoom, (exitY + 4) * zoom, format);
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

					DrawLine(points, colors);
				}

				void DrawLine(List<Point> points, List<Color> colors)
				{
					using (var pen = new Pen(Color.Black, zoom * 6.0f))
					{
						g.DrawLines(pen, points.ToArray());
					}

					using (var brush = new SolidBrush(Color.Black))
					{
						var miterPoints = GetMitterPoints(zoom * 4.0f);

						int n = 0;
						foreach (var item in colors.GroupByAdjacent(x => x, (x, y) => new { Color = x, Count = y.Count() }))
						{
							brush.Color = item.Color;
							g.FillPolygon(brush, GetLineStrip(n, item.Count + 1).ToArray(), FillMode.Winding);
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
							PointF miter = new PointF(-tangent.Y, tangent.X);

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
				using (var format = new StringFormat())
				using (var font = new Font("Arial", 5.0f * zoom))
				{
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
						if (dirs.Path.Count > 0 && dirs.Progress != 0xFD)
						{
							int[] progressFlags = { 0, 1, 2, 4, 8, 16, 32, 64, 128 };
							int progress = Array.IndexOf(progressFlags, dirs.Progress);

							g.FillRectangle(Brushes.Gray, (currentPath.X + offsets[dir, 0]) * zoom, (currentPath.Y + offsets[dir, 1]) * zoom, 8 * zoom, 8 * zoom);
							g.DrawString(progress.ToString(), font, Brushes.White, (currentPath.X + 4 + offsets[dir, 0]) * zoom, (currentPath.Y + 4 + offsets[dir, 1]) * zoom, format);
						}
					}
				}
			}

			GraphicsPath RoundedRect(Rectangle bounds, int offset)
			{
				GraphicsPath path = new GraphicsPath();

				path.AddLines(new[]
				{
				new Point(bounds.Left + offset, bounds.Top),
				new Point(bounds.Right - offset, bounds.Top),
				new Point(bounds.Right, bounds.Top + offset),
				new Point(bounds.Right, bounds.Bottom - offset),
				new Point(bounds.Right - offset, bounds.Bottom),
				new Point(bounds.Left + offset, bounds.Bottom),
				new Point(bounds.Left, bounds.Bottom - offset),
				new Point(bounds.Left, bounds.Top + offset)
			});

				path.CloseFigure();
				return path;
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

		void GetPathPosition(WorldPath level, WorldPathDirection direction, out int posX, out int posY)
		{
			posX = level.X;
			posY = level.Y;

			foreach (var path in direction.Path)
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

		int CurrentMapX => currentWorld == 8 ? 256 : 160;

		int CurrentMapY => currentWorld == 8 ? 256 : 144;
	}
}
