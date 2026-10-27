//  * **************************************************************************
//  * Copyright (c) McCreary, Veselka, Bragg & Allen, P.C.
//  * This source code is subject to terms and conditions of the MIT License.
//  * A copy of the license can be found in the License.txt file
//  * at the root of this distribution.
//  * By using this source code in any fashion, you are agreeing to be bound by
//  * the terms of the MIT License.
//  * You must not remove this notice from this software.
//  * **************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using FluentAssert;

using JetBrains.Annotations;

using MvbaCore.Extensions;

using NUnit.Framework;

// ReSharper disable UnusedMember.Local
// ReSharper disable AccessToStaticMemberViaDerivedType
// ReSharper disable ObjectCreationAsStatement
namespace MvbaCore.Tests
{
	[UsedImplicitly]
	public class NamedConstantTests
	{
		private const int GenerousTimeoutMilliseconds = 10000;
		private const int SecondCallGraceMilliseconds = 500;

		public class DuplicateValue : NamedConstant<DuplicateValue>
		{
			public static readonly DuplicateValue Item = new DuplicateValue("k", "Desc");

			private DuplicateValue(string key, string desc)
			{
				Add(key, this);
				Add(desc, this);
			}
		}

		[TestFixture]
		public class When_a_subclass_looks_up_a_key_with_Get
		{
			[Test]
			public void Given_another_thread_is_still_adding_the_values__should_wait_and_return_the_value_for_a_key_not_added_yet()
			{
				var outcome = RaceTheTypeInitializer(
					Gates.GetRace,
					() => GetRace.GetFor("Last"),
					() => GetRace.Lookup.By("Last"));

				ShouldHaveRunBothCallsToCompletion(outcome);
				outcome.SecondResult.ShouldNotBeNull("Get(\"Last\") returned null on the thread that arrived while the type initializer was still adding values");
				outcome.SecondResult.Key.ShouldBeEqualTo("Last");
				outcome.SecondCallReturnedWhileInitializerWasPaused.ShouldBeFalse("Get returned before the type initializer had finished");
				outcome.SecondResult.ShouldBeSameInstanceAs(outcome.FirstResult);
			}

			[Test]
			public void Given_it_is_the_first_use_of_the_type__should_run_the_type_initializer_and_find_the_value()
			{
				ConstructionCounts.FirstUseThroughGet.ShouldBeEqualTo(0, "setup guard: FirstUseThroughGet's type initializer has already run, so this test cannot show that Get runs it");

				var found = FirstUseThroughGet.Lookup.By("TWO");

				found.ShouldNotBeNull("Get returned null because nothing ran the type initializer that adds the values");
				found.Key.ShouldBeEqualTo("Two");
				ConstructionCounts.FirstUseThroughGet.ShouldBeEqualTo(2, "Get should have run the whole type initializer, once");
			}
		}

		[TestFixture]
		public class When_asked_to_Add
		{
			[TestCase("Red", "NamedConstant storage key 'red' is assigned to Red - cannot assign it to Red")]
			[TestCase("RED", "NamedConstant storage key 'red' is assigned to Red - cannot assign it to RED")]
			[TestCase("rEd", "NamedConstant storage key 'red' is assigned to Red - cannot assign it to rEd")]
			public void Given_a_different_value_under_a_key_that_is_already_taken__should_refuse_it_and_keep_the_original(string key, string expectedMessage)
			{
				// reading Red runs the type initializer, which adds Red and Blue
				var red = Clash.Red;

				var exception = ExceptionThrownBy(() => new Clash(key));

				exception.ShouldNotBeNull("Add accepted a second value under '" + key + "'");
				exception.GetType().ShouldBeEqualTo(typeof(ArgumentException));
				exception.Message.ShouldBeEqualTo(expectedMessage);
				Clash.GetFor("red").ShouldBeSameInstanceAs(red);
				red.Key.ShouldBeEqualTo("Red");
				Clash.GetAll().Count().ShouldBeEqualTo(2);
			}

