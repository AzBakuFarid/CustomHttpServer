using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomHttpServer
{
    public class Request
    {
        public string Type { get; set; }
        public string Url { get; set; }
        public string Host { get; set; }

        private Request(string _type, string _url, string _host)
        {
            Type = _type;
            Url = _url;
            Host = _host;
        }

        public static Request GetRequest(string _request) {
            if (string.IsNullOrEmpty(_request)) 
                return null;
            string[] tokens = _request.Split('\r','\n',' ');
            string c_type = tokens[0];
            string c_url = tokens[1].Substring(1);
            string c_host = tokens[27];
            return new Request(c_type, c_url, c_host);
        }
    }
}
