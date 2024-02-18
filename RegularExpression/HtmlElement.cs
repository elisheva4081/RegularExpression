using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegularExpression
{
    public class HtmlElement
    {
        public string Name { get; set; }
        public List<string> Attributes { get; set; }
        public string Id { get; set; }
        public List<string> Classes { get; set; }
        public string InnerHtml { get; set; }
        public HtmlElement Parent { get; set; }
        public List<HtmlElement> Children { get; set; }

        public HtmlElement() { }

        public HtmlElement(string id, string name, List<string> attributes, List<string> classes, string innerHtml, HtmlElement parent, List<HtmlElement> children)
        {
            Id = id;
            Name = name;
            Attributes = attributes;
            Classes = classes;
            InnerHtml = innerHtml;
            Parent = parent;
            Children = children;
        }

        public IEnumerable<HtmlElement> Descendants()
        {
            Queue<HtmlElement> queue = new Queue<HtmlElement>();
            queue.Enqueue(this);

            while (queue.Count > 0)
            {
                HtmlElement currentHtml = queue.Dequeue();
                yield return currentHtml;

                if (currentHtml.Children != null)
                    foreach (HtmlElement child in currentHtml.Children)
                        queue.Enqueue(child);
            }
        }

        public IEnumerable<HtmlElement> Ancestors()
        {
            HtmlElement current = this.Parent;

            while (current != null)
            {
                yield return current;
                current = current.Parent;
            }
        }
    }
    public static class HtmlElementExtensions
    {
        public static HashSet<HtmlElement> FindElementsBySelector(HtmlElement element, Selector selector)
        {
            if (element == null || selector == null)
                return new HashSet<HtmlElement>();
            HashSet<HtmlElement> result = new HashSet<HtmlElement>();
            foreach (var child in element.Descendants())
                FindElements(child, selector, result);
            return result;
        }

        private static void FindElements(HtmlElement element, Selector selector, HashSet<HtmlElement> result)
        {
            if (element == null || selector == null)
                return;
            Selector current = selector;
            HtmlElement n = null;
            foreach (var child in element.Descendants())
            {
                if (current != null && MatchesSelector(child, current))
                {
                    if (current == selector)
                        n = child;
                    current = current.Child;

                }
                if (current == null)
                {
                    result.Add(n);
                    break;
                }
            }
        }
        
        private static bool MatchesSelector(HtmlElement element, Selector selector)
        {
            if (((!string.IsNullOrEmpty(selector.TagName) && selector.TagName == element.Name) || string.IsNullOrEmpty(selector.TagName)) && ((selector.Id != null && selector.Id == element.Id) || selector.Id == null))
            {
                if (selector.Classes != null && selector.Classes.Count > 0)
                {
                    if (element.Classes != null)
                        return selector.Classes.All(item => element.Classes.Contains(item));
                    return false;
                }
                return true;
            }
            return false;
            
        }

    }
}

