using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DummyClient
{
    class Program
    {
        static void Main(string[] args)
        {
            // DNS을 사용하여 IP 주소를 세팅함.
            String host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress IPAddr = ipHost.AddressList[0];
            // IP 주소는 식당의 이름, 포트 번호는 식당 문의 위치라고 생각하자.
            IPEndPoint endPoint = new IPEndPoint(IPAddr, 7777);

            Connector connector = new Connector();

            connector.Connect(endPoint, () => { return SessionManager.Instance.Generate(); }, 
                10);

            while (true)
            {
                try
                {
                    SessionManager.Instance.SendForEack();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

                Thread.Sleep(250);
            }
        }
    }
}
