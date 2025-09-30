using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StructureHelper.Core
{
	public static class WandSavingSettings
	{
		internal static List<Type> activeCustomDataTypes;
		internal static Dictionary<string, Type> possibleCustomDataTypes;

		/// <summary>
		/// Registers a custom ITileData type to be able to be enabled for structure saving.
		/// After you do this to save your custom data you will need to enable it in-game using
		/// the data configurator tool.
		/// </summary>
		/// <param name="type">The type to register. This should extend ITileData</param>
		/// <param name="mod">An instance of your mod</param>
		/// <exception cref="ArgumentException"></exception>
		public static void RegisterCustomTypeForSaving(Type type, Mod mod)
		{
			if (type.IsAssignableFrom(typeof(ITileData)))
				possibleCustomDataTypes.Add($"{mod.Name}/{type.Name}", type);
			else
				throw new ArgumentException("You can only register ITileData types as custom data for saving!");
		}

		internal static void ActivateTypeForSaving(string key)
		{
			if (possibleCustomDataTypes.ContainsKey(key))
				activeCustomDataTypes.Add(possibleCustomDataTypes[key]);
		}

		internal static void DeactivateTypeForSaving(string key)
		{
			if (possibleCustomDataTypes.ContainsKey(key) && activeCustomDataTypes.Contains(possibleCustomDataTypes[key]))
				activeCustomDataTypes.Add(possibleCustomDataTypes[key]);
		}
	}
}
