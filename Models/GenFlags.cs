using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StructureHelper.Models
{
	/// <summary>
	/// Flags that allow users to define special behavior for structure generation.
	/// </summary>
	[Flags]
	public enum GenFlags
	{
		/// <summary>
		/// No special generation
		/// </summary>
		None = 0b0,
		/// <summary>
		/// Null tiles will inherit the type of the tile behind them, but keep their slope if that tile is slopable
		/// </summary>
		NullsKeepGivenSlope = 0b1,
		/// <summary>
		/// Null tiles and walls will inherit the type of the tile/wall behind them, but will keep the paint they are given
		/// </summary>
		NullsKeepGivenPaint = 0b10,
		/// <summary>
		/// Tile entities will not have their saved data placed in the generated structures, instead falling back to acting as if they are newly created
		/// </summary>
		IgnoreTileEnttiyData = 0b100,
	}
}
