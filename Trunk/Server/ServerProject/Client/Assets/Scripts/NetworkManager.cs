using System;
using UnityEngine;
using System.Net;
using DummyClient;
using ServerCore;
using System.Collections;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviour
{
    ServerSession _session = new ServerSession();

    public void Send(ArraySegment<byte> sendBuff)
    {
        _session.Send(sendBuff);
    }

    void Start()
    {
        // DNS�� ����Ͽ� IP �ּҸ� ������.
        string host = Dns.GetHostName();
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        IPAddress IPAddr = ipHost.AddressList[0];
        // IP �ּҴ� �Ĵ��� �̸�, ��Ʈ ��ȣ�� �Ĵ� ���� ��ġ��� ��������.
        IPEndPoint endPoint = new IPEndPoint(IPAddr, 7777);

        Connector connector = new Connector();

        connector.Connect(endPoint, () => { return _session; }, 1);
    }

    void Update()
    {
        List<IPacket> list = PacketQueue.Instance.PopAll();
        foreach (IPacket packet in list)
            PacketManager.Instance.HandlePacket(_session, packet);
    }
}
