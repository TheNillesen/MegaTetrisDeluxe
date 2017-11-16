using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intermediate;

namespace Server
{
    class Program
    {
        static void Main(params string[] args)
        {
            int port = -1;

#if DEBUG
            args = new string[] { "port:6666" };
#endif

            for (int i = 0; i < args.Length; i++)
                if (args[i].Contains("port:"))
                    port = int.Parse(args[i].Split(':').Last());

            if (port < 0)
                throw new Exception("No port parameter given");

            Encryption encr = new Encryption();
            Encryption decr = new Encryption(encr.GetPublicKey());

            byte[] message = Encoding.ASCII.GetBytes("blaaah");

            byte[] encryptedPublic = decr.Encrypt(message);
            byte[] decryptedPrivate = encr.Decrypt(encryptedPublic);

            Console.WriteLine(Encoding.ASCII.GetString(message));

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(Encoding.ASCII.GetString(encryptedPublic));
            Console.ForegroundColor = decryptedPrivate.All(o => message.Contains(o)) ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine(Encoding.ASCII.GetString(decryptedPrivate));

            ConnectionHandler.Init(port);

            Console.ReadKey();
        }
    }
}
