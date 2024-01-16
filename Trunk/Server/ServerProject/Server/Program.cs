using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;
using ServerCore;

namespace Server
{
    public class Program
    {
        static Listener _listener = new Listener();
        public static GameRoom Room = new GameRoom();

        static void Main(string[] args)
        {
            // DNS을 사용하여 IP 주소를 세팅함.
            String host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress IPAddr = ipHost.AddressList[0];
            // IP 주소는 식당의 이름, 포트 번호는 식당 문의 위치라고 생각하자.
            IPEndPoint endPoint = new IPEndPoint(IPAddr, 7777);

            _listener.init(endPoint, () => { return SessionManager.Instance.Generate(); });
            Console.WriteLine("Listening...");

            while (true)
            {
                ;
            }
        }
    }
}
