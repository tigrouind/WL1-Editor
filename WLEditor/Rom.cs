using System;

namespace WLEditor
{
	public class Rom
	{
		private byte[] data;
		
		private int bank;
		
		public void Load(string filePath)
		{
			data = System.IO.File.ReadAllBytes(filePath);
		}
		
		public void Save(string filePath)
		{
			System.IO.File.WriteAllBytes(filePath, data);
		}
		
		public byte ReadByte(int position)
		{
			if(position < 0x4000)
			{
				return data[position];
			}
			else
			{
				return data[position + (bank - 1) * 0x4000];
			}			
		}
		
		public void WriteByte(int position, byte value)
		{
			if(position < 0x4000)
			{
				data[position] = value;
			}
			else
			{
				data[position + (bank - 1) * 0x4000] = value;
			}
		}
		
		public void WriteBytes(int position, params byte[] data)
		{			
			for(int i = 0 ; i < data.Length ; i++)
			{
				WriteByte(position + i, data[i]);
			}
		}
		
		public ushort ReadWord(int position)
		{
			return (ushort)(ReadByte(position) | ReadByte(position + 1) << 8);
		}
		
		public ushort ReadWordSwap(int position)
		{
			return (ushort)(ReadByte(position) << 8 | ReadByte(position + 1));
		}
		
		
		public void WriteWord(int position, ushort value)
		{
			WriteByte(position, (byte)(value & 0xFF));
			WriteByte(position + 1, (byte)((value >> 8) & 0xFF));
		}
		
		public byte[] ReadBytes(int position, int size)
		{
			byte[] data = new byte[size]; 
			for(int i = 0 ; i < size ; i++)
			{
				data[i] = ReadByte(position + i);
			}

			return data;			
		}
		
		public void FixCRC()
		{
			//fix global CRC
			ushort globalCRC = 0;
			for (int i = 0 ; i < data.Length ; i++)
			{
				if(i != 0x14e && i != 0x14f)
				{
					unchecked
					{
						globalCRC += data[i];
					}
				}
			}
			data[0x14e] = (byte)(globalCRC >> 8);
			data[0x14f] = (byte)(globalCRC & 0xFF);
			
			//fix header CRC
			ushort headerCRC = 0;
			for (int i = 0x134; i <= 0x14c; i++)
		    {
				unchecked
				{
					headerCRC = (ushort)(headerCRC - data[i] - 1);
				}
		    }
			data[0x14d] = (byte)(headerCRC & 0xFF);
		}
		
		public void ExpandTo1MB()
		{		
			data[0x0147] = 0x13; //MBC3+RAM+BATTERY
			data[0x0148] = 0x05; //1MB
			if(data.Length == 512*1024) 
			{
				Array.Resize(ref data, 1024*1024);	
			}			
		}
		
		public string Title
		{
			get
			{
				return System.Text.Encoding.ASCII.GetString(data, 0x134, 16).TrimEnd('\0');
			}
		}
		
		public void SetBank(int rombank)
		{
			bank = rombank;
		}
		
		public bool IsLoaded
		{
			get
			{
				return data != null;
			}
		}
	}
}
