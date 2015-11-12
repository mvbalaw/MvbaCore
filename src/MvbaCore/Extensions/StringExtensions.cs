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

		[NotNull]
		[Pure]
		public static string AddSpacesToSentence([CanBeNull] this string text)
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

		[NotNull]
		[Pure]
		public static string GetFirstTwoWordsInSentence([CanBeNull] this string text)
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

		[NotNull]
		[Pure]
		public static string GetMD5Hash([NotNull] this string input)
		{
			var md5 = new MD5CryptoServiceProvider();
			var bytes = Encoding.ASCII.GetBytes(input);
			var hash = md5.ComputeHash(bytes)
			              .Select(x => x.ToString("x2").ToLower());
			return hash.Join("");
		}

		[NotNull]
		[Pure]
		[ContractAnnotation("separator:null => halt")]
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

		[NotNull]
		[Pure]
		public static byte[] HexToBytes([NotNull] this string hexEncodedBytes, int start, int end)
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

		[Pure]
		[ContractAnnotation("input:null => true")]
		public static bool IsNullOrEmpty([CanBeNull] this string input)
		{
			return input.IsNullOrEmpty(false);
		}

		[Pure]
		[ContractAnnotation("input:null => true")]
		public static bool IsNullOrEmpty([CanBeNull] this string input, bool trim)
		{
			if (String.IsNullOrEmpty(input))
			{
				return true;
			}
			return trim && String.IsNullOrEmpty(input.Trim());
		}

		[CanBeNull, Pure]
		[ContractAnnotation("input:null => null; input:notnull => notnull")]
		public static string NewlinesToBr([CanBeNull] this string input)
		{
			if (input == null)
			{
				return null;
			}
			return input.Replace(Environment.NewLine, "<br />");
		}

		[CanBeNull]
		[Pure]
		public static string PrefixWithAOrAn([NotNull] this string str)
		{
			if (str.IsNullOrEmpty())
			{
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
					                         "utt"
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

		[CanBeNull]
		[Pure]
		[ContractAnnotation("input:null => null; input:notnull => notnull")]
		public static string ReplaceIfExists([CanBeNull] this string input, string oldValue, string newValue)
		{
			if (input == null)
			{
				return null;
			}
			return !oldValue.IsNullOrEmpty() ? input.Replace(oldValue, newValue) : input;
		}

		[Pure]
		[ContractAnnotation("input:null => null")]
		public static int? SafeParseInt32([CanBeNull] this string input)
		{
			int value;
			return !Int32.TryParse(input, out value) ? (int?)null : value;
		}

		[CanBeNull]
		[Pure]
		[ContractAnnotation("input:null => null; input:notnull => notnull")]
		public static string TabsToNbsp([CanBeNull] this string input)
		{
			if (input == null)
			{
				return null;
			}
			return input.Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;");
		}

		[Pure]
		[CanBeNull]
		[ContractAnnotation("str:null => null; str:notnull => notnull")]
		public static string ToCamelCase([CanBeNull] this string str)
		{
			if (String.IsNullOrEmpty(str))
			{
				return str;
			}
			str = Char.ToLower(str[0]) + str.Substring(1);
			return str;
		}

		[NotNull]
		[Pure]
		public static string ToNonNull([CanBeNull] this string input)
		{
			return input ?? "";
		}

		[NotNull]
		[ContractAnnotation("str:null => halt; str:notnull => notnull")]
		[Pure]
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

		[Pure]
		[NotNull]
		[ContractAnnotation("value:null => halt; value:notnull => notnull")]
		public static string ToTitleCase([NotNull] this string value, bool lowerCaseTheRemainder = true)
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

		[NotNull]
		[Pure]
		[ContractAnnotation("value:null => halt;")]
		public static string TrimIfLongerThan([NotNull] this string value, int maxLength)
		{
			return value.Length > maxLength ? value.Substring(0, maxLength) : value;
		}
	}
}