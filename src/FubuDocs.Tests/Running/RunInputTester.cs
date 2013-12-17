using FubuDocsRunner.Running;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuDocs.Tests.Running
{
    [TestFixture, Explicit("Dependency issue on CI")]
    public class RunInputTester
    {
        [Test]
        public void create_an_application_request_maps_open_flag()
        {
            new RunInput{OpenFlag = true}.ToRequest()
                .OpenFlag.ShouldBeTrue();

            new RunInput { OpenFlag = false }.ToRequest()
                .OpenFlag.ShouldBeFalse();
        }

        [Test]
        public void build_flag_is_debug_by_default()
        {
            new RunInput().BuildFlag.ShouldEqual("Debug");
        }

        [Test]
        public void maps_build_flag()
        {
            new RunInput {BuildFlag = "release"}
                .ToRequest()
                .BuildFlag.ShouldEqual("release");
        }

    }
}