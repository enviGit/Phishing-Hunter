using System.Text.RegularExpressions;

namespace ph.Utils {
    public static class StringExtensions {
        public static string AddSpacesBeforeUppercase(this string str) {
            if (string.IsNullOrWhiteSpace(str))
                return str;

            return Regex.Replace(
                str,
                "(?<!^)([A-Z])",
                " $1"
            );
        }
    }
}
