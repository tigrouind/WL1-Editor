using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WLEditor
{
	public class EventForm
	{
		List<(int X, int Y, byte Index)>[] worldEvents;
		List<(int X, int Y, byte Index)> worldEvent;
		int eventStep;
		public readonly HistorySaveState<EventHistory> History;
		readonly PictureBox pictureBox;
		readonly DirectBitmap tilesWorld8x8;
		readonly Selection selection;

		int zoom;
		int currentWorld;

		public event EventHandler EventChanged;
		public Action UpdateTitle;

		public EventForm(PictureBox pictureBox, DirectBitmap tilesWorld8x8, Selection selection)
		{
			this.pictureBox = pictureBox;
			this.tilesWorld8x8 = tilesWorld8x8;
			this.selection = selection;

			History = new(Serialize, Deserialize);
		}

		readonly (int MaxSize, int[][] Offsets)[] events =
		[
			(150, new []
			{
				new [] { 0x68B9, 0x68C5, 0x68D1, 0x68DD, 0x68E9, 0x68F5 }, //applied when map is show    (ld bc, XXXX ld hl, XXXX)
				[0x690F, 0x691B, 0x6927, 0x6933, 0x693F, 0x694B], //applied after beating level (ld bc, XXXX ld hl, XXXX)
				[0x770B, 0x770F, 0x7713, 0x7717, 0x771B],         //step by step animation for next path
			}),
			(26, new []
			{
				new [] { 0x68DD },
				[0x6933],
				[0x7717],
			}),
			(250, new []
			{
				new [] { 0x69FB, 0x6A07, 0x6A13, 0x6A1F, 0x6A2B, 0x6A37, 0x6A43, 0x6A4F },
				[0x6A5C, 0x6A68, 0x6A74, 0x6A80, 0x6A8C, 0x6A98],
				[0x7743, 0x7747, 0x774B, 0x774F, 0x7753, 0x7757],
			}),
			(114, new []
			{
				new [] { 0x702F, 0x703B, 0x7047, 0x7053, 0x705F, 0x706B },
				[0x7084, 0x7090, 0x709C, 0x70A8, 0x70B4, 0x70C0],
				[0x77EB, 0x77EF, 0x77F3, 0x77F7, 0x77FB, 0x77FF],
			}),
			(144, new []
			{
				new [] { 0x6BAB, 0x6BB7, 0x6BC3, 0x6BCF, 0x6BDB, 0x6BED },
				[0x6C00, 0x6C0C, 0x6C18, 0x6C24, 0x6C30],
				[0x777F, 0x7783, 0x7787, 0x778B, 0x778F],
			}),
			(91, new []
			{
				new [] { 0x6CE0, 0x6CEC, 0x6CF8, 0x6D04, 0x6D10 },
				[0x6D2F, 0x6D3B, 0x6D47, 0x6D53],
				[0x77B7, 0x77BB, 0x77BF, 0x77C3],
			}),
			(416, new []
			{
				new [] { 0x6DD3, 0x6DDF, 0x6DEB, 0x6DF7, 0x6E03, 0x6E0F, 0x6E27 },
				[0x6E34, 0x6E40, 0x6E4C, 0x6E58, 0x6E64, 0x0000, 0x6E82],
				[0x7827, 0x782B, 0x782F, 0x7833, 0x7837],
			}),
			(252, new []
			{
				new [] { 0x714B, 0x7157, 0x7163 },
				[0x7173, 0x717F, 0x718B],
				[0x785F],
			}),
			(0, new []
			{
				new int[] { },
				[],
				[],
			}),
			(71, new []
			{
				new [] { 0x66C6 }
			})
		];

		readonly (int Tile, int Position)[] eventAddressOffset =
		[
			( 1, 4 ),
			( 1, 4 ),
			( 0, 2 )
		];

		public void SetZoom(int zoomLevel)
		{
			zoom = zoomLevel;
		}

		public void SetChanges()
		{
			EventChanged(this, EventArgs.Empty);
		}

		public void LoadWorld(Rom rom, int world)
		{
			currentWorld = world;
			worldEvents = Overworld.LoadEvents(rom, events[currentWorld].Offsets[0]);
			eventStep = 0;
			worldEvent = worldEvents[0];
		}

		public bool SaveEvents(Rom rom, out string message)
		{
			return Overworld.SaveEvents(rom, worldEvents, events[currentWorld].Offsets, eventAddressOffset,
				events[currentWorld].MaxSize, out message);
		}

		public string GetTitle()
		{
			int eventId = Array.IndexOf(worldEvents, worldEvent);
			return eventStep == 0 ? $"Event {eventId + 1}" : $"Event {eventId + 1} / Step {eventStep}";
		}

		#region Commands

		public bool ProcessEventKey(Keys key)
		{
			var keyData = key & Keys.KeyCode;
			switch (keyData)
			{
				case Keys.PageUp:
					NextEvent();
					return true;

				case Keys.PageDown:
					PreviousEvent();
					return true;

				case Keys.Home:
					NextStep();
					return true;

				case Keys.End:
					PreviousStep();
					return true;
			}

			return false;
		}

		#endregion

		#region Events

		public bool CanPreviousEvent => worldEvent != worldEvents[0] || eventStep > 0;
		public bool CanNextEvent => worldEvent != worldEvents[^1] || eventStep < worldEvent.Count;
		public bool CanPreviousStep => eventStep > 0 || worldEvent != worldEvents[0];
		public bool CanNextStep => eventStep < worldEvent.Count || worldEvent != worldEvents[^1];

		public void PreviousEvent()
		{
			if (worldEvent != worldEvents[0])
			{
				History.SaveState();
				PreviousEventInternal();
				eventStep = 0;
			}
			else
			{
				History.SaveState();
				eventStep = 0;
			}

			UpdateTitle();
			pictureBox.Invalidate();
		}

		public void NextEvent()
		{
			if (worldEvent != worldEvents[^1])
			{
				History.SaveState();
				NextEventInternal();
				eventStep = 0;
			}
			else
			{
				History.SaveState();
				eventStep = worldEvent.Count;
			}

			UpdateTitle();
			pictureBox.Invalidate();
		}

		public void NextStep()
		{
			if (eventStep < worldEvent.Count)
			{
				History.SaveState();
				eventStep++;
			}
			else if (worldEvent != worldEvents[^1])
			{
				History.SaveState();
				NextEventInternal();
				eventStep = 0;
			}

			UpdateTitle();
			pictureBox.Invalidate();
		}

		public void PreviousStep()
		{
			if (eventStep > 0)
			{
				History.SaveState();
				eventStep--;
			}
			else if (worldEvent != worldEvents[0])
			{
				History.SaveState();
				PreviousEventInternal();
				eventStep = worldEvent.Count;
			}

			UpdateTitle();
			pictureBox.Invalidate();
		}

		void PreviousEventInternal()
		{
			int eventId = Array.IndexOf(worldEvents, worldEvent);
			worldEvent = worldEvents[eventId - 1];
		}

		void NextEventInternal()
		{
			int eventId = Array.IndexOf(worldEvents, worldEvent);
			worldEvent = worldEvents[eventId + 1];
		}

		#endregion

		public bool CanDelete => eventStep > 0;
		public bool CanDeleteAll => worldEvent.Count > 0;

		public void Delete()
		{
			if (selection.HasSelection)
			{
				var rect = selection.GetSelection();
				var events = worldEvent.Where(x => rect.Contains(x.X, x.Y)).ToArray();
				if (events.Any())
				{
					History.SaveState();

					eventStep -= events.Count(x => worldEvent.IndexOf(x) < eventStep);
					worldEvent.RemoveAll(x => events.Contains(x));

					pictureBox.Invalidate();
					UpdateTitle();
					SetChanges();
				}

				selection.ClearSelection();
			}
			else if (eventStep > 0)
			{
				History.SaveState();
				var (x, y, Index) = worldEvent[eventStep - 1];

				worldEvent.RemoveAt(eventStep - 1);
				eventStep--;

				pictureBox.Invalidate();
				UpdateTitle();
				SetChanges();
			}
		}

		public void DeleteAll()
		{
			if (worldEvent.Count > 0)
			{
				History.SaveState();

				worldEvent.Clear();
				eventStep = 0;

				pictureBox.Invalidate();
				UpdateTitle();
				SetChanges();
			}
		}

		public int FindEvent(int posx, int posy)
		{
			return worldEvent.FindIndex(x => x.X == posx && x.Y == posy);
		}

		public int GetEvent(int index)
		{
			if (index != -1)
			{
				return worldEvent[index].Index;
			}

			return -1;
		}

		public void AddEvent(byte tileData, int posx, int posy)
		{
			int index = worldEvent.FindIndex(x => x.X == posx && x.Y == posy);
			if (index == -1)
			{
				History.SaveStateOnce();
				worldEvent.Insert(eventStep, (posx, posy, tileData));
				eventStep++;
			}
			else
			{
				History.SaveStateOnce();
				worldEvent[index] = (posx, posy, tileData); //update
			}

			pictureBox.Invalidate();
			UpdateTitle();
		}

		public void RemoveEvent(int posx, int posy)
		{
			int index = worldEvent.FindIndex(x => x.X == posx && x.Y == posy);
			if (index >= 0)
			{
				History.SaveStateOnce();
				worldEvent.RemoveAt(index);
				if (index <= (eventStep - 1))
				{
					eventStep--;
				}

				pictureBox.Invalidate();
				UpdateTitle();
			}
		}

		public void DrawEvents(Graphics graphics, bool drawRects = true)
		{
			using var brushEvent = new SolidBrush(Color.FromArgb(128, 255, 128, 0));
			using var brushStep = new SolidBrush(Color.FromArgb(128, 64, 192, 192));
			foreach (var worldEv in worldEvents)
			{
				bool selected = worldEv == worldEvent;
				for (int i = 0; i < worldEv.Count; i++)
				{
					var (x, y, tileIndex) = worldEv[i];

					if (i >= eventStep && selected)
					{
						if (drawRects)
						{
							graphics.FillRectangle(brushEvent, new Rectangle(x * 8 * zoom, y * 8 * zoom, 8 * zoom, 8 * zoom));
						}
					}
					else
					{
						int srcx = tileIndex % 16;
						int srcy = tileIndex / 16;
						graphics.DrawImage(tilesWorld8x8.Bitmap,
										new Rectangle(x * 8 * zoom, y * 8 * zoom, 8 * zoom, 8 * zoom),
										new Rectangle(srcx * 8, srcy * 8, 8, 8), GraphicsUnit.Pixel);

						if (selected && drawRects)
						{
							graphics.FillRectangle(brushStep, new Rectangle(x * 8 * zoom, y * 8 * zoom, 8 * zoom, 8 * zoom));
						}
					}
				}

				if (selected)
				{
					break;
				}
			}
		}

		#region Undo / redo

		EventHistory Serialize()
		{
			var cloner = new Cloner();
			return cloner.Clone(new EventHistory
			{
				WorldEvent = [.. worldEvent],
				WorldEventIndex = Array.IndexOf(worldEvents, worldEvent),
				EventStep = eventStep
			});
		}

		void Deserialize(EventHistory history)
		{
			worldEvent = [.. history.WorldEvent];
			worldEvents[history.WorldEventIndex] = worldEvent;
			eventStep = history.EventStep;

			UpdateTitle();
			pictureBox.Invalidate();
		}

		#endregion
	}
}