			[Test]
			public void Given_the_same_value_again_under_its_key_in_another_letter_case__should_ignore_the_repeat()
			{
				// each Direction adds itself as "North", "NORTH" and "north"; the type initializer must not throw
				var all = Direction.GetAll().ToList();

				all.Count.ShouldBeEqualTo(2);
				all.ShouldContainAll(new[] { Direction.North, Direction.South });
				Direction.GetFor("NORTH").ShouldBeSameInstanceAs(Direction.North);
				Direction.GetFor("south").ShouldBeSameInstanceAs(Direction.South);
			}
		}

		[TestFixture]
		public class When_asked_to_GetAll
		{
			[Test]
			public void Given_a_named_constant_where_multiple_keys_are_associated_with_the_same_value_should_return_only_one_instance_of_each_value()
			{
				DuplicateValue.GetAll().Count().ShouldBeEqualTo(1);
			}

			[Test]
			public void Given_a_value_was_added_under_several_keys__should_return_it_once_and_GetFor_should_find_it_by_each_key()
			{
				var all = Currency.GetAll().ToList();

				all.Count.ShouldBeEqualTo(2);
				all.Count(x => ReferenceEquals(x, Currency.Dollar)).ShouldBeEqualTo(1);
				all.Count(x => ReferenceEquals(x, Currency.Euro)).ShouldBeEqualTo(1);
				Currency.GetFor("dollar").ShouldBeSameInstanceAs(Currency.Dollar);
				Currency.GetFor("USD").ShouldBeSameInstanceAs(Currency.Dollar);
				Currency.GetFor("eur").ShouldBeSameInstanceAs(Currency.Euro);
				// Add sets Key on every call, so a value added under several keys reports the last of them
				Currency.Dollar.Key.ShouldBeEqualTo("USD");
			}

			[Test]
			public void Given_another_thread_is_still_adding_the_values__should_wait_and_return_every_value()
			{
				// the keys are listed inside each thread: GetAll is lazy, so listing them here, after the release,
				// would see every value even if the second call had returned early
				var outcome = RaceTheTypeInitializer(
					Gates.GetAllRace,
					() => KeysOf(GetAllRace.GetAll()),
					() => KeysOf(GetAllRace.GetAll()));

				ShouldHaveRunBothCallsToCompletion(outcome);
				outcome.SecondResult.ShouldBeEqualTo("First,Last,Second", "GetAll returned a partial list on the thread that arrived while the type initializer was still adding values");
				outcome.SecondCallReturnedWhileInitializerWasPaused.ShouldBeFalse("GetAll returned before the type initializer had finished");
				outcome.FirstResult.ShouldBeEqualTo("First,Last,Second");
			}

			[Test]
			public void Given_each_value_was_added_under_one_key__should_return_every_value()
			{
				var all = Fruit.GetAll().ToList();

				all.Count.ShouldBeEqualTo(3);
				all.ShouldContainAll(new[] { Fruit.Apple, Fruit.Banana, Fruit.CherryPie });
			}
		}

		[TestFixture]
		public class When_asked_to_GetDefault
		{
			[Test]
			public void Given_a_field_marked_DefaultKey__should_return_that_fields_value()
			{
				var result = Size.GetDefault();

				result.ShouldBeSameInstanceAs(Size.Medium);
				result.Key.ShouldBeEqualTo("Medium");
			}

			[Test]
			public void Given_another_thread_is_still_adding_the_values__should_wait_and_return_the_default()
			{
				// the default is the value whose constructor the first thread is paused in
				var outcome = RaceTheTypeInitializer(
					Gates.GetDefaultRace,
					() => GetDefaultRace.GetDefault(),
					() => GetDefaultRace.GetDefault());

				ShouldHaveRunBothCallsToCompletion(outcome);
				outcome.SecondResult.ShouldNotBeNull("GetDefault returned null on the thread that arrived while the type initializer was still adding values");
				outcome.SecondResult.Key.ShouldBeEqualTo("Last");
				outcome.SecondResult.ShouldBeSameInstanceAs(outcome.FirstResult);
			}

			[Test]
			public void Given_no_field_marked_DefaultKey__should_return_null()
			{
				Unranked.GetDefault().ShouldBeNull();
				// the type does have values; only the default is missing
				Unranked.GetAll().Count().ShouldBeEqualTo(2);
			}
		}

