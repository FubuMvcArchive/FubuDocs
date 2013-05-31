using FubuDocsRunner;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;

namespace FubuDocs.Tests
{
    [TestFixture, Explicit]
    public class debugging
    {
        [Test]
        public void try_to_get_usages()
        {
            var command = new BottleCommand();
            command.Usages.Usages.Any().ShouldBeTrue();
        }
    }
}