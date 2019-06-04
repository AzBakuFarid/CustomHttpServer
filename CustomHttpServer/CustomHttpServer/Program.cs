using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomHttpServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting service on port 8080");
            HttpService service = new HttpService(8080);
            service.Start();
        }
    }
}