		[TestFixture]
		public class When_asked_to_GetFor
		{
			[TestCase("Banana", "Banana")]
			[TestCase("banana", "Banana")]
			[TestCase("BANANA", "Banana")]
			[TestCase("bAnAnA", "Banana")]
			[TestCase("CherryPie", "CherryPie")]
			[TestCase("cherrypie", "CherryPie")]
			[TestCase("CHERRYPIE", "CherryPie")]
			public void Given_a_key_in_any_letter_case__should_return_the_value_added_under_that_key(string lookupKey, string declaredKey)
			{
				var declared = new Dictionary<string, Fruit>
					{
						{ "Banana", Fruit.Banana },
						{ "CherryPie", Fruit.CherryPie }
					};

				var found = Fruit.GetFor(lookupKey);

				found.ShouldNotBeNull("GetFor(\"" + lookupKey + "\") returned null");
				found.ShouldBeSameInstanceAs(declared[declaredKey]);
				found.Key.ShouldBeEqualTo(declaredKey);
			}

			[TestCase("Huge")]
			[TestCase("")]
			[TestCase("Med")]
			[TestCase("Medium ")]
			public void Given_a_key_that_was_never_added__should_return_null_even_though_the_type_has_a_default(string key)
			{
				// setup guard: the type does have a default, so a null below shows GetFor does not fall back to it
				Size.GetDefault().ShouldBeSameInstanceAs(Size.Medium);

				Size.GetFor(key).ShouldBeNull();
			}

			[Test]
			public void Given_a_null_key__should_return_null()
			{
				Size.GetFor(null).ShouldBeNull();
			}

			[Test]
			public void Given_another_thread_is_still_adding_the_values__should_wait_and_return_the_value_for_a_key_not_added_yet()
			{
				var outcome = RaceTheTypeInitializer(
					Gates.GetForRace,
					() => GetForRace.GetFor("Last"),
					() => GetForRace.GetFor("Last"));

				ShouldHaveRunBothCallsToCompletion(outcome);
				outcome.SecondResult.ShouldNotBeNull("GetFor(\"Last\") returned null on the thread that arrived while the type initializer was still adding values");
				outcome.SecondResult.Key.ShouldBeEqualTo("Last");
				outcome.SecondCallReturnedWhileInitializerWasPaused.ShouldBeFalse("GetFor returned before the type initializer had finished");
				outcome.FirstResult.ShouldNotBeNull("GetFor(\"Last\") returned null on the thread that ran the type initializer");
				outcome.FirstResult.Key.ShouldBeEqualTo("Last");
				outcome.SecondResult.ShouldBeSameInstanceAs(outcome.FirstResult);
			}

			[Test]
			public void Given_another_thread_is_still_adding_the_values_of_a_type_with_a_default__OrDefault_should_return_the_value_for_the_key_not_the_default()
			{
				var outcome = RaceTheTypeInitializer(
					Gates.OrDefaultWithDefaultRace,
					() => OrDefaultWithDefaultRace.GetFor("Last").OrDefault(),
					() => OrDefaultWithDefaultRace.GetFor("Last").OrDefault());

				ShouldHaveRunBothCallsToCompletion(outcome);
				outcome.SecondResult.Key.ShouldBeEqualTo("Last", "the thread that arrived while the type initializer was still adding values got the type's default instead of the value for its key");
			}

			[Test]
			public void Given_another_thread_is_still_adding_the_values_of_a_type_without_a_default__OrDefault_should_return_the_value_for_the_key_not_throw()
			{
				var outcome = RaceTheTypeInitializer(
					Gates.OrDefaultWithoutDefaultRace,
					() => OrDefaultWithoutDefaultRace.GetFor("Last").OrDefault(),
					() => OrDefaultWithoutDefaultRace.GetFor("Last").OrDefault());

				ShouldHaveRunBothCallsToCompletion(outcome);
				outcome.SecondResult.Key.ShouldBeEqualTo("Last");
			}

