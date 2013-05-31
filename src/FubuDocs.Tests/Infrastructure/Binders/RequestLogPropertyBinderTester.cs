using System;
using System.Linq.Expressions;
using FubuCore.Binding.InMemory;
using FubuCore.Reflection;
using FubuDocs.Infrastructure.Binders;
using FubuMVC.Diagnostics.Runtime;
using FubuMVC.Diagnostics.Runtime.Tracing;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuDocs.Tests.Infrastructure.Binders
{
    [TestFixture]
    public class RequestLogPropertyBinderTester
    {
        private RequestLogPropertyBinder theBinder;

        [SetUp]
        public void SetUp()
        {
            theBinder = new RequestLogPropertyBinder();
        }

        [Test]
        public void matches_request_log_properties_happy_path()
        {
            matches(x => x.CurrentRequest).ShouldBeTrue();
        }

        [Test]
        public void does_not_match_request_log_properties_with_different_names()
        {
            matches(x => x.Something).ShouldBeFalse();
        }

        [Test]
        public void does_not_match_other_properties()
        {
            matches(x => x.SomethingElse).ShouldBeFalse();
        }

        [Test]
        public void builds_the_log()
        {
            var theLog = new RequestLog();
            var builder = MockRepository.GenerateStub<IRequestLogBuilder>();
            builder.Stub(x => x.BuildForCurrentRequest()).Return(theLog);

            BindingScenario<FakeRequestModel>.For(x =>
            {
                x.Service(builder);
                x.BindPropertyWith(theBinder, model => model.CurrentRequest);


            }).Model.CurrentRequest.ShouldBeTheSameAs(theLog);
        }

        private bool matches(Expression<Func<FakeRequestModel, object>> expression)
        {
            return theBinder.Matches(ReflectionHelper.GetProperty(expression));
        }

        public class FakeRequestModel
        {
            public RequestLog CurrentRequest { get; set; }
            public RequestLog Something { get; set; }
            public object SomethingElse { get; set; }
        }
    }
}