using System;
using System.Collections.Generic;
using System.Linq;

namespace FubuDocsRunner
{
    public class StepCollection
    {
        private readonly string _defaultDirectory;
        private readonly List<IStep> _steps = new List<IStep>();

        public StepCollection(string defaultDirectory)
        {
            _defaultDirectory = defaultDirectory;
        }

        public string DefaultDirectory
        {
            get { return _defaultDirectory; }
        }

        public IStep Add
        {
            set
            {
                _steps.Add(value);
            }
        }

        public bool RunSteps()
        {
            _steps.OfType<GitStep>().Each(x => x.Directory = x.Directory ?? _defaultDirectory);

            foreach (var x in _steps)
            {
                Console.WriteLine(x.Description());
                try
                {
                    if (!x.Execute()) return false;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return false;
                }
            }

            return true;
        }
    }
}