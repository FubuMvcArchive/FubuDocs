using System.Reflection;
using FubuCore;
using FubuCore.Binding;
using FubuMVC.Diagnostics.Runtime;
using FubuMVC.Diagnostics.Runtime.Tracing;

namespace FubuDocs.Infrastructure.Binders
{
    public class RequestLogPropertyBinder : IPropertyBinder
    {
        public bool Matches(PropertyInfo property)
        {
            return property.PropertyType.CanBeCastTo<RequestLog>()
                   && property.Name.EqualsIgnoreCase("CurrentRequest");
        }

        public void Bind(PropertyInfo property, IBindingContext context)
        {
            var log = context.Service<IRequestLogBuilder>().BuildForCurrentRequest();
            property.SetValue(context.Object, log, null);
        }
    }
}