			[Test]
			public void Given_the_type_initializer_itself_looks_up_values__should_answer_at_once_from_the_values_added_so_far()
			{
				// run on a worker so that a deadlock fails the test after the timeout instead of hanging the run
				Reentrant found = null;
				Exception error = null;
				var thread = new Thread(() =>
					{
						try
						{
							found = Reentrant.GetFor("Third");
						}
						catch (Exception exception)
						{
							error = exception;
						}
					})
					{
						IsBackground = true
					};

				thread.Start();
				var finished = thread.Join(GenerousTimeoutMilliseconds);

				finished.ShouldBeTrue("the thread running Reentrant's type initializer blocked on its own lookups");
				error.ShouldBeNull(() => "first use of Reentrant threw " + error);
				found.ShouldNotBeNull("GetFor(\"Third\") returned null after the type initializer finished");
				found.Key.ShouldBeEqualTo("Third");
				ReentrantObservations.FoundFirst.ShouldNotBeNull("GetFor(\"FIRST\") made from inside the type initializer returned null");
				ReentrantObservations.FoundFirst.Key.ShouldBeEqualTo("First");
				ReentrantObservations.ListedKeys.ShouldBeEqualTo("First,Second");
			}
		}

		[TestFixture]
		public class When_asked_to_ToString
		{
			[Test]
			public void Should_return_the_Key()
			{
				DuplicateValue.Item.ToString().ShouldBeEqualTo(DuplicateValue.Item.Key);
			}
		}

		private static Exception ExceptionThrownBy(Action action)
		{
			try
			{
				action();
			}
			catch (Exception exception)
			{
				return exception;
			}
			return null;
		}

		private static string KeysOf<T>(IEnumerable<T> values) where T : NamedConstant
		{
			return String.Join(",", values.Select(x => x.Key).OrderBy(x => x, StringComparer.Ordinal));
		}

		/// <summary>
		///   Thread A makes firstCall and is paused by the gate inside T's type initializer, just before the last value
		///   is added. Thread B then makes secondCall and is given a short grace period to return; only then is A
		///   released. A lookup that waits for the type initializer cannot return within the grace period; one that
		///   does not answers from the values added so far.
		/// </summary>
		private static RaceOutcome<TResult> RaceTheTypeInitializer<TResult>(InitializerGate gate, Func<TResult> firstCall, Func<TResult> secondCall)
		{
			var outcome = new RaceOutcome<TResult>();
			var secondCallStarting = new ManualResetEvent(false);
			var firstThread = new Thread(() =>
				{
					try
					{
						outcome.FirstResult = firstCall();
					}
					catch (Exception exception)
					{
						outcome.FirstError = exception;
					}
				})
				{
					IsBackground = true
				};
			var secondThread = new Thread(() =>
				{
					secondCallStarting.Set();
					try
					{
						outcome.SecondResult = secondCall();
					}
					catch (Exception exception)
					{
						outcome.SecondError = exception;
					}
				})
				{
					IsBackground = true
				};

			var secondThreadStarted = false;
			try
			{
				firstThread.Start();
				outcome.InitializerPaused = gate.WaitUntilReached();
				if (outcome.InitializerPaused)
				{
					secondThread.Start();
					secondThreadStarted = true;
					secondCallStarting.WaitOne(GenerousTimeoutMilliseconds);
					outcome.SecondCallReturnedWhileInitializerWasPaused = secondThread.Join(SecondCallGraceMilliseconds);
				}
			}
			finally
			{
				gate.Release();
			}
			outcome.FirstThreadFinished = firstThread.Join(GenerousTimeoutMilliseconds);
			outcome.SecondThreadFinished = secondThreadStarted && secondThread.Join(GenerousTimeoutMilliseconds);
			return outcome;
		}

		private static void ShouldHaveRunBothCallsToCompletion<TResult>(RaceOutcome<TResult> outcome)
		{
			outcome.InitializerPaused.ShouldBeTrue("setup guard: the first call never paused inside the type initializer - the type was already initialized or the call did not run its initializer - so no race took place");
			outcome.FirstThreadFinished.ShouldBeTrue("the first call did not finish after the type initializer was released");
			outcome.SecondThreadFinished.ShouldBeTrue("the second call did not finish after the type initializer was released");
			outcome.FirstError.ShouldBeNull(() => "the first call threw " + outcome.FirstError);
			outcome.SecondError.ShouldBeNull(() => "the second call threw " + outcome.SecondError);
		}

