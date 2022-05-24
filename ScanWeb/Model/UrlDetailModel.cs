using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanWeb.Model
{
    public class UrlDetailModel
    {
        private string url;
        public string Url { get; set; }
        private string _response;
        public string Response { get; set; }

        public UrlDetailModel(string url, string response)
        {
            Url = url;
            Response = response;
        }
    }
}
