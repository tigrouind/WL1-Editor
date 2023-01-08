﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace WLEditor
{
	public class Selection
	{		
		int zoom;
		int tileSize;
				
		bool selection;
		Point selectionStart, selectionEnd;
		int selectionWidth, selectionHeight;
		readonly List<int> selectionData = new List<int>();
		public event EventHandler<SelectionEventArgs> InvalidatePictureBox;
		
		readonly List<List<SelectionChange>> undo = new List<List<SelectionChange>>();
		readonly List<List<SelectionChange>> redo = new List<List<SelectionChange>>();
		
		public Selection(int tileSize)
		{
			this.tileSize = tileSize;
		}
				
		public void SetZoom(int zoomLevel)
		{
			zoom = zoomLevel;
		}
				
		public void DrawSelection(Graphics g)
		{
			if (selection)
			{
				using (SolidBrush brush = new SolidBrush(Color.FromArgb(128, 255, 255, 0)))
				{
					var rect = GetSelectionRectangle();					
					g.FillRectangle(brush, rect);						
				}
			}
		}		

		void GetSelection(out Point start, out Point end)
		{
			int startX = Math.Min(selectionStart.X, selectionEnd.X);
			int startY = Math.Min(selectionStart.Y, selectionEnd.Y);
			int endX = Math.Max(selectionStart.X, selectionEnd.X);
            int endY = Math.Max(selectionStart.Y, selectionEnd.Y);
			
			start = new Point(startX, startY);
			end = new Point(endX, endY);
		}
		
		void InvalidatePicture()
		{
			InvalidatePictureBox(this, new SelectionEventArgs(GetSelectionRectangle()));
		}
		
		Rectangle GetSelectionRectangle()
		{
			Point start, end;
			GetSelection(out start, out end);
			return new Rectangle(
				start.X * tileSize * zoom, 
				start.Y * tileSize * zoom, 
				((end.X - start.X + 1) * tileSize) * zoom,
				((end.Y - start.Y + 1) * tileSize) * zoom);
		}

		public void CopySelection(Func<int, int, int> getTileAt)
		{
			if (selection)
			{
				selectionData.Clear();
				
				Point start, end;
				GetSelection(out start, out end);
				
				for(int y = start.Y ; y <= end.Y ; y++)
				{
					for(int x = start.X ; x <= end.X ; x++)	
					{
						selectionData.Add(getTileAt(x, y));
					}
				}
				
				selectionWidth = end.X - start.X + 1;
				selectionHeight = end.Y - start.Y + 1;
				ClearSelection();
			}
		}
		
		public void PasteSelection(Func<int, int, int, int> setTileAt)
		{
			if (selection)
			{
				if (selectionHeight > 0 && selectionWidth > 0)
				{
					var changes = new List<SelectionChange>();
					
					Point start, end;
					GetSelection(out start, out end);
					
					for (int ty = start.Y ; ty <= end.Y ; ty += selectionHeight)
					for (int tx = start.X ; tx <= end.X ; tx += selectionWidth)
					for (int y = 0 ; y < selectionHeight ; y++)
					for (int x = 0 ; x < selectionWidth ; x++)	
					{						
						int destX = tx + x;
						int destY = ty + y;
						if ((destX <= end.X && destY <= end.Y) || (start.X == end.X && start.Y == end.Y))
						{
							int data = selectionData[x + y * selectionWidth];
							int track = setTileAt(destX, destY, data);
							if (track != -1)
							{
								changes.Add(new SelectionChange { X = destX, Y =  destY, Data = track });
							}
						}
					}			

					AddChanges(changes);
				}
								
				ClearSelection();
			}		
		}
		
		public void StartSelection(int x, int y)
		{
			if (selection)
			{
				InvalidatePicture();
			}
			
			selectionStart = new Point(x, y);				
			selectionEnd = new Point(x, y);	
			selection = true;	
			
			if (selection)
			{
				InvalidatePicture();
			}
		}
		
		public void SetSelection(int x, int y)
		{
			if (selection)
			{
				InvalidatePicture();
			}
			
			selectionEnd = new Point(x, y);	
			
			if (selection)
			{
				InvalidatePicture();
			}
		}
		
		public void ClearSelection()
		{
			if (selection)
			{
				selection = false;
				InvalidatePicture();
			}
		}
		
		#region Undo
		
		public void ClearUndo()
		{
			redo.Clear();
			undo.Clear();
		}
		
		public void AddChanges(List<SelectionChange> changes)
		{
			if (changes.Count > 0)
			{
				undo.Add(changes);
				redo.Clear();
			}
		}
				
		public void Undo(Action<int, int, int> setTileAt, Func<int, int, int> getTileAt)
		{
			ApplyChanges(setTileAt, getTileAt, undo, redo);
		}
		
		public void Redo(Action<int, int, int> setTileAt, Func<int, int, int> getTileAt)
		{
			ApplyChanges(setTileAt, getTileAt, redo, undo);
		}
				
		void ApplyChanges(Action<int, int, int> setTileAt, Func<int, int, int> getTileAt, List<List<SelectionChange>> source, List<List<SelectionChange>> dest)
		{
			if (source.Count > 0)
			{
				var changes = new List<SelectionChange>();
				foreach(var tile in source.Last())
				{
					changes.Add(new SelectionChange { X = tile.X, Y = tile.Y, Data = getTileAt(tile.X, tile.Y) } );
					setTileAt(tile.X, tile.Y, tile.Data);
				}
									
				source.RemoveAt(source.Count - 1);	
				dest.Add(changes);				
			}
		}
		
		#endregion
	}	
}