using RegularExpression;
using System.Text.RegularExpressions;

HtmlElement BuildTree(IEnumerable<string> htmlLines)
{
    HtmlHelper htmlHelper = HtmlHelper.Instance;
    HtmlElement root = null;
    HtmlElement currentElement = null;

    foreach (var line in htmlLines)
    {
        if (line == "/html")
            return root;

        if (line.StartsWith('/'))
        {
            if (currentElement != null)
                currentElement = currentElement.Parent;
            continue;
        }

        int spaceIndex = line.IndexOf(' ');
        string tagName = (spaceIndex != -1) ? line.Substring(0, spaceIndex) : line;

        var newElement = new HtmlElement();
        newElement.Name = tagName;

        var restOfString = line.Remove(0, tagName.Length);
        var attributes = Regex.Matches(restOfString, "([a-zA-Z]+)=\\\"([^\\\"]*)\\\"")
            .Cast<Match>()
            .Select(m => $"{m.Groups[1].Value}=\"{m.Groups[2].Value}\"").ToList();

        if (newElement.Attributes == null)
            newElement.Attributes = new List<string>();

        newElement.Attributes.AddRange(attributes);

        var idAttribute = attributes.FirstOrDefault(a => a.StartsWith("id"));

        if (!string.IsNullOrEmpty(idAttribute))
            newElement.Id = idAttribute.Split('=')[1].Trim('"');

        if (htmlHelper.HtmlTags.Contains(tagName) && !htmlHelper.HtmlVoidTags.Contains(tagName))
        {
            if (currentElement != null)
            {
                if (currentElement.Children == null)
                    currentElement.Children = new List<HtmlElement>();

                currentElement.Children.Add(newElement);
                newElement.Parent = currentElement;
            }
            currentElement = newElement;

            if (root == null)
                root = currentElement;
        }
        else
            if (currentElement != null)
                currentElement.InnerHtml = line;
    }
        return root;
}

async Task<string> Load(string url)
{
    HttpClient client = new HttpClient();
    var response = await client.GetAsync(url);
    var html = await response.Content.ReadAsStringAsync();
    return html;
}


var html = await Load("https://hebrewbooks.org/beis");
var cleanHtml = new Regex("\\s+").Replace(html, " ");
var htmlLines = new Regex("<(.*?)>").Split(cleanHtml).Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

var tree = BuildTree(htmlLines);
string selectorString = "head#mshead";
Selector selector = Selector.ParseSelectorString(selectorString);
HashSet<HtmlElement> selectedElements = HtmlElementExtensions.FindElementsBySelector(tree, selector);

Console.ReadLine();



