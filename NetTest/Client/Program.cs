using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Client c = new Client();
            c.Connect(new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 }), 45555);

            while(true)
            {
                string s = Console.ReadLine();
                byte[] b;

                BinaryFormatter binaryFormatter = new BinaryFormatter();

                using (MemoryStream memS = new MemoryStream())
                {
                    binaryFormatter.Serialize(memS, s);
                    b = memS.ToArray();
                }

                c.Send(b);
            }
        }
    }
}
