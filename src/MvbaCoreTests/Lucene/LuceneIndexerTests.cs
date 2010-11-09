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

using Lucene.Net.Index;

using MvbaCore.Lucene;

using NUnit.Framework;

using Rhino.Mocks;

using StructureMap.AutoMocking;

namespace MvbaCoreTests.Lucene
{
	public class LuceneIndexerTests
	{
		[TestFixture]
		public class When_asked_to_delete_a_non_indexed_entity_type_from_the_index
		{
			private string _entity;
			private LuceneIndexer _indexer;
			private ILuceneWriter _writer;

			[SetUp]
			public void BeforeEachTest()
			{
				var mocker = new RhinoAutoMocker<LuceneIndexer>();
				_indexer = mocker.ClassUnderTest;
				_writer = mocker.Get<ILuceneWriter>();
			}

			[TearDown]
			public void AfterEachTest()
			{
				_writer.VerifyAllExpectations();
			}

			[Test]
			public void Given_an_entity_type_that_is_not_indexed()
			{
				Test.Verify(
					with_an_entity_type_that_is_not_indexed,
					when_asked_to_delete_the_entity_from_the_index,
					should_not_call_writer_DeleteDocuments
					);
			}

			private void should_not_call_writer_DeleteDocuments()
			{
				_writer.Expect(x => x.DeleteDocuments(Arg<Term>.Is.Anything)).Repeat.Never();
			}

			private void when_asked_to_delete_the_entity_from_the_index()
			{
				_indexer.DeleteFromIndex(_entity);
			}

			private void with_an_entity_type_that_is_not_indexed()
			{
				_entity = "";
			}
		}
	}
}