		private sealed class RaceOutcome<TResult>
		{
			public bool FirstThreadFinished;
			public Exception FirstError;
			public TResult FirstResult;
			public bool InitializerPaused;
			public bool SecondCallReturnedWhileInitializerWasPaused;
			public Exception SecondError;
			public TResult SecondResult;
			public bool SecondThreadFinished;
		}

		private sealed class InitializerGate
		{
			private readonly ManualResetEvent _reached = new ManualResetEvent(false);
			private readonly ManualResetEvent _released = new ManualResetEvent(false);

			public void PauseHere()
			{
				_reached.Set();
				_released.WaitOne(GenerousTimeoutMilliseconds);
			}

			public void Release()
			{
				_released.Set();
			}

			public bool WaitUntilReached()
			{
				return _reached.WaitOne(GenerousTimeoutMilliseconds);
			}
		}

		// A type initializer runs once per process, so every test that needs a type nobody has used yet has its own
		// subclass below. The threaded tests reach those subclasses only through GetFor, GetAll, GetDefault and Get,
		// never through their static fields: reading a static field can run the initializer on the test thread before
		// any race starts. The gates live apart from the subclasses for the same reason.
		private static class Gates
		{
			public static readonly InitializerGate GetAllRace = new InitializerGate();
			public static readonly InitializerGate GetDefaultRace = new InitializerGate();
			public static readonly InitializerGate GetForRace = new InitializerGate();
			public static readonly InitializerGate GetRace = new InitializerGate();
			public static readonly InitializerGate OrDefaultWithDefaultRace = new InitializerGate();
			public static readonly InitializerGate OrDefaultWithoutDefaultRace = new InitializerGate();
		}

		private static class ConstructionCounts
		{
			public static int FirstUseThroughGet;
		}

		private static class ReentrantObservations
		{
			public static Reentrant FoundFirst;
			public static string ListedKeys;
		}

		private sealed class Fruit : NamedConstant<Fruit>
		{
			public static readonly Fruit Apple = new Fruit("Apple");
			public static readonly Fruit Banana = new Fruit("Banana");
			public static readonly Fruit CherryPie = new Fruit("CherryPie");

			private Fruit(string key)
			{
				Add(key, this);
			}
		}

		private sealed class Size : NamedConstant<Size>
		{
			public static readonly Size Small = new Size("Small");
			[DefaultKey]
			public static readonly Size Medium = new Size("Medium");
			public static readonly Size Large = new Size("Large");

			private Size(string key)
			{
				Add(key, this);
			}
		}

		private sealed class Unranked : NamedConstant<Unranked>
		{
			public static readonly Unranked Alpha = new Unranked("Alpha");
			public static readonly Unranked Beta = new Unranked("Beta");

			private Unranked(string key)
			{
				Add(key, this);
			}
		}

		private sealed class Currency : NamedConstant<Currency>
		{
			public static readonly Currency Dollar = new Currency("Dollar", "USD");
			public static readonly Currency Euro = new Currency("Euro", "EUR");

			private Currency(params string[] keys)
			{
				foreach (var key in keys)
				{
					Add(key, this);
				}
			}
		}

		private sealed class Direction : NamedConstant<Direction>
		{
			public static readonly Direction North = new Direction("North");
			public static readonly Direction South = new Direction("South");

			private Direction(string key)
			{
				Add(key, this);
				Add(key.ToUpperInvariant(), this);
				Add(key.ToLowerInvariant(), this);
			}
		}

		private sealed class Clash : NamedConstant<Clash>
		{
			public static readonly Clash Red = new Clash("Red");
			public static readonly Clash Blue = new Clash("Blue");

			public Clash(string key)
			{
				Add(key, this);
			}
		}

		private sealed class FirstUseThroughGet : NamedConstant<FirstUseThroughGet>
		{
			public static readonly FirstUseThroughGet One = new FirstUseThroughGet("One");
			public static readonly FirstUseThroughGet Two = new FirstUseThroughGet("Two");

			private FirstUseThroughGet(string key)
			{
				ConstructionCounts.FirstUseThroughGet++;
				Add(key, this);
			}

			// a nested type reaches the protected Get without touching FirstUseThroughGet's own static members
			public static class Lookup
			{
				public static FirstUseThroughGet By(string key)
				{
					return Get(key);
				}
			}
		}

