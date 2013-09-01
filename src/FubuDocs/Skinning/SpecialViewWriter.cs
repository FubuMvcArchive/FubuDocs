using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.View.Rendering;

namespace FubuDocs.Skinning
{
    public class SpecialViewWriter<T> : IMediaWriter<T>
    {
        private readonly IOutputWriter _writer;
        private readonly IViewFactory _factory;
        private readonly IServiceLocator _services;

        public SpecialViewWriter(IOutputWriter writer, IViewFactory factory, IServiceLocator services)
        {
            _writer = writer;
            _factory = factory;
            _services = services;
        }

        public void Write(string mimeType, T resource)
        {
            var view = _factory.GetView();
            view.Page.ServiceLocator = _services;
            view.Render();

            _writer.ContentType(MimeType.Html);
        }

        public IEnumerable<string> Mimetypes
        {
            get { yield return MimeType.Html.ToString(); }
        }
    }
}