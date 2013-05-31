using System;
using System.Collections.Generic;
using System.Linq;

namespace FubuDocs.Topics
{
    public abstract class OrderedTopic : IComparable<OrderedTopic>
    {
        public OrderedTopic(string text)
        {
            Raw = text;
            Name = FindValue(text);
        }

        public string Raw { get; private set; }
        public string Name { get; private set; }

        public int CompareTo(OrderedTopic other)
        {
            return Topic.CompareName(Raw, other.Raw);
        }

        public static string FindValue(string text)
        {
            string[] values = text.Split('.');
            return values.Last();
        }

        public abstract IEnumerable<Topic> TopLevelTopics();
    }
}