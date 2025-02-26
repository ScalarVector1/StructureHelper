using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StructureHelper.Helpers
{
	internal class ErrorHelper
	{
		public static string GenerateErrorMessage(string message, Mod mod)
		{
			return $"{mod.DisplayName}({mod.Name}) <-- has caused an issue with Structure helper! \n\n{message}\n\nIf you are a player, please report this to the developers of {mod.DisplayName}, NOT StructureHelper!";
		}
	}
}
