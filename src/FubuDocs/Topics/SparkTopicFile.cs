using System.Linq;
using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Model;
using FubuMVC.Spark;
using FubuMVC.Spark.SparkModel;

namespace FubuDocs.Topics
{
    public class SparkTopicFile : ITopicFile
    {
        private readonly ViewDescriptor<Template> _viewDescriptor;

        public SparkTopicFile(ViewDescriptor<Template> viewDescriptor)
        {
            _viewDescriptor = viewDescriptor;
            _viewDescriptor.ViewModel = typeof (Topic);
        }

        public string FilePath { get { return _viewDescriptor.Template.FilePath; } }
        public string Name { get { return _viewDescriptor.Name(); } }

        public string Folder
        {
            get
            {
                string relativeFile = _viewDescriptor.RelativePath().Replace("\\", "/");
                return relativeFile.Split('.').Reverse().Skip(1).Reverse().Join(".").ParentUrl();
            }
        }

        public IViewToken ToViewToken()
        {
            var sparkDescriptor = new SparkDescriptor(_viewDescriptor.Template);
            sparkDescriptor.ViewModel = typeof (Topic);

            return new SparkViewToken(sparkDescriptor);
        }
    }
}