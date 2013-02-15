//   * **************************************************************************
//   * Copyright (c) McCreary, Veselka, Bragg & Allen, P.C.
//   * This source code is subject to terms and conditions of the MIT License.
//   * A copy of the license can be found in the License.txt file
//   * at the root of this distribution.
//   * By using this source code in any fashion, you are agreeing to be bound by
//   * the terms of the MIT License.
//   * You must not remove this notice from this software.
//   * **************************************************************************

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

using JetBrains.Annotations;

// ReSharper disable CheckNamespace

namespace System
// ReSharper restore CheckNamespace
{
	public static class StringExtensions
	{
		public static void AddIfNotNullOrEmpty(this List<string> list, string item)
		{
			if (!item.IsNullOrEmpty())
			{
				list.Add(item);
			}
		}

		public static string AddSpacesToSentence(this string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return "";
			}
			// remove any spaces already in text
			text = text.Replace(" ", "");
			var newText = new StringBuilder(text.Length * 2);
			newText.Append(text[0]);
			for (var i = 1; i < text.Length; i++)
			{
				if (char.IsUpper(text[i]))
				{
					newText.Append(' ');
				}
				newText.Append(text[i]);
			}
			return newText.ToString();
		}

		public static string GetFirstTwoWordsInSentence(this string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return "";
			}
			// remove any spaces already in text
			text = text.Replace(" ", "");
			var newText = new StringBuilder(text.Length * 2);
			var wordCount = 0;
			newText.Append(text[0]);
			for (var i = 1; i < text.Length; i++)
			{
				if (char.IsUpper(text[i]))
				{
					newText.Append(' ');
					wordCount++;
					if (wordCount == 2)
					{
						return newText.ToString();
					}
				}
				newText.Append(text[i]);
			}
			return newText.ToString();
		}

		public static string GetMD5Hash(this string input)
		{
			var md5 = new MD5CryptoServiceProvider();
			var bytes = Encoding.ASCII.GetBytes(input);
			var hash = md5.ComputeHash(bytes)
			              .Select(x => x.ToString("x2").ToLower());
			return hash.Join("");
		}

		public static IList<IList<string>> GroupBy([NotNull] this string[] lines, string separator)
		{
			if (separator == null)
			{
				throw new ArgumentException("Line grouping separator cannot be null", "separator");
			}
			var groups = new List<IList<string>>();
			if (lines.Length == 0)
			{
				return groups;
			}

			var group = new List<string>();
			foreach (var line in lines)
			{
				if (line == separator)
				{
					groups.Add(group);
					group = new List<string>();
					continue;
				}
				if (line.TrimEnd().Length == 0)
				{
					continue;
				}
				group.Add(line);
			}
			if (group.Any())
			{
				groups.Add(group);
			}

			return groups;
		}

		public static byte[] HexToBytes(this string hexEncodedBytes, int start, int end)
		{
			var length = end - start;
			const string tagName = "hex";
			var fakeXmlDocument = String.Format("<{1}>{0}</{1}>",
			                                    hexEncodedBytes.Substring(start, length),
			                                    tagName);
			var hexLength = length / 2;
			var result = new byte[hexLength];
			using (var stream = new MemoryStream(Encoding.ASCII.GetBytes(fakeXmlDocument)))
			{
				var reader = XmlReader.Create(stream, new XmlReaderSettings());
				reader.ReadStartElement(tagName);
				reader.ReadContentAsBinHex(result, 0, hexLength);
			}
			return result;
		}

		public static bool IsNullOrEmpty(this string input)
		{
			return input.IsNullOrEmpty(false);
		}

		public static bool IsNullOrEmpty(this string input, bool trim)
		{
			if (String.IsNullOrEmpty(input))
			{
				return true;
			}
			return trim && String.IsNullOrEmpty(input.Trim());
		}

		public static string NewlinesToBr([CanBeNull] this string input)
		{
			if (input == null)
			{
				return null;
			}
			return input.Replace(Environment.NewLine, "<br />");
		}

		public static string PrefixWithAOrAn([NotNull] this string str)
		{
			if (str.IsNullOrEmpty())
			{
				//throw new ArgumentNullException("str", "cannot pluralize a null or empty string");
				return str;
			}

			// note: this is really tough, the rule is "an" if it starts with
			// a vowel SOUND, and "a" if it starts witha consonant SOUND

			// a box
			// a cat
			// a university (starts with same sound as 'you', so consonant sound)
			// a unicorn (starts with same sound as 'you', so consonant sound)
			// a hotel (h is not silent, so consonant sound)

			// an atom
			// an entrance
			// an ice cream
			// an uncle (starts with the same sound as 'other', so vowel sound)
			// an hour (h is silent)

			// acronyms depend on the sound of the first letter
			// a UFO (u => 'you' so consonant)
			// an MBA (m => 'em' so vowel)

			var shouldPrefixWithA = new HashSet<string>
				                        {
					                        "ha",
					                        "hot",
					                        "uni",
					                        "ut"
				                        };

			var shouldPrefixWithAn = new HashSet<string>
				                         {
					                         "utt",
				                         };

			var lower = str.ToLower();
			if ("aeiouh".IndexOf(lower[0]) != -1)
			{
				if (!shouldPrefixWithA.Any(lower.StartsWith) ||
				    shouldPrefixWithAn.Any(lower.StartsWith))
				{
					return "an " + str;
				}
			}
			return "a " + str;
		}

		public static string ReplaceIfExists([CanBeNull] this string input, string oldValue, string newValue)
		{
			if (input == null)
			{
				return null;
			}
			return !oldValue.IsNullOrEmpty() ? input.Replace(oldValue, newValue) : input;
		}

		public static int? SafeParseInt32(this string input)
		{
			int value;
			return !Int32.TryParse(input, out value) ? (int?)null : value;
		}

		public static string TabsToNbsp([CanBeNull] this string input)
		{
			if (input == null)
			{
				return null;
			}
			return input.Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;");
		}

		public static string ToCamelCase([CanBeNull] this string str)
		{
			if (String.IsNullOrEmpty(str))
			{
				return str;
			}
			str = Char.ToLower(str[0]) + str.Substring(1);
			return str;
		}

		public static string ToNonNull(this string input)
		{
			return input ?? "";
		}

		public static string ToPlural([NotNull] this string str)
		{
			if (str.IsNullOrEmpty())
			{
				throw new ArgumentNullException("str", "cannot pluralize a null or empty string");
			}
			var lower = str.ToLower();
			var last = lower[str.Length - 1];

			if (last == 'y' && str.Length > 1)
			{
				str = str.Substring(0, str.Length - 1);
				last = lower[str.Length - 1];
				if (last.IsVowel())
				{
					return str + "ys";
				}
				return str + "ies";
			}

			if (last == 'x' ||
			    last == 's' ||
			    last == 'h' && (lower.EndsWith("sh") || lower.EndsWith("ch")))
			{
				return str + "es";
			}

			return str + "s";
		}

		public static string ToTitleCase(this string value, bool lowerCaseTheRemainder = true)
		{
			if (value.Length == 0)
			{
				return value;
			}
			return value.Length == 1
				       ? value.ToUpper()
				       : String.Concat(value.Substring(0, 1).ToUpper(),
				                       lowerCaseTheRemainder
					                       ? value.Substring(1).ToLower()
					                       : value.Substring(1));
		}

		[Conditional("TRACE")]
		public static void Trace(this string message)
		{
			Diagnostics.Trace.WriteLine(DateTime.Now + " " + message);
		}

		[Conditional("TRACE")]
		public static void Trace(this string message, string additionalInfo)
		{
			Diagnostics.Trace.WriteLine(DateTime.Now + " " + message + " " + additionalInfo);
		}

		public static string TrimIfLongerThan(this string value, int maxLength)
		{
			return value.Length > maxLength ? value.Substring(0, maxLength) : value;
		}
	}
}