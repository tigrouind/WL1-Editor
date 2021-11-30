using System;
using System.IO;
using System.Text;

namespace WLEditor
{
	public class Rom
	{
		byte[] data;

		int bank;

		public void Load(string filePath)
		{
			data = File.ReadAllBytes(filePath);
		}

		public void Save(string filePath)
		{
			File.WriteAllBytes(filePath, data);
		}
		
		#region Read/Write

		public byte ReadByte(int position)
		{
			if(position < 0x4000)
			{
				return data[position];
			}
			
			return data[position + (bank - 1) * 0x4000];
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
		
		public void WriteWordSwap(int position, ushort value)
		{
			WriteByte(position, (byte)((value >> 8) & 0xFF));
			WriteByte(position + 1, (byte)(value & 0xFF));			
		}

		public byte[] ReadBytes(int position, int size)
		{
			byte[] result = new byte[size];
			for(int i = 0 ; i < size ; i++)
			{
				result[i] = ReadByte(position + i);
			}

			return result;
		}
		
		#endregion

		#region CRC
		
		public bool CheckCRC()
		{
			return GetHeaderCRC() == ReadByte(0x14d) && GetGlobalCRC() == ReadWordSwap(0x14e);
		}

		public void FixCRC()
		{
			WriteByte(0x14d, GetHeaderCRC());
			WriteWordSwap(0x14e, GetGlobalCRC());
		}
		
		byte GetHeaderCRC()
		{
			ushort headerCRC = 0;
			for (int i = 0x134; i <= 0x14c; i++)
			{
				unchecked
				{
					headerCRC = (ushort)(headerCRC - data[i] - 1);
				}
			}
			
			return (byte)(headerCRC & 0xFF);
		}
		
		ushort GetGlobalCRC()
		{
			ushort globalCRC = 0;
			for (int i = 0 ; i < data.Length ; i++)
			{
				if(i != 0x14e && i != 0x14f) //skip CRC itself
				{
					unchecked
					{
						globalCRC += data[i];
					}
				}
			}
			
			return globalCRC;
		}
		
		#endregion
		
		public void ExpandTo1MB()
		{
			WriteByte(0x0147, 0x13); //MBC3+RAM+BATTERY
			WriteByte(0x0148, 0x05); //1MB
			
			if (data.Length == 512*1024)
			{
				Array.Resize(ref data, 1024*1024);
			}
		}

		public string Title
		{
			get
			{
				if(data.Length >= 0x150) //header size
				{
					return Encoding.ASCII.GetString(data, 0x134, 16).TrimEnd('\0');
				}
				
				return string.Empty;
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
