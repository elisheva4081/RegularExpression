using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RegularExpression
{
    public class Selector
    {
        public string TagName { get; set; }
        public string Id { get; set; }
        public List<string> Classes { get; set; }
        public Selector Parent { get; set; }
        public Selector Child { get; set; }

        public Selector() { }

        public Selector(string tagName, string id, List<string> classes, Selector parent, Selector child)
        {
            TagName = tagName;
            Id = id;
            Classes = classes;
            Parent = parent;
            Child = child;
        }

        public static Selector ParseSelectorString(string query)
        {
            if (string.IsNullOrEmpty(query))
                throw new ArgumentException("Query string cannot be null or empty.");

            Selector currentSelector = new Selector();
            Selector rootSelector = null;

            string[] parts = query.Split(' ');

            foreach (string part in parts)
            {
                Selector newChildSelector = new Selector();
                string[] subParts = Regex.Split(part, @"(?=[.#])");

                foreach (string subPart in subParts)
                {
                    if (subPart.StartsWith("#"))
                            newChildSelector.Id = subPart.Substring(1);
                    else if (subPart.StartsWith("."))
                    {
                        if (newChildSelector.Classes == null)
                            newChildSelector.Classes = new List<string>();
                        newChildSelector.Classes.Add(subPart.Substring(1));
                    }
                    else
                        newChildSelector.TagName = subPart;
                }

                if (rootSelector == null)
                {
                    rootSelector = newChildSelector;
                    currentSelector = rootSelector;
                }
                else
                {
                    currentSelector.Child = newChildSelector;
                    newChildSelector.Parent = currentSelector;
                    currentSelector = newChildSelector;
                }
            }
            return rootSelector;
        }
    }
}

