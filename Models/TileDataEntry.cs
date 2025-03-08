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
		/// Sets the data for this TileDataEntry from a byte array, such as one read from a binary file
		/// </summary>
		/// <param name="data">The raw data to populate this entry with</param>
		/// <exception cref="ArgumentException"></exception>
		public void SetData(byte[] data);

		/// <summary>
		/// Returns the pointer to the backing data of this TileDataEntry
		/// </summary>
		/// <returns></returns>
		public void* GetRawPtr();

		/// <summary>
		/// Gets the raw data of this tile data entry as a byte array.
		/// </summary>
		/// <returns>The raw data as a byte array</returns>
		public byte[] GetData();

		/// <summary>
		/// Returns the raw size of this tile data entry, in bytes.
		/// </summary>
		/// <returns>The total size of this data entry in bytes</returns>
		public int GetRawSize();

		/// <summary>
		/// Copies an entire row of data from this data entry into the world.
		/// </summary>
		/// <param name="target">The pointer to the leftmost tiles ITileData of this type to copy into</param>
		/// <param name="rowIdx">The row of data to place at that location</param>
		public void EmplaceRow(void* target, int rowIdx);

		/// <summary>
		/// Copies a single entry of data into the world.
		/// </summary>
		/// <param name="target">The pointer to the tiles ITileData of this type to copy into</param>
		/// <param name="rowIdx">The Y position to copy from</param>
		/// <param name="columnIdx">The X position to copy from</param>
		public void EmplaceSingle(void* target, int rowIdx, int columnIdx);
	}

	public unsafe class TileDataEntry<T> : ITileDataEntry, IDisposable where T : unmanaged, ITileData
	{
		private readonly T[] rawData;
		private readonly T* rawDataPtr;

		private GCHandle rawDataHandle;

		public int length;
		public int rowLength;

		public TileDataEntry(int length, int rowLength)
		{
			rawData = new T[length];
			rawDataHandle = GCHandle.Alloc(rawData, GCHandleType.Pinned);
			rawDataPtr = (T*)rawDataHandle.AddrOfPinnedObject().ToPointer();

			this.length = length;
			this.rowLength = rowLength;
		}

		public void* GetRawPtr()
		{
			return rawDataPtr;
		}

		public void SetData(byte[] data)
		{
			if (data.Length != sizeof(T) * length)
				throw new ArgumentException($"Data size is inappropriate, Expected enough for {length} {nameof(T)} ({sizeof(T) * length} bytes), but got {data.Length}");

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

		public int GetRawSize()
		{
			return sizeof(T) * length;
		}

		public void EmplaceRow(void* target, int rowIdx)
		{
			long rowSize = sizeof(T) * rowLength;

			T* source = rawDataPtr + rowIdx * rowLength;
			Buffer.MemoryCopy(source, target, rowSize, rowSize);
		}

		public void EmplaceSingle(void* target, int rowIdx, int columnIdx)
		{
			T* source = rawDataPtr + rowIdx * rowLength + columnIdx;
			Buffer.MemoryCopy(source, target, sizeof(T), sizeof(T));
		}

		public void Dispose()
		{
			rawDataHandle.Free();
		}
	}
}
