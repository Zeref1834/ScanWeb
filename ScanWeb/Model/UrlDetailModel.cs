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
        private string _method;
        public string Method { get; set; }
        private string _parameter;
        public string Parameter { get; set; }

        public UrlDetailModel(string method, string url, string parameter, string response)
        {
            Method = method;
            Url = url;
            Parameter = parameter;
            Response = response;
        }
    }
}
