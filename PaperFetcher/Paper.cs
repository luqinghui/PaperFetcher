using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperFetcher
{
    public class Paper
    {
        public string Title { get; set; }
        public string DOI { get; set; }
        public int GoogleCite { get; set; }
        public int BingCIte { get; set; }
        public int BaiduCite { get; set; }
        public List<string> Reference { get; set; }
        public List<string> Quote { get; set; }
        public string BingID { get; set; }
        public List<string> Authors { get; set; }
        public string Abstract { get; set; }
        public List<string> PdfUrls { get; set; }
        public List<string> Urls { get; set; }
        public string PublishDate { get; set;}
    }
}