		private sealed class GetForRace : NamedConstant<GetForRace>
		{
			public static readonly GetForRace First = new GetForRace("First", null);
			public static readonly GetForRace Second = new GetForRace("Second", null);
			public static readonly GetForRace Last = new GetForRace("Last", Gates.GetForRace);

			private GetForRace(string key, [CanBeNull] InitializerGate pauseBeforeAdding)
			{
				pauseBeforeAdding?.PauseHere();
				Add(key, this);
			}
		}

		private sealed class GetRace : NamedConstant<GetRace>
		{
			public static readonly GetRace First = new GetRace("First", null);
			public static readonly GetRace Second = new GetRace("Second", null);
			public static readonly GetRace Last = new GetRace("Last", Gates.GetRace);

			private GetRace(string key, [CanBeNull] InitializerGate pauseBeforeAdding)
			{
				pauseBeforeAdding?.PauseHere();
				Add(key, this);
			}

			public static class Lookup
			{
				public static GetRace By(string key)
				{
					return Get(key);
				}
			}
		}

		private sealed class GetAllRace : NamedConstant<GetAllRace>
		{
			public static readonly GetAllRace First = new GetAllRace("First", null);
			public static readonly GetAllRace Second = new GetAllRace("Second", null);
			public static readonly GetAllRace Last = new GetAllRace("Last", Gates.GetAllRace);

			private GetAllRace(string key, [CanBeNull] InitializerGate pauseBeforeAdding)
			{
				pauseBeforeAdding?.PauseHere();
				Add(key, this);
			}
		}

		private sealed class GetDefaultRace : NamedConstant<GetDefaultRace>
		{
			public static readonly GetDefaultRace First = new GetDefaultRace("First", null);
			public static readonly GetDefaultRace Second = new GetDefaultRace("Second", null);
			[DefaultKey]
			public static readonly GetDefaultRace Last = new GetDefaultRace("Last", Gates.GetDefaultRace);

			private GetDefaultRace(string key, [CanBeNull] InitializerGate pauseBeforeAdding)
			{
				pauseBeforeAdding?.PauseHere();
				Add(key, this);
			}
		}

		private sealed class OrDefaultWithDefaultRace : NamedConstant<OrDefaultWithDefaultRace>
		{
			[DefaultKey]
			public static readonly OrDefaultWithDefaultRace First = new OrDefaultWithDefaultRace("First", null);
			public static readonly OrDefaultWithDefaultRace Second = new OrDefaultWithDefaultRace("Second", null);
			public static readonly OrDefaultWithDefaultRace Last = new OrDefaultWithDefaultRace("Last", Gates.OrDefaultWithDefaultRace);

			private OrDefaultWithDefaultRace(string key, [CanBeNull] InitializerGate pauseBeforeAdding)
			{
				pauseBeforeAdding?.PauseHere();
				Add(key, this);
			}
		}

		private sealed class OrDefaultWithoutDefaultRace : NamedConstant<OrDefaultWithoutDefaultRace>
		{
			public static readonly OrDefaultWithoutDefaultRace First = new OrDefaultWithoutDefaultRace("First", null);
			public static readonly OrDefaultWithoutDefaultRace Second = new OrDefaultWithoutDefaultRace("Second", null);
			public static readonly OrDefaultWithoutDefaultRace Last = new OrDefaultWithoutDefaultRace("Last", Gates.OrDefaultWithoutDefaultRace);

			private OrDefaultWithoutDefaultRace(string key, [CanBeNull] InitializerGate pauseBeforeAdding)
			{
				pauseBeforeAdding?.PauseHere();
				Add(key, this);
			}
		}

		private sealed class Reentrant : NamedConstant<Reentrant>
		{
			public static readonly Reentrant First = new Reentrant("First", false);
			public static readonly Reentrant Second = new Reentrant("Second", false);
			public static readonly Reentrant Third = new Reentrant("Third", true);

			private Reentrant(string key, bool lookUpTheValuesAddedSoFar)
			{
				if (lookUpTheValuesAddedSoFar)
				{
					ReentrantObservations.FoundFirst = GetFor("FIRST");
					ReentrantObservations.ListedKeys = KeysOf(GetAll());
				}
				Add(key, this);
			}
		}
	}
}
