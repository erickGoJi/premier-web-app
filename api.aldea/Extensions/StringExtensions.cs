using System;
using System.Text;

namespace api.premier.Extensions
{
    public static class StringExtensions
    {
        public static string UpperCaseFirstLetter(this string str) => str switch
            {
                null => throw new ArgumentNullException(nameof(str)),
                "" => throw new ArgumentException($"{nameof(str)} cannot be empty", nameof(str)),
                _ => string.Concat(str[0].ToString().ToUpper(), str.AsSpan(1))
            };

        public static string CapitalizeFirst(this string s)
        {
            bool IsNewSentense = true;
            var result = new StringBuilder(s.Length);
            for (int i = 0; i < s.Length; i++)
            {
                if (IsNewSentense && char.IsLetter(s[i]))
                {
                    result.Append(char.ToUpper(s[i]));
                    IsNewSentense = false;
                }
                else
                    result.Append(char.ToLower(s[i]));

                if (s[i] == '!' || s[i] == '?' || s[i] == '.')
                {
                    IsNewSentense = true;
                }
            }
            
            return result.ToString();
        }
    }
}