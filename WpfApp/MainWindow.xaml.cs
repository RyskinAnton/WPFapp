using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AngleSharp;
using System.Data.Objects;

namespace WpfApp
{
    public partial class MainWindow : Window
    {
        private ResumeEntities RE = new ResumeEntities();
        public MainWindow()
        {
            InitializeComponent();
            Parsing("https://ekb.zarplata.ru/resume");    
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var query =
             from item in RE.Resume
             select item;
           dataGrid.ItemsSource = query.ToList();
        }
        private async void Parsing(string website)
        {
            using (ResumeEntities res = new ResumeEntities())
            {
                HttpClient http = new HttpClient();
                var response = await http.GetByteArrayAsync(website);
                string source = Encoding.GetEncoding("utf-8").GetString(response, 0, response.Length - 1);
                source = WebUtility.HtmlDecode(source);
                HtmlDocument resultat = new HtmlDocument();
                resultat.LoadHtml(source);
                List<HtmlNode> nodes = resultat.DocumentNode.Descendants().Where
                   (x => (x.Name == "div" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("ra-elements-container"))).ToList();
                List<HtmlNode> Nodes = resultat.DocumentNode.Descendants().Where
                   (x => (x.Name == "ul" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("ra-elements-list-hidden"))).ToList();
                var li = Nodes[0].Descendants("li").ToList();

                foreach (var item in li)
                {
                    var link = item.Descendants("a").ToList()[0].GetAttributeValue("href", null);
                    var title = item.Descendants("h3").ToList()[0].InnerText;
                    var descrption = item.InnerText;
                    res.Resume.Add(new Resume { Link = link, Title = title, Description = descrption });
                }
            }
        }
        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
