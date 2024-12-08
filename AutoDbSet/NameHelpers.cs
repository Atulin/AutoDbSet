using System.Linq;

namespace AutoDbSet;

public static class NameHelpers
{
	public static string Pluralize(this string str)
	{
		if (str.Length <= 2) return str + "s";
		if (EsSuffixes.Any(str.EndsWith)) return str + "es";
		if (str.EndsWith("s")) return str;

		
		var last = str[str.Length - 1];
		var secondLast = str[str.Length - 2];
		
		if (!secondLast.IsVowel() && last == 'y') return str.Substring(0, str.Length - 1) + "ies";
		if (secondLast.IsVowel() && last == 'y') return str + "s";
		
		if (!secondLast.IsVowel() && last == 'o') return str + "es";
		if (secondLast.IsVowel() && last == 'o') return str + "s";

		if (last == 'e' && secondLast == 'f') return str.Substring(0, str.Length - 2) + "ves";
		if (last == 'f' && secondLast == 'f') return str + 's';
		if (last == 'f') return str.Substring(0, str.Length - 1) + "ves";

		return str + "s";
	}
	
	private static readonly char[] Vowels = ['a', 'e', 'i', 'o', 'u'];

	private static readonly string[] EsSuffixes = ["ss", "x", "ch", "sh"];

	private static bool IsVowel(this char c) => Vowels.Contains(c);
}