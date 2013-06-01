using System.Reflection;
using FubuDocs.Infrastructure;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuDocs.Tests.Infrastructure
{
	[TestFixture]
	public class BottlesFilterTester
	{
		[TearDown]
		public void TearDown()
		{
			BottlesFilter.Reset();
		}

		[Test]
		public void should_load_directory_when_no_filters_are_added()
		{
			BottlesFilter.ShouldLoad("Test").ShouldBeTrue();
		}

		[Test]
		public void should_load_directory_for_project()
		{
			BottlesFilter.Include("fubuvalidation");
			BottlesFilter.ShouldLoad("FubuValidation.Docs").ShouldBeTrue();
			BottlesFilter.ShouldLoad("SomethingElse.Docs").ShouldBeFalse();
		}

		[Test]
		public void should_load_assembly_when_no_filters_are_added()
		{
			BottlesFilter.ShouldLoad(GetType().Assembly).ShouldBeTrue();
		}

		[Test]
		public void should_load_assembly_for_project()
		{
			BottlesFilter.Include("fubudocs");

			var assembly = Assembly.Load(AssemblyName.GetAssemblyName("FubuDocs.Docs.dll"));

			BottlesFilter.ShouldLoad(assembly).ShouldBeTrue();
			BottlesFilter.ShouldLoad(GetType().Assembly).ShouldBeFalse();
		}
	}
}