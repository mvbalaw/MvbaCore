//  * **************************************************************************
//  * Copyright (c) McCreary, Veselka, Bragg & Allen, P.C.
//  * This source code is subject to terms and conditions of the MIT License.
//  * A copy of the license can be found in the License.txt file
//  * at the root of this distribution. 
//  * By using this source code in any fashion, you are agreeing to be bound by 
//  * the terms of the MIT License.
//  * You must not remove this notice from this software.
//  * **************************************************************************

using FluentAssert;

using JetBrains.Annotations;

using MvbaCore.Lucene;

using NUnit.Framework;

namespace MvbaCoreTests.Lucene
{
	[UsedImplicitly]
	public class LuceneSearcherTests
	{
		[TestFixture]
		public class When_asked_to_EscapeLeadingWildcards
		{
			[Test]
			public void Given_initial_prefixed_quoted_multi_word_term_should_get_the_input()
			{
				const string input = "name:\"Bob Jackson\" 456qwe";
				var result = LuceneSearcher.EscapeLeadingWildcards(input);
				result.ShouldBeEqualTo(input);
			}

			[Test]
			public void Given_initial_prefixed_quoted_multi_word_term_with_leading_wildcard_should_get_input_without_the_wildcard()
			{
				const string input = "name:\"*Bob Jackson\" 456qwe";
				var result = LuceneSearcher.EscapeLeadingWildcards(input);
				result.ShouldBeEqualTo(input.Replace("*", ""));
			}

			[Test]
			public void Given_initial_prefixed_quoted_single_word_term_should_get_the_input()
			{
				const string input = "name:\"Jackson\" 456qwe";
				var result = LuceneSearcher.EscapeLeadingWildcards(input);
				result.ShouldBeEqualTo(input);
			}

			[Test]
			public void Given_initial_prefixed_quoted_single_word_term_with_leading_wildcard_should_get_input_without_the_wildcard()
			{
				const string input = "name:\"*Jackson\" 456qwe";
				var result = LuceneSearcher.EscapeLeadingWildcards(input);
				result.ShouldBeEqualTo(input.Replace("*", ""));
			}

			[Test]
			public void Given_initial_prefixed_unquoted_term_should_get_the_input()
			{
				const string input = "name:Jackson 456qwe";
				var result = LuceneSearcher.EscapeLeadingWildcards(input);
				result.ShouldBeEqualTo(input);
			}

			[Test]
			public void Given_initial_prefixed_unquoted_term_with_leading_wildcard_should_get_the_input_with_the_wildcard_escaped()
			{
				const string input = "name:*Jackson 456qwe";
				var result = LuceneSearcher.EscapeLeadingWildcards(input);
				result.ShouldBeEqualTo(input.Replace("*", LuceneConstants.WildcardEndsWithSearchEnabler + "*"));
			}

			[Test]
			public void Given_initial_unprefixed_quoted_multi_word_term_should_get_the_input()
			{
				const string input = "\"Bob Jackson\" 456qwe";
				var result = LuceneSearcher.EscapeLeadingWildcards(input);
				result.ShouldBeEqualTo(input);
			}

			[Test]
			public void Given_initial_unprefixed_quoted_multi_word_term_with_leading_wildcard_should_get_input_without_the_wildcard()
			{
				const string input = "\"*Bob Jackson\" 456qwe";
				var result = LuceneSearcher.EscapeLeadingWildcards(input);
				result.ShouldBeEqualTo(input.Replace("*", ""));
			}

			[Test]
			public void Given_initial_unprefixed_quoted_single_word_term_should_get_the_input()
			{
				const string input = "\"Jackson\" 456qwe";
				var result = LuceneSearcher.EscapeLeadingWildcards(input);
				result.ShouldBeEqualTo(input);
			}

			[Test]
			public void Given_initial_unprefixed_quoted_single_word_term_with_leading_wildcard_should_get_input_without_the_wildcard()
			{
				const string input = "\"*Jackson\" 456qwe";
				var result = LuceneSearcher.EscapeLeadingWildcards(input);
				result.ShouldBeEqualTo(input.Replace("*", ""));
			}

			[Test]
			public void Given_initial_unprefixed_unquoted_term_should_get_the_input()
			{
				const string input = "Jackson 456qwe";
				var result = LuceneSearcher.EscapeLeadingWildcards(input);
				result.ShouldBeEqualTo(input);
			}

