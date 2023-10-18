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

		PathModeEnum pathMode;
		const int gridSnap = 4;

		int zoom;
		int currentWorld;
		readonly PictureBox pictureBox;
		readonly DirectBitmap tilesFlag = new DirectBitmap(16 * 8, 1 * 8);
		public Func<int> GetAnimationIndex;

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
					var (posX, posY) = FindExitPosition(pos[2]);
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

				(int posX, int posY) FindExitPosition(int startLevel)
				{
					int level = levels[currentWorld][startLevel];

					var item = PathData[level];
					var dir = item.Directions.FirstOrDefault(x => x.Path.Count > 0 && IsSpecialExit(x.Next));
					if (dir != null)
					{
						return GetPathPosition(item, dir);
					}
					else
					{
						return (item.X, item.Y);
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
			Overworld.DumpFlags(rom, tilesFlag);
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

				case Keys.Up | Keys.Alt:
				case Keys.Down | Keys.Alt:
				case Keys.Left | Keys.Alt:
				case Keys.Right | Keys.Alt:
					if (currentWorld == 8 && currentLevel != 7)
					{
						MoveFlag();

						pictureBox.Invalidate();
						SetChanges();
					}
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
					pathMode = pathMode == PathModeEnum.Invisible ? PathModeEnum.None : PathModeEnum.Invisible;
					return true;

				case Keys.W:
					pathMode = pathMode == PathModeEnum.Water ? PathModeEnum.None : PathModeEnum.Water;
					return true;

				case Keys.X:
					if (currentDirection != null && IsSpecialExit(currentDirection.Next))
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
					currentDirection = currentPath.Directions[(int)GetDirection()];
				}

				UnbindPath(currentDirection);

				//check previous step
				WorldPathDirectionEnum newDir = GetDirection();
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
						RemovePath();
					}
				}
				else
				{
					WorldPathStatusEnum status = GetStatus();

					currentDirection.Path.Add(new WorldPathSegment
					{
						Direction = newDir,
						Status = this.GetStatus(newDir, status),
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

			void SetProgress()
			{
				WorldPathProgressEnum[] flags =
				{
					WorldPathProgressEnum.None,
					WorldPathProgressEnum.Level1,
					WorldPathProgressEnum.Level2,
					WorldPathProgressEnum.Level3,
					WorldPathProgressEnum.Level4,
					WorldPathProgressEnum.Level5,
					WorldPathProgressEnum.Level6,
					WorldPathProgressEnum.Level7,
					WorldPathProgressEnum.Level8
				};

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
					currentDirection.Next = WorldPathNextEnum.TeapotOverworld;
				}
				else
				{
					WorldPathNextEnum[] exits = { WorldPathNextEnum.Overworld, WorldPathNextEnum.Sherbet, WorldPathNextEnum.Teapot };
					int next = Array.IndexOf(exits, currentDirection.Next);
					currentDirection.Next = exits[(next + 1) % exits.Length];
				}
			}

			void MoveFlag()
			{
				int posX = currentPath.FlagX / gridSnap * gridSnap;
				int posY = currentPath.FlagY / gridSnap * gridSnap;

				switch (keyCode)
				{
					case Keys.Up:
						currentPath.FlagY = Math.Max(posY - gridSnap, 0);
						break;

					case Keys.Down:
						currentPath.FlagY = Math.Min(posY + gridSnap, CurrentMapY - 16);
						break;

					case Keys.Left:
						currentPath.FlagX = Math.Max(posX - gridSnap, 0);
						break;

					case Keys.Right:
						currentPath.FlagX = Math.Min(posX + gridSnap, CurrentMapX - 16);
						break;
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

				if (currentWorld != 8)
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
						if (dir.Next == (WorldPathNextEnum)currentLevel)
						{
							UnbindPath(dir);
						}
					}
				}
			}

			void ChangeDirection()
			{
				var newDir = GetDirection();
				currentDirection = currentPath.Directions[(int)newDir];

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
						reverseDir.Path = dir.Path.AsEnumerable().Reverse()
							.Select(x => new WorldPathSegment
							{
								Status = GetStatus(GetReverseDir(x.Direction), x.Status),
								Direction = GetReverseDir(x.Direction),
								Steps = x.Steps
							})
							.ToList();

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
					dir.Next = currentWorld == 8 ? WorldPathNextEnum.TeapotOverworld : WorldPathNextEnum.Overworld;
				}
			}

			WorldPathDirectionEnum GetDirection()
			{
				switch (keyCode)
				{
					case Keys.Right:
						return WorldPathDirectionEnum.Right;

					case Keys.Left:
						return WorldPathDirectionEnum.Left;

					case Keys.Up:
						return WorldPathDirectionEnum.Up;

					case Keys.Down:
						return WorldPathDirectionEnum.Down;
				}

				throw new InvalidOperationException();
			}

			#region Reverse

			WorldPathDirectionEnum GetReverseDir(WorldPathDirectionEnum dir)
			{
				switch (dir)
				{
					case WorldPathDirectionEnum.Right:
						return WorldPathDirectionEnum.Left;

					case WorldPathDirectionEnum.Left:
						return WorldPathDirectionEnum.Right;

					case WorldPathDirectionEnum.Up:
						return WorldPathDirectionEnum.Down;

					case WorldPathDirectionEnum.Down:
						return WorldPathDirectionEnum.Up;
				}

				throw new InvalidOperationException();
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
				}
			}

			#endregion
		}

		WorldPathStatusEnum GetStatus(WorldPathDirectionEnum newDir, WorldPathStatusEnum status)
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
			DrawPaths();
			DrawProgress();
			DrawLevels();
			DrawExits();
			DrawFlag();

			void DrawLevels()
			{
				using (var format = new StringFormat())
				using (var font = new Font("Arial", 5.0f * zoom))
				using (var pen = new Pen(Color.Black, zoom * 2.0f))
				{
					format.LineAlignment = StringAlignment.Center;
					format.Alignment = StringAlignment.Center;

					WorldPathNextEnum next = currentDirection == null ? (WorldPathNextEnum)(-1) : currentDirection.Next;

					foreach (var level in levels[currentWorld].OrderBy(x => x == currentLevel || next == (WorldPathNextEnum)x))
					{
						var item = PathData[level];
						int posX = item.X;
						int posY = item.Y;

						bool selected = currentPath == item || next == (WorldPathNextEnum)level;

						using (GraphicsPath path = RoundedRect(new Rectangle(posX * zoom, posY * zoom, 8 * zoom, 8 * zoom), zoom * 2))
						{
							g.DrawPath(pen, path);
							g.FillPath(selected ? Brushes.Lime : Brushes.MediumSeaGreen, path);
						}

						g.DrawString((Array.IndexOf(levels[currentWorld], level) + 1).ToString(), font, Brushes.Black, (posX + 4) * zoom, (posY + 4) * zoom, format);
					}
				}
			}

			void DrawFlag()
			{
				if (currentWorld == 8 && currentLevel != 7)
				{
					var item = PathData[currentLevel];
					int animationIndex = GetAnimationIndex();
					for (int n = 0; n < 4; n++)
					{
						var src = new Rectangle((n + animationIndex % 3 * 4) * 8, 0, 8, 8);
						var dest = new Rectangle((item.FlagX + n % 2 * 8) * zoom, (item.FlagY + n / 2 * 8) * zoom, 8 * zoom, 8 * zoom);
						g.DrawImage(tilesFlag.Bitmap, dest, src, GraphicsUnit.Pixel);
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

					foreach (var dir in currentPath.Directions.Where(x => x.Path.Count > 0 && IsSpecialExit(x.Next))
							.OrderBy(x => x == currentDirection))
					{
						var (nextX, nextY) = GetPathPosition(currentPath, dir);

						int exitX = Math.Max(0, Math.Min(nextX, CurrentMapX - 8));
						int exitY = Math.Max(0, Math.Min(nextY, CurrentMapY - 8));
						WorldPathNextEnum[] flags = { WorldPathNextEnum.Overworld, WorldPathNextEnum.Sherbet, WorldPathNextEnum.Teapot, WorldPathNextEnum.TeapotOverworld };

						bool selected = dir == currentDirection;

						using (GraphicsPath path = RoundedRect(new Rectangle(exitX * zoom, exitY * zoom, 8 * zoom, 8 * zoom), zoom * 2))
						{
							g.DrawPath(penBorder, path);
							g.FillPath(selected ? Brushes.Lime : Brushes.MediumSeaGreen, path);
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

						Color color = pathColors[(int)GroupPath(path.Status) + (selected ? 0 : 3)];
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
						if (dirs.Path.Count > 0)
						{
							WorldPathProgressEnum[] flags =
							{
								WorldPathProgressEnum.Level1,
								WorldPathProgressEnum.Level2,
								WorldPathProgressEnum.Level3,
								WorldPathProgressEnum.Level4,
								WorldPathProgressEnum.Level5,
								WorldPathProgressEnum.Level6,
								WorldPathProgressEnum.Level7,
								WorldPathProgressEnum.Level8
							};

							int progress = Array.IndexOf(flags, dirs.Progress);
							if (progress != -1)
							{
								g.FillRectangle(Brushes.Gray, (currentPath.X + offsets[dir, 0]) * zoom, (currentPath.Y + offsets[dir, 1]) * zoom, 8 * zoom, 8 * zoom);
								g.DrawString((progress + 1).ToString(), font, Brushes.White, (currentPath.X + 4 + offsets[dir, 0]) * zoom, (currentPath.Y + 4 + offsets[dir, 1]) * zoom, format);
							}
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

		PathModeEnum GroupPath(WorldPathStatusEnum status)
		{
			switch (status)
			{
				case WorldPathStatusEnum.Invisible:
					return PathModeEnum.Invisible;

				case WorldPathStatusEnum.WaterFront:
				case WorldPathStatusEnum.WaterBack:
					return PathModeEnum.Water;

				default:
					return PathModeEnum.None;
			}
		}

		#endregion

		(int posX, int posY) GetPathPosition(WorldPath level, WorldPathDirection direction)
		{
			int posX = level.X;
			int posY = level.Y;

			foreach (var path in direction.Path)
			{
				GetPathPosition(path, ref posX, ref posY);
			}

			return (posX, posY);
		}

		void GetPathPosition(WorldPathSegment path, ref int posX, ref int posY)
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
		}

		bool IsSpecialExit(WorldPathNextEnum next)
		{
			switch (next)
			{
				case WorldPathNextEnum.Overworld:
				case WorldPathNextEnum.Sherbet:
				case WorldPathNextEnum.Teapot:
				case WorldPathNextEnum.TeapotOverworld:
					return true;

				default:
					return false;
			}
		}

		int CurrentMapX => currentWorld == 8 ? 256 : 160;

		int CurrentMapY => currentWorld == 8 ? 256 : 144;
	}
}
