using StructureHelper.ChestHelper;
using StructureHelper.Helpers;
using StructureHelper.Models;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace StructureHelper
{
	/// <summary>
	/// The legacy API left as is to prevent runtime breakage.
	/// </summary>
	public static class Generator
	{
		/// <summary>
		/// This method generates a structure from a structure file within your mod.
		/// </summary>
		/// <param name="path">The path to your structure file within your mod - this should not include your mod's folder, only the path beyond it.</param>
		/// <param name="pos">The position in the world in which you want your structure to generate, in tile coordinates.</param>
		/// <param name="mod">The instance of your mod to grab the file from.</param>
		/// <param name="fullPath">Indicates if you want to use a fully qualified path to get the structure file instead of one from your mod - generally should only be used for debugging.</param>
		/// <param name="ignoreNull">If the structure should repsect the normal behavior of null tiles or not. This should never be true if you're using the mod as a dll reference.</param>
		/// <param name="flags">Allows you to pass flags for special generation behavior. See <see cref="GenFlags"/> </param>
		/// <returns>If the structure generated successfully or not</returns>
		[Obsolete("Legacy generation API is deprecated. Please port your structures to the 3.0 format and use StructureHelper.API.Generator." +
			"\n Porting instructions can be found at https://github.com/ScalarVector1/StructureHelper/wiki/3.0-Porting-Guide" +
			"\n If you truly need legacy API access, use StructureHelper.API.Legacy.LegacyGenerator.", true)]
		public static bool GenerateStructure(string path, Point16 pos, Mod mod, bool fullPath = false, bool ignoreNull = false, GenFlags flags = GenFlags.None)
		{
			return API.Legacy.LegacyGenerator.GenerateStructure(path, pos, mod, fullPath, ignoreNull, flags);
		}

		/// <summary>
		/// This method generates a structure selected randomly from a multistructure file within your mod.
		/// </summary>
		/// <param name="path">The path to your multistructure file within your mod - this should not include your mod's folder, only the path beyond it.</param>
		/// <param name="pos">The position in the world in which you want your structure to generate, in tile coordinates.</param>
		/// <param name="mod">The instance of your mod to grab the file from.</param>
		/// <param name="fullPath">Indicates if you want to use a fully qualified path to get the structure file instead of one from your mod - generally should only be used for debugging.</param>
		/// <param name="ignoreNull">If the structure should repsect the normal behavior of null tiles or not. This should never be true if you're using the mod as a dll refference.</param>
		/// <param name="flags">Allows you to pass flags for special generation behavior. See <see cref="GenFlags"/> </param>
		/// <returns>If the structure generated successfully or not</returns>
		[Obsolete("Legacy generation API is deprecated. Please port your structures to the 3.0 format and use StructureHelper.API.Generator." +
			"\n Porting instructions can be found at https://github.com/ScalarVector1/StructureHelper/wiki/3.0-Porting-Guide" +
			"\n If you truly need legacy API access, use StructureHelper.API.Legacy.LegacyGenerator.", true)]
		public static bool GenerateMultistructureRandom(string path, Point16 pos, Mod mod, bool fullPath = false, bool ignoreNull = false, GenFlags flags = GenFlags.None)
		{
			return API.Legacy.LegacyGenerator.GenerateMultistructureRandom(path, pos, mod, fullPath, ignoreNull, flags);
		}

		/// <summary>
		/// This method generates a structure you select from a multistructure file within your mod. Useful if you want to do your own weighted randomization or want additional logic based on dimensions gotten from GetMultistructureDimensions.
		/// </summary>
		/// <param name="path">The path to your multistructure file within your mod - this should not include your mod's folder, only the path beyond it.</param>
		/// <param name="pos">The position in the world in which you want your structure to generate, in tile coordinates.</param>
		/// <param name="mod">The instance of your mod to grab the file from.</param>
		/// <param name="index">The index of the structure you want to generate out of the multistructure file, structure indicies are 0-based and match the order they were saved in.</param>
		/// <param name="fullPath">Indicates if you want to use a fully qualified path to get the structure file instead of one from your mod - generally should only be used for debugging.</param>
		/// <param name="ignoreNull">If the structure should repsect the normal behavior of null tiles or not. This should never be true if you're using the mod as a dll refference.</param>
		/// <param name="flags">Allows you to pass flags for special generation behavior. See <see cref="GenFlags"/> </param>
		/// <returns>If the structure generated successfully or not</returns>
		[Obsolete("Legacy generation API is deprecated. Please port your structures to the 3.0 format and use StructureHelper.API.Generator." +
			"\n Porting instructions can be found at https://github.com/ScalarVector1/StructureHelper/wiki/3.0-Porting-Guide" +
			"\n If you truly need legacy API access, use StructureHelper.API.Legacy.LegacyGenerator.", true)]
		public static bool GenerateMultistructureSpecific(string path, Point16 pos, Mod mod, int index, bool fullPath = false, bool ignoreNull = false, GenFlags flags = GenFlags.None)
		{
			return API.Legacy.LegacyGenerator.GenerateMultistructureSpecific(path, pos, mod, index, fullPath, ignoreNull, flags);
		}

		/// <summary>
		/// Gets the dimensions of a structure from a structure file within your mod.
		/// </summary>
		/// <param name="path">The path to your structure file within your mod - this should not include your mod's folder, only the path beyond it.</param>
		/// <param name="mod">The instance of your mod to grab the file from.</param>
		/// <param name="dims">The Point16 variable which you want to be set to the dimensions of the structure.</param>
		/// <param name="fullPath">Indicates if you want to use a fully qualified path to get the structure file instead of one from your mod - generally should only be used for debugging.</param>
		/// <returns></returns>
		[Obsolete("Legacy generation API is deprecated. Please port your structures to the 3.0 format and use StructureHelper.API.Generator." +
			"\n Porting instructions can be found at https://github.com/ScalarVector1/StructureHelper/wiki/3.0-Porting-Guide" +
			"\n If you truly need legacy API access, use StructureHelper.API.Legacy.LegacyGenerator.", true)]
		public static bool GetDimensions(string path, Mod mod, ref Point16 dims, bool fullPath = false)
		{
			return API.Legacy.LegacyGenerator.GetDimensions(path, mod, ref dims, fullPath);
		}

		/// <summary>
		/// Gets the dimensions of a structure from a structure file within your mod.
		/// </summary>
		/// <param name="path">The path to your structure file within your mod - this should not include your mod's folder, only the path beyond it.</param>
		/// <param name="mod">The instance of your mod to grab the file from.</param>
		/// <param name="index">The index of the structure you want to get the dimensions of out of the multistructure file, structure indicies are 0-based and match the order they were saved in.</param>
		/// <param name="dims">The Point16 variable which you want to be set to the dimensions of the structure.</param>
		/// <param name="fullPath">Indicates if you want to use a fully qualified path to get the structure file instead of one from your mod - generally should only be used for debugging.</param>
		/// <returns></returns>
		[Obsolete("Legacy generation API is deprecated. Please port your structures to the 3.0 format and use StructureHelper.API.Generator." +
			"\n Porting instructions can be found at https://github.com/ScalarVector1/StructureHelper/wiki/3.0-Porting-Guide" +
			"\n If you truly need legacy API access, use StructureHelper.API.Legacy.LegacyGenerator.", true)]
		public static bool GetMultistructureDimensions(string path, Mod mod, int index, ref Point16 dims, bool fullPath = false)
		{
			return API.Legacy.LegacyGenerator.GetMultistructureDimensions(path, mod, index, ref dims, fullPath);
		}

		/// <summary>
		/// Checks if a structure file is a multistructure or not. Can be used to easily add support for parameterizing strucutres or multistructures in your mod.
		/// </summary>
		/// <param name="path">The path to the structure file you wish to check.</param>
		/// <param name="mod">The instance of your mod to grab the file from.</param>
		/// <returns>True if the file is a multistructure, False if the file is a structure, null if it is invalid.</returns>
		[Obsolete("Legacy generation API is deprecated. Please port your structures to the 3.0 format and use StructureHelper.API.Generator." +
			"\n Porting instructions can be found at https://github.com/ScalarVector1/StructureHelper/wiki/3.0-Porting-Guide" +
			"\n If you truly need legacy API access, use StructureHelper.API.Legacy.LegacyGenerator.", true)]
		public static bool? IsMultistructure(string path, Mod mod)
		{
			return API.Legacy.LegacyGenerator.IsMultistructure(path, mod);
		}

		/// <summary>
		/// Gets the quantity of structures in a multistructure file if possible. Returns null if the structure is invalid or not a multistructure.
		/// </summary>
		/// <param name="path">The path to the structure file you wish to check.</param>
		/// <param name="mod">The instance of your mod to grab the file from.</param>
		/// <returns>The amount of structures in a multistructure, or null if invalid.</returns>
		[Obsolete("Legacy generation API is deprecated. Please port your structures to the 3.0 format and use StructureHelper.API.Generator." +
			"\n Porting instructions can be found at https://github.com/ScalarVector1/StructureHelper/wiki/3.0-Porting-Guide" +
			"\n If you truly need legacy API access, use StructureHelper.API.Legacy.LegacyGenerator.", true)]
		public static int? GetStructureCount(string path, Mod mod)
		{
			return API.Legacy.LegacyGenerator.GetStructureCount(path, mod);
		}

		/// <summary>
		/// Parses and generates the actual tiles from a structure file
		/// </summary>
		/// <param name="tag">The structure data TagCompound to generate from</param>
		/// <param name="pos">The position in the world of the top-leftmost tile to be placed at</param>
		/// <param name="ignoreNull">If this structure should place null tiles or not</param>
		/// <returns>If the structure successfully generated or not</returns>
		[Obsolete("Legacy generation API is deprecated. Please port your structures to the 3.0 format and use StructureHelper.API.Generator." +
			"\n Porting instructions can be found at https://github.com/ScalarVector1/StructureHelper/wiki/3.0-Porting-Guide" +
			"\n If you truly need legacy API access, use StructureHelper.API.Legacy.LegacyGenerator.", true)]
		public static unsafe bool Generate(TagCompound tag, Point16 pos, bool ignoreNull = false, GenFlags flags = GenFlags.None)
		{
			return API.Legacy.LegacyGenerator.Generate(tag, pos, ignoreNull, flags);
		}
	}
}