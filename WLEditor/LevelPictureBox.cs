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
		readonly DirectBitmap levelTiles = new(4096, 512);
		int zoom;

		public bool ShowSectors = true;
		public bool ShowObjects = true;
		public int CurrentSector = -1;
		int currentTileIndex = -1;
		int lastTile = -1;
		public int ScrollLines;
		int mouseDownSector;
		readonly Selection selection = new(16);
		readonly History history = new();
		List<SelectionChange> changes = [];

		public event EventHandler<TileEventArgs> TileMouseDown;
		public event EventHandler<TileEventArgs> TileMouseMove;
		public event EventHandler SectorChanged;
		public Func<int, int> GetSourceSector;

		public readonly static Brush[] TransparentBrushes =
		[
			new SolidBrush(Color.FromArgb(64, 128, 64, 0)),  //brown
			new SolidBrush(Color.FromArgb(64, 0, 255, 0)),   //green
			new SolidBrush(Color.FromArgb(64, 0, 255, 255)), //light blue
			new SolidBrush(Color.FromArgb(64, 255, 255, 0)), //yellow
			new SolidBrush(Color.FromArgb(64, 0, 0, 255)),   //blue
			new SolidBrush(Color.FromArgb(64, 255, 128, 0)),   //amber
			new SolidBrush(Color.FromArgb(128, 0, 128, 0)),  //dark green
			new SolidBrush(Color.FromArgb(128, 128, 64, 128)),  //purple
		];

		public static Brush EnemyBrush = new SolidBrush(Color.FromArgb(255, 50, 50, 155));

		public LevelPictureBox()
		{
			selection.InvalidateSelection += (s, e) => Invalidate(e.ClipRectangle);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (Level.LevelData != null && !DesignMode)
			{
				using StringFormat format = new();
				using Font font = new("Arial", 8 * zoom);
				using Graphics g = Graphics.FromImage(levelTiles.Bitmap);
				format.LineAlignment = StringAlignment.Center;
				format.Alignment = StringAlignment.Center;

				//draw tiles to cache
				DrawTiles();

				//draw tiles from cache
				e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
				e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
				e.Graphics.DrawImage(levelTiles.Bitmap, 0, 0, 4096 * zoom, 512 * zoom);

				//sector objects (enemies, powerups)
				if (ShowObjects)
				{
					DrawObjects(font, format);

					//wario position
					int index = Level.WarioRightFacing ? 0 : 1;
					Rectangle playerRectangle = Sprite.PlayerRects[index].Rectangle;
					Point playerOffset = Sprite.PlayerRects[index].Offsets;

					Rectangle destRect = new(
						(Level.WarioPosition % 4096 + playerOffset.X) * zoom,
						(Level.WarioPosition / 4096 + playerOffset.Y) * zoom, playerRectangle.Width * zoom, playerRectangle.Height * zoom);

					if (destRect.IntersectsWith(e.ClipRectangle))
					{
						e.Graphics.DrawImage(Sprite.PlayerSprite.Bitmap, destRect, playerRectangle, GraphicsUnit.Pixel);
					}
				}

				if (ShowSectors)
				{
					DrawCamera();
				}

				selection.DrawSelection(e.Graphics);

				//sectors
				if (ShowSectors)
				{
					DrawSectors(font, format);
					DrawSelectedSector();
				}

				DrawScrollLines();
			}

			void DrawObjects(Font font, StringFormat format)
			{
				var positions = new List<(int, int, byte, Rectangle)>();
				for (int j = 0; j < 32; j++)
				{
					for (int i = 0; i < 256; i++)
					{
						byte data = Level.ObjectsData[i + j * 256];
						if (data > 0)
						{
							Rectangle rect = new(i * 16 * zoom, j * 16 * zoom, 16 * zoom, 16 * zoom);
							positions.Add((i, j, data, rect));
						}
					}
				}

				foreach ((int i, int j, byte data, Rectangle rect) in positions)
				{
					if (rect.IntersectsWith(e.ClipRectangle))
					{
						e.Graphics.FillRectangle(EnemyBrush, rect);
					}
				}

				foreach ((int i, int j, byte data, Rectangle rect) in positions)
				{
					if (data > 6) //power up
					{
						if (rect.IntersectsWith(e.ClipRectangle))
						{
							e.Graphics.DrawImage(Sprite.TilesObjects.Bitmap, rect, new Rectangle((data - 7) * 16, 0, 16, 16), GraphicsUnit.Pixel);
						}
					}
					else //enemy
					{
						var (enemyRect, enemyOffset) = Sprite.LoadedSprites[data - 1];
						if (enemyRect != default)
						{
							var destRect = new Rectangle((i * 16 + enemyOffset.X) * zoom, (j * 16 + enemyOffset.Y) * zoom, enemyRect.Width * zoom, enemyRect.Height * zoom);
							if (destRect.IntersectsWith(e.ClipRectangle))
							{
								e.Graphics.DrawImage(Sprite.TilesEnemies.Bitmap, destRect, enemyRect, GraphicsUnit.Pixel);
							}
						}
						else if (rect.IntersectsWith(e.ClipRectangle))
						{
							e.Graphics.DrawString(data.ToString(), font, Brushes.White, rect, format);
						}
					}
				}
			}

			void DrawTiles()
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
						}
					}
				}

				void DrawTileToBitmap(int x, int y)
				{
					byte tileIndex = Level.LevelData[x + y * 256 + 0x1000];
					Point dest = new(x * 16, y * 16);
					Point src = new((tileIndex % 8) * 16, (tileIndex / 8) * 16);

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

					var dest = new Rectangle(x * 256 * zoom, y * 256 * zoom, 16 * zoom, 16 * zoom);
					e.Graphics.FillRectangle(EnemyBrush, dest);
					e.Graphics.DrawString(drawSector.ToString("D2"), font, Brushes.White, dest, format);

					int sectorTarget = Level.Warps[drawSector];
					if (sectorTarget != 255)
					{
						string text = GetWarpName();
						var result = TextRenderer.MeasureText(text, font);

						dest = new Rectangle((x * 256 + 20) * zoom, y * 256 * zoom, result.Width, 16 * zoom);
						e.Graphics.FillRectangle(EnemyBrush, dest);
						e.Graphics.DrawString(text, font, Brushes.White, dest, format);
					}

					string GetWarpName()
					{
						string warpText = sectorTarget switch
						{
							32 => "EXIT MAP",
							33 => "EXIT A",
							34 => "EXIT B",
							_ => "S" + sectorTarget.ToString("D2"),
						};
						return warpText;
					}
				}

				void DrawSectorBorders()
				{
					//draw sector borders
					using Pen penBlue = new(EnemyBrush, 2.0f * zoom);
					penBlue.DashPattern = [5.0f, 1.0f];

					for (int i = 1; i < 16; i++)
					{
						int x = i * 256 * zoom;
						Rectangle lineRect = new(x - zoom, 0, 2 * zoom, 512 * zoom);
						if (lineRect.IntersectsWith(e.ClipRectangle))
						{
							e.Graphics.DrawLine(penBlue, x, 0, x, 512 * zoom);
						}
					}

					e.Graphics.DrawLine(penBlue, 0, 256 * zoom, 4096 * zoom, 256 * zoom);
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
					Rectangle camera = new((Level.CameraPosition % 4096) * zoom, (Level.CameraPosition / 4096) * zoom, 10 * 16 * zoom, 9 * 16 * zoom);
					if (camera.IntersectsWith(e.ClipRectangle))
					{
						using Brush red = new SolidBrush(Color.FromArgb(128, 0, 0, 255));
						e.Graphics.FillRectangle(red, camera);
					}
				}
			}

			void DrawSelectedSector()
			{
				if (CurrentSector != -1)
				{
					Rectangle sectorRect = new((CurrentSector % 16) * 256 * zoom, (CurrentSector / 16) * 256 * zoom, 256 * zoom, 256 * zoom);
					if (sectorRect.IntersectsWith(e.ClipRectangle))
					{
						using Brush blue = new SolidBrush(Color.FromArgb(64, 0, 0, 255));
						e.Graphics.FillRectangle(blue, sectorRect);
					}
				}
			}

			void DrawScrollLines()
			{
				if (ScrollLines != 0)
				{
					int positionY = new[] { 23, 15, 7, -1 }[ScrollLines - 1];
					int lower = (positionY + 9) * 16;
					int upper = positionY * 16;

					using (Brush brush = new SolidBrush(Color.FromArgb(192, 50, 25, 0)))
					{
						e.Graphics.FillRectangle(brush, 0, 0, 4096 * zoom, upper * zoom);
						e.Graphics.FillRectangle(brush, 0, lower * zoom, 4096 * zoom, (32 * 16 - lower) * zoom);
					}

					using Pen pen = new(Color.Yellow, 2.0f * zoom);
					pen.DashPattern = [5.0f, 1.0f];
					e.Graphics.DrawLine(pen, 0, upper * zoom, 4096 * zoom, upper * zoom);
					if (ScrollLines > 1)
					{
						e.Graphics.DrawLine(pen, 0, lower * zoom, 4096 * zoom, lower * zoom);
					}
				}
			}

			Rectangle GetClipRectangle(Rectangle clipRectangle, int size)
			{
				Point start = new(clipRectangle.Left / size, clipRectangle.Top / size);
				Point end = new((clipRectangle.Right - 1) / size + 1, (clipRectangle.Bottom - 1) / size + 1);
				return new Rectangle(start.X, start.Y, end.X - start.X, end.Y - start.Y);
			}
		}

		public void InvalidateTile(int tileIndex)
		{
			using Region r = new(new Rectangle((tileIndex % 256) * 16 * zoom, (tileIndex / 256) * 16 * zoom, 16 * zoom, 16 * zoom));
			invalidTiles[tileIndex] = false;
			Invalidate(r);
		}

		public void InvalidateAnimatedTiles()
		{
			using Region r = new(Rectangle.Empty);
			for (int tileIndex = 0; tileIndex < 8192; tileIndex++)
			{
				byte tileData = Level.LevelData[tileIndex + 0x1000];
				if (Level.Animated16x16Tiles[tileData])
				{
					r.Union(new Rectangle((tileIndex % 256) * 16 * zoom, (tileIndex / 256) * 16 * zoom, 16 * zoom, 16 * zoom));
					invalidTiles[tileIndex] = false;
				}
			}

			Invalidate(r);
		}

		public void InvalidateObject(int tileIndex, int currentObject, int previousObject)
		{
			using Region r = new(new Rectangle((tileIndex % 256) * 16 * zoom, (tileIndex / 256) * 16 * zoom, 16 * zoom, 16 * zoom));

			var previous = GetEnemyRectangle(tileIndex, previousObject, zoom);
			if (previous != Rectangle.Empty) r.Union(previous);

			var current = GetEnemyRectangle(tileIndex, currentObject, zoom);
			if (current != Rectangle.Empty) r.Union(current);

			Invalidate(r);

			static Rectangle GetEnemyRectangle(int tileIndex, int enemyIndex, int zoom = 1)
			{
				if (enemyIndex >= 1 && enemyIndex <= 6)
				{
					var (enemyRect, enemyOffset) = Sprite.LoadedSprites[enemyIndex - 1];
					return new Rectangle(
						(tileIndex % 256 * 16 + enemyOffset.X) * zoom,
						(tileIndex / 256 * 16 + enemyOffset.Y) * zoom,
						enemyRect.Width * zoom, enemyRect.Height * zoom);
				}

				return Rectangle.Empty;
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
					TileMouseDown?.Invoke(this, new TileEventArgs(e.Button, status, tilePosX, tilePosY));
				}
			}
			else if (e.Button == MouseButtons.Middle)
			{
				Point coordinates = e.Location;
				int sector = e.Location.X / 256 / zoom + (e.Location.Y / 256 / zoom) * 16;

				if (ModifierKeys.HasFlag(Keys.Shift) || ModifierKeys.HasFlag(Keys.Control))
				{
					sector = GetSourceSector(sector);
				}

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
					SectorChanged?.Invoke(this, EventArgs.Empty);
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
			if (selection.HasSelection)
			{
				history.AddChanges(selection.PasteSelection(PasteTileAt));
				RaiseTileMoveEvent();
				Invalidate();
				return true;
			}

			return false;
		}

		public bool CutSelection()
		{
			if (selection.HasSelection)
			{
				selection.CopySelection(CopyTileAt);
				history.AddChanges(selection.DeleteSelection(SetTileAt, GetEmptyTile()));

				RaiseTileMoveEvent();
				Invalidate();
				return true;
			}

			return false;
		}

		public bool DeleteSelection()
		{
			if (selection.HasSelection)
			{
				history.AddChanges(selection.DeleteSelection(SetTileAt, GetEmptyTile()));
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

		public bool HasSelection => selection.HasSelection;

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

		static int GetEmptyTile()
		{
			return Level.GetEmptyTile(Level.Tiles16x16.Bits, 16, 8);
		}

		#endregion

		#region Undo

		public void StartChanges()
		{
			changes = [];
		}

		public void AddChange(int x, int y)
		{
			changes.Add(new SelectionChange { X = x, Y = y, Data = GetTileAt(x, y) });
		}

		public void CommitChanges()
		{
			history.AddChanges(changes);
		}

		public bool Undo()
		{
			if (history.Undo(SetTileAt, GetTileAt))
			{
				RaiseTileMoveEvent();
				Invalidate();
				return true;
			}

			return false;
		}

		public bool Redo()
		{
			if (history.Redo(SetTileAt, GetTileAt))
			{
				RaiseTileMoveEvent();
				Invalidate();
				return true;
			}

			return false;
		}

		public void ClearUndo()
		{
			history.ClearUndo();
		}

		#endregion

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				levelTiles.Dispose();
			}
		}
	}
}
