using System;
using UnityEngine;
using ServerSession;

public class NewBehaviourScript : MonoBehaviour
{
    ServerSession _session = new ServerSession();

    // Start is called before the first frame update
    void Start()
    {
        // DNS�� ����Ͽ� IP �ּҸ� ������.
        String host = Dns.GetHostName();
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        IPAddress IPAddr = ipHost.AddressList[0];
        // IP �ּҴ� �Ĵ��� �̸�, ��Ʈ ��ȣ�� �Ĵ� ���� ��ġ��� ��������.
        IPEndPoint endPoint = new IPEndPoint(IPAddr, 7777);

        Connector connector = new Connector();

        connector.Connect(endPoint, () => { return _session; }, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
