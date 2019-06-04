using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CustomHttpServer
{
    public class Responce
    {
        private Byte[] data = null;
        private string status;
        private string mime;

        private static IDictionary<string, string> _mimeTypeMappings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase) {
        {".asf", "video/x-ms-asf"},
        {".asx", "video/x-ms-asf"},
        {".avi", "video/x-msvideo"},
        {".bin", "application/octet-stream"},
        {".cco", "application/x-cocoa"},
        {".crt", "application/x-x509-ca-cert"},
        {".css", "text/css"},
        {".deb", "application/octet-stream"},
        {".der", "application/x-x509-ca-cert"},
        {".dll", "application/octet-stream"},
        {".dmg", "application/octet-stream"},
        {".ear", "application/java-archive"},
        {".eot", "application/octet-stream"},
        {".exe", "application/octet-stream"},
        {".flv", "video/x-flv"},
        {".gif", "image/gif"},
        {".hqx", "application/mac-binhex40"},
        {".htc", "text/x-component"},
        {".htm", "text/html"},
        {".html", "text/html"},
        {".ico", "image/x-icon"},
        {".img", "application/octet-stream"},
        {".iso", "application/octet-stream"},
        {".jar", "application/java-archive"},
        {".jardiff", "application/x-java-archive-diff"},
        {".jng", "image/x-jng"},
        {".jnlp", "application/x-java-jnlp-file"},
        {".jpeg", "image/jpeg"},
        {".jpg", "image/jpeg"},
        {".js", "application/x-javascript"},
        {".mml", "text/mathml"},
        {".mng", "video/x-mng"},
        {".mov", "video/quicktime"},
        {".mp3", "audio/mpeg"},
        {".mpeg", "video/mpeg"},
        {".mpg", "video/mpeg"},
        {".msi", "application/octet-stream"},
        {".msm", "application/octet-stream"},
        {".msp", "application/octet-stream"},
        {".pdb", "application/x-pilot"},
        {".pdf", "application/pdf"},
        {".pem", "application/x-x509-ca-cert"},
        {".pl", "application/x-perl"},
        {".pm", "application/x-perl"},
        {".png", "image/png"},
        {".prc", "application/x-pilot"},
        {".ra", "audio/x-realaudio"},
        {".rar", "application/x-rar-compressed"},
        {".rpm", "application/x-redhat-package-manager"},
        {".rss", "text/xml"},
        {".run", "application/x-makeself"},
        {".sea", "application/x-sea"},
        {".shtml", "text/html"},
        {".sit", "application/x-stuffit"},
        {".swf", "application/x-shockwave-flash"},
        {".tcl", "application/x-tcl"},
        {".tk", "application/x-tcl"},
        {".txt", "text/plain"},
        {".war", "application/java-archive"},
        {".wbmp", "image/vnd.wap.wbmp"},
        {".wmv", "video/x-ms-wmv"},
        {".xml", "text/xml"},
        {".xpi", "application/x-xpinstall"},
        {".zip", "application/zip"}
        };
        private Responce(string _status, string _extention, Byte[] _data)
        {
            data = _data;
            status = _status;
            mime = _mimeTypeMappings[_extention];
        }

        public static Responce From(Request _request)
        {
            if (_request == null)
                return MakeNullResponce();
            if (_request.Type == "GET")
            {
                string filePath = Environment.CurrentDirectory + HttpService.WebDirectory + _request.Url;
                FileInfo fi = new FileInfo(filePath);
                if (fi.Exists && fi.Extension.Contains("."))
                {
                    return MakeFromFile(fi);
                }
                else
                {
                    DirectoryInfo di = new DirectoryInfo(fi + "\\");
                    if (!di.Exists)
                        return Make404Responce();

                    FileInfo[] files = di.GetFiles();
                    foreach (var ff in files)
                    {
                        string n = ff.Name;
                        if (n.Contains("index.html") || n.Contains("index.htm") || n.Contains("default.html") || n.Contains("default.htm"))
                        {
                            return MakeFromFile(ff);
                        }
                    }
                }
            }
            else
            {
                return MakeMethodNotAllowed();
            }
            return Make404Responce();
        }

        private static Responce MakeFromFile(FileInfo fi)
        {

            byte[] bytes = GetView(fi);

            return new Responce("200 OK", Path.GetExtension(fi.Name), bytes);
        }

        private static Responce MakeMethodNotAllowed() // burda faylin adini ve statusu deyismek lazimdi
        {

            byte[] bytes = GetView(HttpService.MsgDirectory, "400.html");

            return new Responce("400 Bad Request", Path.GetExtension("400.html"), bytes);
        }

        private static Responce MakeNullResponce()
        {
            byte[] bytes = GetView(HttpService.MsgDirectory, "400.html");

            return new Responce("400 Bad Request", Path.GetExtension("400.html"), bytes);
        }

        private static Responce Make404Responce()
        {
            byte[] bytes = GetView(HttpService.MsgDirectory, "404.html");

            return new Responce("404 Page Not Found", Path.GetExtension("404.html"), bytes);
        }

        public void Post(NetworkStream ns)
        {
            StreamWriter sw = new StreamWriter(ns);
            string content = $"{HttpService.Version} {status} {Environment.NewLine} " +
                $"Server: {HttpService.ServiceName} {Environment.NewLine} " +
                $"Content type : {mime} {Environment.NewLine}" +
                $"Accept ranges: bytes {Environment.NewLine}" +
                $"Content lengh : {data.Length} {Environment.NewLine} ";
            sw.Write(content);
            sw.Flush();

            ns.Write(data, 0, data.Length);
        
            //sw.Dispose();
        }

        private static byte[] GetView(string directory, string file)
        {
            string filePath = Environment.CurrentDirectory + directory + file;
            FileInfo fi = new FileInfo(filePath);
            FileStream fs = fi.OpenRead();
            BinaryReader br = new BinaryReader(fs);
            byte[] bytes = new byte[fs.Length];
            br.Read(bytes, 0, bytes.Length);
            fs.Close();
            return bytes;
        }

        private static byte[] GetView(FileInfo file)
        {
            FileStream fs = file.OpenRead();
            BinaryReader br = new BinaryReader(fs);
            byte[] bytes = new byte[fs.Length];
            br.Read(bytes, 0, bytes.Length);
            fs.Close();
            return bytes;
        }
    }
}
