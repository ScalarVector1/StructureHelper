namespace StructureHelper.Helpers
{
	internal class ErrorHelper
	{
		public static string GenerateErrorMessage(string message, Mod mod)
		{
			if (mod != null)
				return $"{mod.DisplayName}({mod.Name}) <-- has caused an issue with Structure helper! \n\n{message}\n\nIf you are a player, please report this to the developers of {mod.DisplayName}, NOT StructureHelper!";
			else
				return $"An unknown mod has caused an issue with Structure helper! \n\n{message}\n\nIf you are a developer, did you pass null to a parameter requiring a mod? Otherwise this error may have occured in a place without mod context.";
		}
	}
}