using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace PaperFetcher
{
    public partial class Form1 : Form
    {
        List<Paper> papers = new List<Paper>();
        public Form1()
        {
            InitializeComponent();
            SearchResultTreeView.CheckBoxes = true;
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            Search(SearchTextBox.Text);
        }
        private void Search(string word)
        {
            int start_page = 0;
            string query_word = word;
            string url = $"https://scholar.google.com.hk/scholar?start={start_page}&q={query_word}";
           
            GetPaperByUrlInGoogle(url, papers);
            
            foreach(var paper in papers)
            {
                TreeNode node = new TreeNode();
                node.Text = paper.Title;
                SearchResultTreeView.Nodes.Add(node);
            }
        }

        private void GetPaperByUrlInGoogle(string url, List<Paper> papers)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

            req.Method = "GET";
            req.ContentType = "text/html";

            req.CookieContainer = new CookieContainer();
            SetCookie(req);

            string useragent_str = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36";

            req.UserAgent = useragent_str;
            req.KeepAlive = false;

            var res = req.GetResponse();
            Stream s = res.GetResponseStream();

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.Load(s, Encoding.UTF8);
            var nodes = doc.DocumentNode.SelectNodes("//*[@id=\"gs_ccl_results\"]/div");
            foreach (var node in nodes)
            {
                string title = node.SelectSingleNode(".//*[contains(@class, 'gs_rt')]").InnerText;
                string cite = node.SelectSingleNode(".//*[contains(@class, 'gs_fl')]/a").InnerText;

                papers.Add(new Paper { Title = title, GoogleCite = Convert.ToInt32(cite.Split('：')[1].Trim()) });
            }
        }

        private void SetCookie(HttpWebRequest req)
        {
            req.CookieContainer.Add(new Cookie("GSP", "A=0N0I8w:CPTS=1481166247:LM=1481166247:S=meXgr-ZhWCDVv4dj", "/", "scholar.google.com.hk"));
            req.CookieContainer.Add(new Cookie("SID", "EgREE-h0bt5yJOG2HS9NP6V_Czel-Tu2WP2JmeDf6ZPwGqaFqQacbdz6816m6IDY9wj9Cg.", "/", "scholar.google.com.hk"));
            req.CookieContainer.Add(new Cookie("HSID", "ABGLWV1yuhh2wCew6", "/", "scholar.google.com.hk"));
            req.CookieContainer.Add(new Cookie("SSID", "A62tc6AKuA8p_2wl4", "/", "scholar.google.com.hk"));
            req.CookieContainer.Add(new Cookie("APISID", "Yql3vIWYg0sxdLDc/Abd3hvjbc0glOKiDi", "/", "scholar.google.com.hk"));
            req.CookieContainer.Add(new Cookie("SAPISID", "mS_e5B9HM47cgBQ0/AkyxTYWD-q2OdY3eD", "/", "scholar.google.com.hk"));
            req.CookieContainer.Add(new Cookie("OGPC", "448059392-3:527891456-4:", "/", "scholar.google.com.hk"));
            req.CookieContainer.Add(new Cookie("GOOGLE_ABUSE_EXEMPTION", "ID=7e281a74e30d50a7:TM=1488962351:C=r:IP=221.226.44.144-:S=APGng0v4l35tyRoB5Zfo2_F-5zvyW-kweg", "/", "scholar.google.com.hk"));
            req.CookieContainer.Add(new Cookie("NID", "98=CnQ10lmRfMKPl12JsTKJG8xMNXP_u4ft9jYOBai8iJPQYXa8ryfnDAgETO1xWNLDRHW0RxcJ8X2sJYXeWi9Oem0TxWcd5owadC7mVCNBzZH3Qq26Ru5tqN64q_-qnJqLSQl5EXZlti8bfAVeUxfBk2-RxjIOg38n2iz96zifl9HZ6I4YG9Ls9gMob1sklvweBjAwy7H-uztV4azaWpDWkEPpIIp8LFR1NRnv2MZVt_GKdUXwkCSKP-E-pslxsyE", "/", "scholar.google.com.hk"));
        }

        private void SearchResultTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            //判断中英文
            if(IsChinese(e.Node.Text))
            {
                //中文
                //万方获取DOI
                

                //baidu获取详细信息





            }
            else
            {
                //英文，用bing获取详细信息
                string bing_url = GetPaperBingUrl(e.Node.Text);

                GetPaperDetailByBing(bing_url, e.Node.Text);
            }

            //设置详细信息
            

            //填充参考文献

        }

        private void GetDetailByBaidu(string title)
        {
            string bd_url = $"http://xueshu.baidu.com/s?wd={title}";

            var req = WebRequest.Create(title) as HttpWebRequest;

            var res = req.GetResponse();
            Stream s = res.GetResponseStream();

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

            doc.Load(s, Encoding.UTF8);

            string t = doc.DocumentNode.SelectSingleNode("//*[@id=\"1\"]/div[1]/h3").InnerText;

        }

        private string GetPaperBingUrl(string title)
        {
            string bing_url = $"http://cn.bing.com/academic/search?q={title}";

            var req = WebRequest.Create(bing_url) as HttpWebRequest;

            var res = req.GetResponse();
            Stream s = res.GetResponseStream();

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.Load(s, Encoding.UTF8);
            var paper_url_e = "http://cn.bing.com" + doc.DocumentNode.SelectSingleNode("//*[@id=\"b_results\"]/li[1]/h2/a").Attributes["href"].Value.ToString();
            return WebUtility.HtmlDecode(paper_url_e);
        }

        private void GetPaperDetailByBing(string url, string title)
        {
            //get bing id


            //req
            var req = WebRequest.Create(url) as HttpWebRequest;

            var res = req.GetResponse();
            Stream s = res.GetResponseStream();

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.Load(s, Encoding.UTF8);

            //title
            var title_new = doc.DocumentNode.SelectSingleNode("//*[@id=\"b_content\"]/ol[1]/li[1]").InnerText;

            if (title.Trim().ToUpper() != title_new.Trim().ToUpper())
            {
                MessageBox.Show("文章标题不一致");
                return;
            }

            var paper = (from p in papers
                         where title == p.Title
                         select p).FirstOrDefault();

            if (paper == null)
                return;


            //遍历，检查元素
            var nodes = doc.DocumentNode.SelectNodes("//*[@id=\"b_content\"]/ol[1]/li[3]/ul/li");
            foreach(var node in nodes)
            {
                string label = node.SelectSingleNode("./div/span[1]/span[1]").InnerText;
                label = Regex.Replace(label, @"\s", "");

                switch(label)
                {
                    case "作者":
                        {
                            //author
                            var author_nodes = node.SelectNodes("./div/span[2]/div/span/a");
                            if (author_nodes != null)
                            {
                                paper.Authors = new List<string>();
                                foreach (var a_node in author_nodes)
                                    paper.Authors.Add(a_node.InnerText.Trim());
                            }
                        }
                        break;

                    case "摘要":
                        {
                            //abstrect
                            var abstract_node = node.SelectSingleNode("./div/span[2]/div/span/span");
                            if (abstract_node != null)
                            {
                                paper.Abstract = abstract_node.Attributes["title"].Value.ToString();
                            }
                        }
                        break;

                    case "发表日期":
                        {
                            //date
                            var date_node = node.SelectSingleNode("./div/span[2]/div");
                            if (date_node != null)
                            {
                                paper.PublishDate = date_node.InnerText.Trim();
                            }
                        }
                        break;

                    case "被引量":
                        {
                            //bing_cite
                            var bingcite_node = node.SelectSingleNode("./div/span[2]/div");
                            if (bingcite_node != null)
                            {
                                paper.BingCIte = Convert.ToInt32(bingcite_node.InnerText.Trim());
                            }
                        }
                        break;

                    case "DOI":
                        {
                            //DOI
                            var doi_node = node.SelectSingleNode("./div/span[2]/div");
                            if (doi_node != null)
                            {
                                paper.DOI = doi_node.InnerText.Trim();
                            }
                        }
                        break;

                    default:
                        break;                     
                }
            }

            //获取参考文献
        }

        private bool IsChinese(string s)
        {
            Regex rx = new Regex("[\u4e00-\u9fa5]");
            if (rx.IsMatch(s))
                return true;
            else
                return false;
        }
    }
}
