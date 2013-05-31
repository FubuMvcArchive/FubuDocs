using System;

namespace FubuDocsRunner.Exports
{
    public class HtmlElement
    {
        public static string GetAttributeValue(string input, string attribute)
        {
            var index = input.IndexOf(attribute);
            if (index == -1)
            {
                return "";
            }

            var length = attribute.Length + 2; // attribute="
            var delimiter = input.Substring(index + attribute.Length + 1, 1); // " or '
            var value = "";

            index += length;

            while (true)
            {
                if (input.Length <= index)
                {
                    Console.WriteLine(input);
                    break;
                }

                var next = input.Substring(index, 1);
                if (next == delimiter)
                {
                    break;
                }

                value += next;
                ++index;
            }

            return value;
        }
    }
}