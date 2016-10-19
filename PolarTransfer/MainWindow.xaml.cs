using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
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

namespace PolarTransfer
{
    public partial class MainWindow : Window
    {
        CookieCollection sessionCookie;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            string returnurl = "%2F";
            string login = "aashmarin%40gmail.com";
            string password = "1qaz2wsx";
            string loginurl = "https://flow.polar.com/login";

            //login
            HttpWebRequest req2 = (HttpWebRequest)WebRequest.Create(loginurl);
            req2.Proxy = WebRequest.DefaultWebProxy;
            req2.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            req2.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
            req2.Method = "POST";
            req2.CookieContainer = new CookieContainer();
            string postData = string.Format("returnUrl={2}&email={0}&password={1}", login, password, returnurl);
            byte[] bytes = Encoding.ASCII.GetBytes(postData);
            req2.ContentLength = bytes.Length;
            using (Stream os = req2.GetRequestStream())
            {
                os.Write(bytes, 0, bytes.Length);
            }
            HttpWebResponse resp2 = (HttpWebResponse)req2.GetResponse();
            sessionCookie = resp2.Cookies;
        }

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            // download training
            string url = "https://flow.polar.com/training/analysis/491398793/export/tcx/false";
            HttpWebRequest req2 = (HttpWebRequest)WebRequest.Create(url);
            req2.Proxy = WebRequest.DefaultWebProxy;
            req2.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            req2.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
            req2.Method = "GET";
            req2.CookieContainer = new CookieContainer();
            req2.CookieContainer.Add(sessionCookie);
            HttpWebResponse resp2 = (HttpWebResponse)req2.GetResponse();
            var rs = resp2.GetResponseStream();
            var fs = new FileStream("tcxpolar.tcx", FileMode.Create, FileAccess.Write);
            rs.CopyTo(fs);
            fs.Close();
        }

        private void button_Copy1_Click(object sender, RoutedEventArgs e)
        {
            string url = "https://flow.polar.com/exercises/add";

            //add training result
            HttpWebRequest req2 = (HttpWebRequest)WebRequest.Create(url);
            req2.Proxy = WebRequest.DefaultWebProxy;
            req2.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            req2.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
            req2.Method = "POST";
            req2.CookieContainer = new CookieContainer();
            req2.CookieContainer.Add(sessionCookie);
            string postData = string.Format("day=19&month=4&year=2016&hours=16&minutes=0&sport=122&note=&durationHours=1&durationMinutes=0&durationSeconds=0&distance=&maximumHeartRate=&averageHeartRate=&minimumHeartRate=&kiloCalories=&pace=&speed=&cadence=&feeling=");
            byte[] bytes = Encoding.ASCII.GetBytes(postData);
            req2.ContentLength = bytes.Length;
            using (Stream os = req2.GetRequestStream())
            {
                os.Write(bytes, 0, bytes.Length);
            }
            HttpWebResponse resp2 = (HttpWebResponse)req2.GetResponse();
        }

        private void button_Copy2_Click(object sender, RoutedEventArgs e)
        {
            //add training target

        }

        private void button_Copy3_Click(object sender, RoutedEventArgs e)
        {
            string loginurl = "http://aerobia.ru/users/sign_in";
            string pwd = "T%40shk3nter";
            string login = "aashmarin%40gmail.com";

            //login
            HttpWebRequest req2 = (HttpWebRequest)WebRequest.Create(loginurl);
            req2.Proxy = WebRequest.DefaultWebProxy;
            req2.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            req2.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
            req2.Method = "POST";
            req2.CookieContainer = new CookieContainer();
            string postData = string.Format("user%5Bemail%5D={0}&user%5Bpassword%5D={1}", login, pwd);
            byte[] bytes = Encoding.ASCII.GetBytes(postData);
            req2.ContentLength = bytes.Length;
            using (Stream os = req2.GetRequestStream())
            {
                os.Write(bytes, 0, bytes.Length);
            }
            HttpWebResponse resp2 = (HttpWebResponse)req2.GetResponse();
            var reader = new StreamReader(resp2.GetResponseStream());
            var page = reader.ReadToEnd();
        }
    }
}
