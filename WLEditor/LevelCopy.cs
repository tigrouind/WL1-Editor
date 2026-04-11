using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace WLEditor
{
	public class LevelCopy
	{
		public static void Copy(Rom rom, int currentCourseId, LevelPictureBox levelPictureBox)
		{
			//copy tiles
			levelPictureBox.StartSelection(0, 0);
			levelPictureBox.SetSelection(255, 31);
			var data = levelPictureBox.CopySelection(ClipboardType.LEVEL);
			levelPictureBox.ClearSelection();

			//warps
			bool hasCheckpoint = Sector.GetLevelHeader(rom, currentCourseId) != Sector.GetCheckpoint(rom, currentCourseId);

			data.LevelHeader = Sector.GetLevelHeader(rom, currentCourseId, false);
			data.Checkpoint = hasCheckpoint ? Sector.GetLevelHeader(rom, currentCourseId, true) : null;
			data.Warps = [.. GetWarps()];
			data.Scroll = [.. Enumerable.Range(0, 32).Select(x => (byte)Sector.GetScroll(rom, currentCourseId, x))];
			data.Music = Sector.GetMusic(rom, currentCourseId);

			Clipboard.Copy(data, ClipboardType.LEVEL);

			IEnumerable<(int WarpType, Warp Warp)> GetWarps()
			{
				for (int i = 0; i < 32; i++)
				{
					int warp = Sector.GetWarp(rom, currentCourseId, i);
					yield return (warp, warp >= 0x5B7A ? Sector.GetWarp(rom, warp) : null);
				}
			}
		}

		public static bool Paste(Rom rom, int currentCourseId, LevelPictureBox levelPictureBox, string text)
		{
			var data = Clipboard.Paste(ClipboardType.LEVEL);
			if (data == null //should never happen
				|| !CheckFree()
				|| MessageBox.Show("Are you sure you want to paste all blocks into current level ?\r\n" +
					"This will replace all of it's content. This action can't be undone.", text, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) != DialogResult.Yes)
			{
				return false;
			}

			//tiles
			levelPictureBox.StartSelection(0, 0);
			levelPictureBox.PasteSelection(ClipboardType.LEVEL);

			Sector.SaveLevelHeader(rom, currentCourseId, false, data.LevelHeader);
			Sector.SaveMusic(rom, currentCourseId, data.Music);
			SetCheckpoint(data.Checkpoint);
			SetWarps(data.Warps);
			SetScroll(data.Scroll);
			levelPictureBox.History.ClearUndo();

			return true;

			bool CheckFree()
			{
				bool hasCheckpoint = Sector.GetLevelHeader(rom, currentCourseId) != Sector.GetCheckpoint(rom, currentCourseId);
				if (data.Checkpoint != null && !hasCheckpoint)
				{
					int header = Sector.GetFreeCheckpoint(rom, currentCourseId);
					if (header == -1)
					{
						MessageBox.Show("No more checkpoint available.\r\nPlease free a checkpoint in another level", text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
						return false;
					}
				}

				int free = 370 - Sector.GetWarpUsage(rom).Count() +
					Enumerable.Range(0, 32)
						.Select(x => Sector.GetWarp(rom, currentCourseId, x))
						.Count(x => x >= 0x5B7A); //warps in current level
				int warpsNeeded = data.Warps.Count(x => x.Type >= 0x5B7A);

				if (free < warpsNeeded)
				{
					MessageBox.Show($"No more warps available.\r\nPlease free {warpsNeeded - free} warp(s) in another level", text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return false;
				}

				return true;
			}

			void SetWarps((int WarpType, Warp Warp)[] warps)
			{
				foreach (var sector in Enumerable.Range(0, 32).Reverse()) //remove all warps
				{
					Sector.FreeWarp(rom, currentCourseId, sector);
				}

				foreach (var (index, warp) in warps.Select((x, i) => (Index: i, Warp: x)))
				{
					if (warp.WarpType >= 0x5B7A) //sector
					{
						var freeWarp = Sector.GetFreeWarp(rom);
						Sector.SaveWarp(rom, freeWarp, warp.Warp);
						Sector.SaveWarp(rom, currentCourseId, index, freeWarp);
					}
					else //exit or none
					{
						Sector.SaveWarp(rom, currentCourseId, index, warp.WarpType);
					}
				}
			}

			void SetCheckpoint(Warp checkpoint)
			{
				Sector.FreeCheckpoint(rom, currentCourseId);

				if (checkpoint != null)
				{
					int header = Sector.GetFreeCheckpoint(rom, currentCourseId);
					Sector.SaveCheckpoint(rom, currentCourseId, header);
					Sector.SaveLevelHeader(rom, currentCourseId, true, checkpoint);
				}
			}

			void SetScroll(byte[] scroll)
			{
				for (int i = 0; i < 32; i++)
				{
					Sector.SaveScroll(rom, currentCourseId, i, scroll[i]);
				}
			}
		}
	}
}
