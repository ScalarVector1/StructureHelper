using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace StructureHelper.Models
{
	public unsafe interface ITileDataEntry
	{
		/// <summary>
		/// Returns the pointer to the backing data of this TileDataEntry
		/// </summary>
		/// <returns></returns>
		public void* GetRawPtr();

		/// <summary>
		/// Sets the data for this TileDataEntry from a byte array, such as one read from a binary file
		/// </summary>
		/// <param name="data">The raw data to populate this entry with</param>
		/// <exception cref="ArgumentException"></exception>
		public void SetData(byte[] data);

		/// <summary>
		/// Gets the raw data of this tile data entry as a byte array.
		/// </summary>
		/// <returns>The raw data as a byte array</returns>
		public byte[] GetData();

		/// <summary>
		/// Gets a single entry at this location in the data. Returns as a void pointer,
		/// you will have to derefference this yourself.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public void* GetSingleEntry(int x, int y);

		/// <summary>
		/// Returns the raw size of this tile data entry, in bytes.
		/// </summary>
		/// <returns>The total size of this data entry in bytes</returns>
		public int GetRawSize();

		/// <summary>
		/// Returns the size in bytes of one column of this data
		/// </summary>
		/// <returns></returns>
		public int GetColumnSize();

		/// <summary>
		/// Returns the size in bytes of a single entry of this data
		/// </summary>
		/// <returns></returns>
		public int GetSingleSize();

		/// <summary>
		/// Imports a column of data from the world into this structure
		/// </summary>
		/// <param name="source">The topmost point of the column</param>
		/// <param name="columnIdx">The index of the column to read into</param>
		public void ImportColumn(void* source, int columnIdx);

		/// <summary>
		/// Copies an entire column of data from this data entry into the world.
		/// </summary>
		/// <param name="target">The pointer to the topmost tiles ITileData of this type to copy into</param>
		/// <param name="columnIdx">The column of data to place at that location</param>
		public void ExportColumn(void* target, int columnIdx);

		/// <summary>
		/// Copies a single entry of data into the world.
		/// </summary>
		/// <param name="target">The pointer to the tiles ITileData of this type to copy into</param>
		/// <param name="columnIdx">The X position to copy from</param>
		/// <param name="rowIdx">The Y position to copy from</param>
		public void ExportSingle(void* target, int columnIdx, int rowIdx);
	}

	public unsafe class TileDataEntry<T> : ITileDataEntry, IDisposable where T : unmanaged, ITileData
	{
		private readonly T[] rawData;
		private readonly T* rawDataPtr;

		private GCHandle rawDataHandle;

		public int length;
		public int colLength;

		public TileDataEntry(int length, int rowLength)
		{
			rawData = new T[length];
			rawDataHandle = GCHandle.Alloc(rawData, GCHandleType.Pinned);
			rawDataPtr = (T*)rawDataHandle.AddrOfPinnedObject().ToPointer();

			this.length = length;
			this.colLength = rowLength;
		}

		public void* GetRawPtr()
		{
			return rawDataPtr;
		}

		public void SetData(byte[] data)
		{
			if (data.Length != sizeof(T) * length)
				throw new ArgumentException($"Data size is inappropriate, Expected enough for {length} {typeof(T).Name} ({sizeof(T) * length} bytes), but got {data.Length}");

			fixed (byte* dataPtr = data)
			{
				Buffer.MemoryCopy(dataPtr, rawDataPtr, data.Length, data.Length);
			}
		}

		public byte[] GetData()
		{
			byte[] data = new byte[GetRawSize()];

			fixed (byte* dataPtr = data)
			{
				Buffer.MemoryCopy(rawDataPtr, dataPtr, data.Length, data.Length);
			}

			return data;
		}

		public void* GetSingleEntry(int x, int y)
		{
			return rawDataPtr + (x * colLength + y);
		}

		public int GetRawSize()
		{
			return sizeof(T) * length;
		}

		public int GetColumnSize()
		{
			return sizeof(T) * colLength;
		}

		public int GetSingleSize()
		{
			return sizeof(T);
		}

		public void ImportColumn(void* source, int colIdx)
		{
			long colSize = GetColumnSize();

			T* target = rawDataPtr + colIdx * colLength;
			Buffer.MemoryCopy(source, target, colSize, colSize);
		}

		public void ExportColumn(void* target, int colIdx)
		{
			long colSize = GetColumnSize();

			T* source = rawDataPtr + colIdx * colLength;
			Buffer.MemoryCopy(source, target, colSize, colSize);
		}

		public void ExportSingle(void* target, int colIdx, int rowIdx)
		{
			T* source = rawDataPtr + (colIdx * colLength + rowIdx);
			Buffer.MemoryCopy(source, target, GetSingleSize(), GetSingleSize());
		}

		public void Dispose()
		{
			rawDataHandle.Free();
		}
	}
}
