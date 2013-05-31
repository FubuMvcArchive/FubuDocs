using System;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Rendering;

namespace FubuDocs.Topics
{
    // Only using integration tests on this
    public class TopicBehaviorNode : BehaviorNode, IMayHaveInputType
    {
        private readonly Topic _topic;
        private readonly ViewNode _view;

        public TopicBehaviorNode(Topic topic, ViewNode view)
        {
            _topic = topic;
            _view = view;
        }

        public Topic Topic
        {
            get { return _topic; }
        }

        public ViewNode View
        {
            get { return _view; }
        }

        public override BehaviorCategory Category
        {
            get { return BehaviorCategory.Output; }
        }

        public Type InputType()
        {
            return typeof (Topic);
        }

        protected override ObjectDef buildObjectDef()
        {
            ObjectDef def = ObjectDef.ForType<TopicBehavior>();

            def.DependencyByValue(typeof (Topic), Topic);
            ObjectDef viewDef = Topic.File.ToViewToken().ToViewFactoryObjectDef();
            def.Dependency(typeof (IViewFactory), viewDef);

            return def;
        }
    }
}