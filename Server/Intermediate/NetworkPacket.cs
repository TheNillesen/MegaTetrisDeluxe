using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Intermediate
{
    [Serializable]
    public class NetworkPacket
    {
        private string head;
        private string sender;

        private List<object> data;

        public string Head
        {
            get
            {
                return head;
            }
            set
            {
                head = value;
            }
        }
        public string Sender
        {
            get
            {
                return sender;
            }
            set
            {
                sender = value;
            }
        }

        public List<object> Data
        {
            get
            {
                return data;
            }
            set
            {
                data = value;
            }
        }

        public NetworkPacket(string head, string sender, params object[] objects)
        {
            this.head = head;
            this.sender = sender;

            if (objects == null)
                data = new List<object>();
            else
                data = objects.ToList();
        }

        public byte[] Serialize()
        {
            BinaryFormatter bf = new BinaryFormatter();
            byte[] result;

            using (MemoryStream memStream = new MemoryStream())
            {
                bf.Serialize(memStream, this);

                result = memStream.ToArray();
            }

            return result;
        }

        public static NetworkPacket Deserialize(byte[] package)
        {
            BinaryFormatter bf = new BinaryFormatter();
            NetworkPacket result;

            using (MemoryStream memStream = new MemoryStream(package))
            {
                result = (NetworkPacket)bf.Deserialize(memStream);
            }

            return result;
        }
    }
}
