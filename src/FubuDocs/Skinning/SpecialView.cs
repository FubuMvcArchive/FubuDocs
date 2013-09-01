using System;
using System.Collections.Generic;
using FubuCore.Descriptions;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Rendering;

namespace FubuDocs.Skinning
{
    public class SpecialView<T> : WriterNode
    {
        private readonly ObjectDef _def;

        public SpecialView(IViewToken view)
        {
            _def = new ObjectDef(typeof (SpecialViewWriter<>), typeof (T));

            _def.DependencyByType<IViewFactory>(view.ToViewFactoryObjectDef());
        }

        protected override ObjectDef toWriterDef()
        {
            return _def;
        }

        protected override void createDescription(Description description)
        {
            description.Title = "Special View";
        }

        public override Type ResourceType
        {
            get { return typeof (T); }
        }

        public override IEnumerable<string> Mimetypes
        {
            get { return new string[] {MimeType.Html.ToString()}; }
        }
    }
}