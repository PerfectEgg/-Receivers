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
        // DNS을 사용하여 IP 주소를 세팅함.
        string host = Dns.GetHostName();
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        IPAddress IPAddr = ipHost.AddressList[0];
        // IP 주소는 식당의 이름, 포트 번호는 식당 문의 위치라고 생각하자.
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
