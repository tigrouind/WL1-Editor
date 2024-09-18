using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

public class DirectBitmap : IDisposable
{
	public readonly Bitmap Bitmap;
	public readonly uint[] Bits;
	public readonly int Height;
	public readonly int Width;

	readonly GCHandle bitsHandle;
	bool disposed;

	public DirectBitmap(int width, int height)
	{
		Width = width;
		Height = height;
		Bits = new uint[width * height];
		bitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
		Bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppPArgb, bitsHandle.AddrOfPinnedObject());
	}

	public void Dispose()
	{
		if (!disposed)
		{
			disposed = true;
			Bitmap.Dispose();
			bitsHandle.Free();
		}
	}
}