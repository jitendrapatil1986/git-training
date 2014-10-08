namespace Warranty.UI.Core.Helpers
{
    using System.Web.Mvc;

    public static class ModelBindingHelper
    {
        public static string ReplaceSpecialChactersWithDotReplacement(string value)
        {
            var specialCharacters = new[] { ".", "[", "]" };
            foreach (var character in specialCharacters)
            {
                value = value.Replace(character, HtmlHelper.IdAttributeDotReplacement);
            }
            return value;
        }
    }
}