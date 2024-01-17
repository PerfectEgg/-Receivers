﻿using System.Net;
using System.Text;
using Server;
using ServerCore;

namespace Server
{
    class ClientSession : PacketSession
    {
        public int SessionID { get; set; }
        public GameRoom Room { get; set; }

        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

            Program.Room.Enter(this);
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            PacketManager.Instance.OnRecvPacket(this, buffer);
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            SessionManager.Instance.Remove(this);
            if (Room != null)
            { 
                Room.Leave(this);
                Room = null;
            }

            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred byte: {numOfBytes}");
        }
    }
}
