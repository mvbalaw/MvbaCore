//   * **************************************************************************
//   * Copyright (c) McCreary, Veselka, Bragg & Allen, P.C.
//   * This source code is subject to terms and conditions of the MIT License.
//   * A copy of the license can be found in the License.txt file
//   * at the root of this distribution.
//   * By using this source code in any fashion, you are agreeing to be bound by
//   * the terms of the MIT License.
//   * You must not remove this notice from this software.
//   * **************************************************************************

using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

using FluentAssert;

using JetBrains.Annotations;

using NUnit.Framework;

namespace MvbaCore.Tests
{
	[UsedImplicitly]
	public class MvbaCoreProjectTests
	{
		// Every PackageReference in MvbaCore.csproj becomes a dependency of the MvbaCore package, so every program that
		// uses MvbaCore gets that package too. A package the test project declares for itself is one only the tests need;
		// FluentAssert sat in MvbaCore.csproj unused for years that way, waiting to be handed to the next consumer.
		[TestFixture]
		public class When_MvbaCore_is_packed
		{
			[Test]
			public void Should_not_depend_on_a_package_only_the_tests_use()
			{
				var testPackages = PackageReferences("MvbaCore.Tests");
				testPackages.Any().ShouldBeTrue();

				var shared = string.Join(", ", PackageReferences("MvbaCore").Where(x => testPackages.Contains(x, StringComparer.OrdinalIgnoreCase)));

				shared.ShouldBeEqualTo("", "MvbaCore.csproj declares packages only the tests use, which belong in MvbaCore.Tests.csproj: " + shared);
			}
		}

		private static string[] PackageReferences(string projectName, [CallerFilePath] string thisFile = "")
		{
			var srcFolder = Path.GetDirectoryName(Path.GetDirectoryName(thisFile));
			return XDocument.Load(Path.Combine(srcFolder, projectName, projectName + ".csproj"))
				.Descendants("PackageReference")
				.Select(x => (string)x.Attribute("Include"))
				.ToArray();
		}
	}
}
