using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WLEditor
{
	public class LevelPictureBox : PictureBox
	{
		readonly bool[] invalidTiles = new bool[256 * 32];
		readonly DirectBitmap levelTiles = new DirectBitmap(4096, 512);
		int zoom;

		public bool ShowSectors = true;
		public bool ShowObjects = true;
		public bool ShowColliders = true;
		public int SwitchMode;
		public int SwitchType;
		public int CurrentSector = -1;
		int currentTileIndex = -1;
		int lastTile = -1;
		public int ScrollLines;
		int mouseDownSector;
		readonly Selection selection = new Selection(16);
		List<SelectionChange> changes = new List<SelectionChange>();

		public event EventHandler<TileEventArgs> TileMouseDown;
		public event EventHandler<TileEventArgs> TileMouseMove;
		public event EventHandler SectorChanged;

		public readonly static Brush[] TransparentBrushes =
		{
			new SolidBrush(Color.FromArgb(64, 128, 64, 0)),  //brown
			new SolidBrush(Color.FromArgb(64, 0, 255, 0)),   //green
			new SolidBrush(Color.FromArgb(64, 0, 255, 255)), //light blue
			new SolidBrush(Color.FromArgb(64, 255, 255, 0)), //yellow
			new SolidBrush(Color.FromArgb(64, 0, 0, 255)),   //blue
			new SolidBrush(Color.FromArgb(64, 255, 128, 0)),   //amber
			new SolidBrush(Color.FromArgb(128, 0, 128, 0)),  //dark green
			new SolidBrush(Color.FromArgb(128, 128, 64, 128)),  //purple
		};

		public static Brush EnemyBrush = new SolidBrush(Color.FromArgb(255, 50, 50, 155));

		public LevelPictureBox()
		{
			selection.InvalidateSelection += (s, e) => Invalidate(e.ClipRectangle);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (Level.LevelData != null && !DesignMode)
			{
				using (StringFormat format = new StringFormat())
				using (Font font = new Font("Arial", 8 * zoom))
				using (Graphics g = Graphics.FromImage(levelTiles.Bitmap))
				{
					format.LineAlignment = StringAlignment.Center;
					format.Alignment = StringAlignment.Center;

					//draw tiles to cache
					DrawTiles(g, ShowColliders);

					//draw tiles from cache
					e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
					e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
					e.Graphics.DrawImage(levelTiles.Bitmap, 0, 0, 4096 * zoom, 512 * zoom);

					//sector objects (enemies, powerups)
					if (ShowObjects)
					{
						DrawObjects(font, format, EnemyBrush);

						//wario position
						int index = Level.WarioRightFacing ? 0 : 1;
						Rectangle playerRectangle = Sprite.PlayerRectangles[index];
						Point playerOffset = Sprite.PlayerOffsets[index];

						Rectangle destRect = new Rectangle(
							(Level.WarioPosition % 4096 + playerOffset.X) * zoom,
							(Level.WarioPosition / 4096 + playerOffset.Y) * zoom, playerRectangle.Width * zoom, playerRectangle.Height * zoom);

						if (destRect.IntersectsWith(e.ClipRectangle))
						{
							e.Graphics.DrawImage(Sprite.PlayerSprite.Bitmap, destRect, playerRectangle, GraphicsUnit.Pixel);
						}
					}

					//sectors
					if (ShowSectors)
					{
						DrawSectors(font, format);
						DrawCamera();
						DrawSelectedSector();
						DrawScrollLines();
					}

					selection.DrawSelection(e.Graphics);
				}
			}

			void DrawObjects(Font font, StringFormat format, Brush brush)
			{
				for (int j = 0; j < 32; j++)
				{
					for (int i = 0; i < 256; i++)
					{
						byte data = Level.ObjectsData[i + j * 256];
						if (data > 0)
						{
							Rectangle destRect = new Rectangle(i * 16 * zoom, j * 16 * zoom, 16 * zoom, 16 * zoom);
							if (destRect.IntersectsWith(e.ClipRectangle))
							{
								e.Graphics.FillRectangle(brush, destRect);

								//power up
								if (data > 6)
								{
									e.Graphics.DrawImage(Sprite.TilesObjects.Bitmap, destRect, new Rectangle((data - 7) * 16, 0, 16, 16), GraphicsUnit.Pixel);
								}
							}

							//enemy
							if (data <= 6)
							{
								Rectangle enemyRect = Sprite.LoadedSprites[data - 1];
								if (enemyRect != Rectangle.Empty)
								{
									Point enemyOffset = Sprite.LoadedOffsets[data - 1];

									destRect = new Rectangle((i * 16 + enemyOffset.X) * zoom, (j * 16 + enemyOffset.Y) * zoom, enemyRect.Width * zoom, enemyRect.Height * zoom);
									if (destRect.IntersectsWith(e.ClipRectangle))
									{
										e.Graphics.DrawImage(Sprite.TilesEnemies.Bitmap, destRect, enemyRect, GraphicsUnit.Pixel);
									}
								}
								else
								{
									e.Graphics.DrawString(data.ToString(), font, Brushes.White, (i * 16 + 8) * zoom, (j * 16 + 8) * zoom, format);
								}
							}
						}
					}
				}
			}

			void DrawTiles(Graphics g, bool viewColliders)
			{
				var clipRectangle = GetClipRectangle(e.ClipRectangle, 16 * zoom);

				for (int j = clipRectangle.Top; j < clipRectangle.Bottom; j++)
				{
					for (int i = clipRectangle.Left; i < clipRectangle.Right; i++)
					{
						if (!invalidTiles[i + j * 256])
						{
							invalidTiles[i + j * 256] = true;
							DrawTileToBitmap(i, j);

							if (viewColliders)
							{
								byte tileIndex = Level.LevelData[i + j * 256 + 0x1000];
								tileIndex = (byte)Level.SwitchTile(tileIndex, SwitchMode);

								int specialTile = Level.GetTileInfo(tileIndex, SwitchType).Type;
								if (specialTile != -1)
								{
									g.FillRectangle(TransparentBrushes[specialTile], new Rectangle(i * 16, j * 16, 16, 16));
								}
							}
						}
					}
				}

				void DrawTileToBitmap(int x, int y)
				{
					byte tileIndex = Level.LevelData[x + y * 256 + 0x1000];
					Point dest = new Point(x * 16, y * 16);
					Point src = new Point((tileIndex % 8) * 16, (tileIndex / 8) * 16);

					for (int i = 0; i < 16; i++)
					{
						Array.Copy(Level.Tiles16x16.Bits, src.X + (src.Y + i) * Level.Tiles16x16.Width, levelTiles.Bits, dest.X + (dest.Y + i) * levelTiles.Width, 16);
					}
				}
			}

			void DrawSectors(Font font, StringFormat format)
			{
				foreach (Point point in GetVisibleSectors(e.ClipRectangle))
				{
					DrawScrollInfo(point.X, point.Y);
					DrawSectorInfo(point.X, point.Y);
				}

				DrawSectorBorders();

				void DrawScrollInfo(int x, int y)
				{
					int drawSector = x + y * 16;

					byte scroll = Level.ScrollData[drawSector];
					if ((scroll & 2) == 2)
					{
						e.Graphics.FillRectangle(Brushes.Yellow, x * 256 * zoom, y * 256 * zoom, 6 * zoom, 256 * zoom);
					}

					if ((scroll & 1) == 1)
					{
						e.Graphics.FillRectangle(Brushes.Yellow, ((x + 1) * 256 - 6) * zoom, y * 256 * zoom, 6 * zoom, 256 * zoom);
					}
				}

				void DrawSectorInfo(int x, int y)
				{
					int drawSector = x + y * 16;

					e.Graphics.FillRectangle(Brushes.Blue, x * 256 * zoom, y * 256 * zoom, 16 * zoom, 16 * zoom);
					e.Graphics.DrawString(drawSector.ToString("D2"), font, Brushes.White, (x * 256 + 8) * zoom, (y * 256 + 8) * zoom, format);

					int sectorTarget = Level.Warps[drawSector];
					if (sectorTarget != 255)
					{
						string text = GetWarpName();
						var result = TextRenderer.MeasureText(text, font);

						e.Graphics.FillRectangle(Brushes.Blue, (x * 256 + 20) * zoom, y * 256 * zoom, result.Width, 16 * zoom);
						e.Graphics.DrawString(text, font, Brushes.White, (x * 256) * zoom + result.Width / 2 + 20 * zoom, (y * 256 + 8) * zoom, format);
					}

					string GetWarpName()
					{
						string warpText;
						switch (sectorTarget)
						{
							case 32:
								warpText = "EXIT MAP";
								break;
							case 33:
								warpText = "EXIT A";
								break;
							case 34:
								warpText = "EXIT B";
								break;
							default:
								warpText = "S" + sectorTarget.ToString("D2");
								break;
						}

						return warpText;
					}
				}

				void DrawSectorBorders()
				{
					//draw sector borders
					using (Pen penBlue = new Pen(Color.Blue, 2.0f * zoom))
					{
						penBlue.DashPattern = new[] { 5.0f, 1.0f };

						for (int i = 1; i < 16; i++)
						{
							int x = i * 256 * zoom;
							Rectangle lineRect = new Rectangle(x - zoom, 0, 2 * zoom, 512 * zoom);
							if (lineRect.IntersectsWith(e.ClipRectangle))
							{
								e.Graphics.DrawLine(penBlue, x, 0, x, 512 * zoom);
							}
						}

						e.Graphics.DrawLine(penBlue, 0, 256 * zoom, 4096 * zoom, 256 * zoom);
					}
				}

				IEnumerable<Point> GetVisibleSectors(Rectangle clipRectangle)
				{
					clipRectangle = GetClipRectangle(clipRectangle, 256 * zoom);
					for (int y = clipRectangle.Top; y < clipRectangle.Bottom; y++)
					{
						for (int x = clipRectangle.Left; x < clipRectangle.Right; x++)
						{
							yield return new Point(x, y);
						}
					}
				}
			}

			void DrawCamera()
			{
				if (Level.CameraPosition != -1)
				{
					Rectangle camera = new Rectangle((Level.CameraPosition % 4096) * zoom, (Level.CameraPosition / 4096) * zoom, 10 * 16 * zoom, 9 * 16 * zoom);
					if (camera.IntersectsWith(e.ClipRectangle))
					{
						using (Brush red = new SolidBrush(Color.FromArgb(128, 0, 0, 255)))
						{
							e.Graphics.FillRectangle(red, camera);
						}
					}
				}
			}

			void DrawSelectedSector()
			{
				if (CurrentSector != -1)
				{
					Rectangle sectorRect = new Rectangle((CurrentSector % 16) * 256 * zoom, (CurrentSector / 16) * 256 * zoom, 256 * zoom, 256 * zoom);
					if (sectorRect.IntersectsWith(e.ClipRectangle))
					{
						using (Brush blue = new SolidBrush(Color.FromArgb(64, 0, 0, 255)))
						{
							e.Graphics.FillRectangle(blue, sectorRect);
						}
					}
				}
			}

			void DrawScrollLines()
			{
				if (ScrollLines != 0)
				{
					int positionY = new[] { 23, 15, 7, -1 }[ScrollLines - 1];
					using (Pen pen = new Pen(Color.Yellow, 2.0f * zoom))
					{
						pen.DashPattern = new[] { 5.0f, 1.0f };
						e.Graphics.DrawLine(pen, 0, positionY * 16 * zoom, 4096 * zoom, positionY * 16 * zoom);
						e.Graphics.DrawLine(pen, 0, (positionY + 9) * 16 * zoom, 4096 * zoom, (positionY + 9) * 16 * zoom);
					}
				}
			}

			Rectangle GetClipRectangle(Rectangle clipRectangle, int size)
			{
				Point start = new Point(clipRectangle.Left / size, clipRectangle.Top / size);
				Point end = new Point((clipRectangle.Right - 1) / size + 1, (clipRectangle.Bottom - 1) / size + 1);
				return new Rectangle(start.X, start.Y, end.X - start.X, end.Y - start.Y);
			}
		}

		public void InvalidateTile(int tileIndex)
		{
			using (Region r = new Region(new Rectangle((tileIndex % 256) * 16 * zoom, (tileIndex / 256) * 16 * zoom, 16 * zoom, 16 * zoom)))
			{
				invalidTiles[tileIndex] = false;
				Invalidate(r);
			}
		}

		public void InvalidateAnimatedTiles()
		{
			using (Region r = new Region(Rectangle.Empty))
			{
				for (int tileIndex = 0; tileIndex < 8192; tileIndex++)
				{
					byte data = Level.LevelData[tileIndex + 0x1000];
					if (Level.Animated16x16Tiles[data])
					{
						r.Union(new Rectangle((tileIndex % 256) * 16 * zoom, (tileIndex / 256) * 16 * zoom, 16 * zoom, 16 * zoom));
						invalidTiles[tileIndex] = false;
					}
				}

				Invalidate(r);
			}
		}

		public void InvalidateObject(int tileIndex, int currentObject, int previousObject)
		{
			using (Region r = new Region(new Rectangle((tileIndex % 256) * 16 * zoom, (tileIndex / 256) * 16 * zoom, 16 * zoom, 16 * zoom)))
			{
				AddEnemyRegion(r, previousObject);
				AddEnemyRegion(r, currentObject);
				Invalidate(r);
			}

			void AddEnemyRegion(Region region, int enemyIndex)
			{
				if (enemyIndex >= 1 && enemyIndex <= 6)
				{
					Rectangle enemyRect = Sprite.LoadedSprites[enemyIndex - 1];
					Point enemyOffset = Sprite.LoadedOffsets[enemyIndex - 1];

					if (enemyRect != Rectangle.Empty)
					{
						region.Union(new Rectangle(((tileIndex % 256) * 16 + enemyOffset.X) * zoom, ((tileIndex / 256) * 16 + enemyOffset.Y) * zoom, enemyRect.Width * zoom, enemyRect.Height * zoom));
					}
				}
			}
		}

		void OnMouseEvent(MouseEventArgs e, TileEventStatus status)
		{
			if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
			{
				int tilePosX = e.Location.X / 16 / zoom;
				int tilePosY = e.Location.Y / 16 / zoom;
				int tileIndex = tilePosX + tilePosY * 256;

				if (tileIndex != currentTileIndex)
				{
					currentTileIndex = tileIndex;
					if (TileMouseDown != null)
					{
						TileMouseDown(this, new TileEventArgs(e.Button, status, tilePosX, tilePosY));
					}
				}
			}
			else if (e.Button == MouseButtons.Middle)
			{
				Point coordinates = e.Location;
				int sector = e.Location.X / 256 / zoom + (e.Location.Y / 256 / zoom) * 16;

				switch (status)
				{
					case TileEventStatus.MouseDown:
						mouseDownSector = CurrentSector;
						break;

					case TileEventStatus.MouseMove:
						mouseDownSector = -1;
						break;

					case TileEventStatus.MouseUp:
						if (CurrentSector == mouseDownSector)
						{
							sector = -1;
						}
						break;
				}

				if (sector != CurrentSector)
				{
					CurrentSector = sector;
					if (SectorChanged != null)
					{
						SectorChanged(this, EventArgs.Empty);
					}
				}
			}

			if (status == TileEventStatus.MouseDown || status == TileEventStatus.MouseMove)
			{
				int currentTile = e.Location.X / 16 / zoom + (e.Location.Y / 16 / zoom) * 256;
				if (currentTile != lastTile)
				{
					lastTile = currentTile;
					RaiseTileMoveEvent();
				}
			}
		}

		public void RaiseTileMoveEvent()
		{
			if (lastTile != -1)
			{
				TileMouseMove?.Invoke(this, new TileEventArgs(MouseButtons.None, TileEventStatus.None, lastTile % 256, lastTile / 256));
			}
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			currentTileIndex = -1;
			lastTile = -1;
			OnMouseEvent(e, TileEventStatus.MouseDown);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (ClientRectangle.Contains(e.Location))
			{
				OnMouseEvent(e, TileEventStatus.MouseMove);
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			currentTileIndex = -1;
			lastTile = -1;
			OnMouseEvent(e, TileEventStatus.MouseUp);
		}

		public void SetZoom(int zoomLevel)
		{
			Height = 512 * zoomLevel;
			Width = 4096 * zoomLevel;
			zoom = zoomLevel;
			selection.SetZoom(zoomLevel);
			Invalidate();
		}

		public void ClearTileCache()
		{
			Array.Clear(invalidTiles, 0, invalidTiles.Length);
		}

		#region Selection

		public void CopySelection()
		{
			selection.CopySelection(CopyTileAt);
		}

		public bool PasteSelection()
		{
			if (selection.PasteSelection(PasteTileAt))
			{
				RaiseTileMoveEvent();
				Invalidate();
				return true;
			}

			return false;
		}

		public bool CutSelection()
		{
			if (selection.CopySelection(CopyTileAt) && selection.DeleteSelection(SetTileAt, GetEmptyTile()))
			{
				RaiseTileMoveEvent();
				Invalidate();
				return true;
			}

			return false;
		}

		public bool DeleteSelection()
		{
			if (selection.DeleteSelection(SetTileAt, GetEmptyTile()))
			{
				RaiseTileMoveEvent();
				Invalidate();
				return true;
			}

			return false;
		}

		public void ClearSelection()
		{
			selection.ClearSelection();
		}

		public void StartSelection(int x, int y)
		{
			selection.StartSelection(x, y);
		}

		public void SetSelection(int x, int y)
		{
			selection.SetSelection(x, y);
		}

		public bool HasSelection
		{
			get
			{
				return selection.HasSelection;
			}
		}

		int PasteTileAt(int x, int y, int data)
		{
			if (x < 256 && y < 32)
			{
				return SetTileAt(x, y, data);
			}

			return data; //no changes
		}

		ClipboardData CopyTileAt(int x, int y)
		{
			return new ClipboardData { Tile = GetTileAt(x, y) };
		}

		int GetTileAt(int x, int y)
		{
			int dest = x + y * 256;
			return (Level.ObjectsData[dest] << 8) | Level.LevelData[dest + 0x1000];
		}

		int SetTileAt(int x, int y, int data)
		{
			int previous = GetTileAt(x, y);
			if (previous != data)
			{
				int dest = x + y * 256;
				Level.ObjectsData[dest] = (byte)(data >> 8);
				Level.LevelData[dest + 0x1000] = (byte)(data & 0xFF);
				invalidTiles[dest] = false;
			}

			return previous;
		}

		int GetEmptyTile()
		{
			return Level.GetEmptyTile(Level.Tiles16x16.Bits, 16, 8);
		}

		#endregion

		#region Undo

		public void StartChanges()
		{
			changes = new List<SelectionChange>();
		}

		public void AddChange(int x, int y)
		{
			changes.Add(new SelectionChange { X = x, Y = y, Data = GetTileAt(x, y) });
		}

		public void CommitChanges()
		{
			selection.AddChanges(changes);
		}

		public bool Undo()
		{
			if (selection.Undo(SetTileAt, GetTileAt))
			{
				RaiseTileMoveEvent();
				Invalidate();
				return true;
			}

			return false;
		}

		public bool Redo()
		{
			if (selection.Redo(SetTileAt, GetTileAt))
			{
				RaiseTileMoveEvent();
				Invalidate();
				return true;
			}

			return false;
		}

		public void ClearUndo()
		{
			selection.ClearUndo();
		}

		#endregion
	}
}
