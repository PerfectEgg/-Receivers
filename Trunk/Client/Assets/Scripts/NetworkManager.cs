using System;
using UnityEngine;
using System.Net;
using DummyClient;
using ServerCore;
using System.Collections;
using System.Collections.Generic;
using static S_PlayerList;

public class NetworkManager : MonoBehaviour
{
    ServerSession _session = new ServerSession();
    public static int _playerId;

    public void Send(ArraySegment<byte> sendBuff)
    {
        _session.Send(sendBuff);
    }

    public void SetPlayerId(int id)
    {
        _playerId = id;
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
        SetPlayerId(1);
    }

    void Update()
    {
        List<IPacket> list = PacketQueue.Instance.PopAll();
        foreach (IPacket packet in list)
            PacketManager.Instance.HandlePacket(_session, packet);
    }
}
