using System;
using UnityEngine;
using ServerSession;

public class NewBehaviourScript : MonoBehaviour
{
    ServerSession _session = new ServerSession();

    // Start is called before the first frame update
    void Start()
    {
        // DNS을 사용하여 IP 주소를 세팅함.
        String host = Dns.GetHostName();
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        IPAddress IPAddr = ipHost.AddressList[0];
        // IP 주소는 식당의 이름, 포트 번호는 식당 문의 위치라고 생각하자.
        IPEndPoint endPoint = new IPEndPoint(IPAddr, 7777);

        Connector connector = new Connector();

        connector.Connect(endPoint, () => { return _session; }, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