			[Test]
			public void Given_initial_unprefixed_unquoted_term_with_leading_wildcard_should_get_the_input_with_the_wildcard_escaped()
			{
				const string input = "*Jackson 456qwe";
				var result = LuceneSearcher.EscapeLeadingWildcards(input);
				result.ShouldBeEqualTo(input.Replace("*", LuceneConstants.WildcardEndsWithSearchEnabler + "*"));
			}

			[Test]
			public void Given_prefixed_quoted_multi_word_term_should_get_the_input()
			{
				const string input = "123abc name:\"Bob Jackson\" 456qwe";
				var result = LuceneSearcher.EscapeLeadingWildcards(input);
				result.ShouldBeEqualTo(input);
			}

			[Test]
			public void Given_prefixed_quoted_multi_word_term_with_leading_wildcard_should_get_input_without_the_wildcard()
			{
				const string input = "123abc name:\"*Bob Jackson\" 456qwe";
				var result = LuceneSearcher.EscapeLeadingWildcards(input);
				result.ShouldBeEqualTo(input.Replace("*", ""));
			}

			[Test]
			public void Given_prefixed_quoted_single_word_term_with_leading_wildcard_should_get_input_without_the_wildcard()
			{
				const string input = "123abc name:\"*Jackson\" 456qwe";
				var result = LuceneSearcher.EscapeLeadingWildcards(input);
				result.ShouldBeEqualTo(input.Replace("*", ""));
			}

			[Test]
			public void Given_prefixed_unquoted_term_should_get_the_input()
			{
				const string input = "123abc name:Jackson 456qwe";
				var result = LuceneSearcher.EscapeLeadingWildcards(input);
				result.ShouldBeEqualTo(input);
			}

			[Test]
			public void Given_prefixed_unquoted_term_with_leading_wildcard_should_get_the_input_with_the_wildcard_escaped()
			{
				const string input = "123abc name:*Jackson 456qwe";
				var result = LuceneSearcher.EscapeLeadingWildcards(input);
				result.ShouldBeEqualTo(input.Replace("*", LuceneConstants.WildcardEndsWithSearchEnabler + "*"));
			}

			[Test]
			public void Given_unprefixed_quoted_multi_word_term_should_get_the_input()
			{
				const string input = "123abc \"Bob Jackson\" 456qwe";
				var result = LuceneSearcher.EscapeLeadingWildcards(input);
				result.ShouldBeEqualTo(input);
			}

			[Test]
			public void Given_unprefixed_quoted_multi_word_term_with_leading_wildcard_should_get_input_without_the_wildcard()
			{
				const string input = "123abc \"*Bob Jackson\" 456qwe";
				var result = LuceneSearcher.EscapeLeadingWildcards(input);
				result.ShouldBeEqualTo(input.Replace("*", ""));
			}

			[Test]
			public void Given_unprefixed_quoted_single_word_term_should_get_the_input()
			{
				const string input = "123abc \"Jackson\" 456qwe";
				var result = LuceneSearcher.EscapeLeadingWildcards(input);
				result.ShouldBeEqualTo(input);
			}

			[Test]
			public void Given_unprefixed_quoted_single_word_term_with_leading_wildcard_should_get_input_without_the_wildcard()
			{
				const string input = "123abc \"*Jackson\" 456qwe";
				var result = LuceneSearcher.EscapeLeadingWildcards(input);
				result.ShouldBeEqualTo(input.Replace("*", ""));
			}

			[Test]
			public void Given_unprefixed_unquoted_term_should_get_the_input()
			{
				const string input = "123abc Jackson 456qwe";
				var result = LuceneSearcher.EscapeLeadingWildcards(input);
				result.ShouldBeEqualTo(input);
			}

			[Test]
			public void Given_unprefixed_unquoted_term_with_leading_wildcard_should_get_the_input_with_the_wildcard_escaped()
			{
				const string input = "123abc *Jackson 456qwe";
				var result = LuceneSearcher.EscapeLeadingWildcards(input);
				result.ShouldBeEqualTo(input.Replace("*", LuceneConstants.WildcardEndsWithSearchEnabler + "*"));
			}
		}
	}
}