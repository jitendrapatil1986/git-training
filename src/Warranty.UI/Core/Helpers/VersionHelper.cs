namespace Warranty.UI.Core.Helpers
{
	public static class VersionHelper
	{
		public static string ApplicationVersionNumber()
		{
			return System.Reflection.Assembly.GetAssembly(typeof(VersionHelper)).GetName().Version.ToString();
		}
	}
}