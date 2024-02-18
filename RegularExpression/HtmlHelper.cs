using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RegularExpression
{
    public class HtmlHelper
    {
        private readonly static HtmlHelper _instance = new HtmlHelper();
        public static HtmlHelper Instance=>_instance;
        public string[] HtmlTags { get; set; }
        public string[] HtmlVoidTags { get; set; }
        private HtmlHelper()
        {
            var tags = File.ReadAllText("HtmlTags.json");
            HtmlTags = JsonSerializer.Deserialize<string[]>(tags);

            var voidTags = File.ReadAllText("HtmlVoidTags.json");
            HtmlVoidTags = JsonSerializer.Deserialize<string[]>(voidTags);
        }

    }
}
