﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DummyClient
{
    class SessionManager
    {
        static SessionManager _session = new SessionManager();
        public static SessionManager Instance { get { return _session; } }

        List<ServerSession> _sessions = new List<ServerSession>();
        object _lock = new object();
        Random _random = new Random();

        public void SendForEack()
        {
            lock (_lock) 
            {
                try
                {
                    foreach (ServerSession session in _sessions)
                    {
                        float x = (float)_random.NextDouble();
                        float y = (float)_random.NextDouble();

                        C_Move movePacket = new C_Move();
                        movePacket.posX = x;
                        movePacket.posY = y;

                        session.Send(movePacket.Write());
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{e.ToString()}");
                }
                
            }
        }

        public ServerSession Generate()
        {
            lock (_lock) 
            {
                ServerSession session = new ServerSession();
                _sessions.Add(session);
                return session;
            }
        }
    }
}
