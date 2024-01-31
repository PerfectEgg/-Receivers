using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class GameRoom : IJobQueue
    {
        List<ClientSession> _session = new List<ClientSession>();
        JobQueue _jobQueue = new JobQueue();
        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>(); 

        public void Push(Action job)
        {
            _jobQueue.Push(job);
        }

        public void Flush()
        {
            foreach (ClientSession s in _session)
                s.Send(_pendingList);

            Console.WriteLine($"Flushed {_pendingList.Count} items");
            _pendingList.Clear();
        }

        public void Broadcast(ClientSession session, string chat)
        {
            S_Chat packet = new S_Chat();
            packet.playerID = session.SessionID;
            packet.chat = $"{chat} I m {packet.playerID}";
            ArraySegment<byte> segment = packet.Write();

            _pendingList.Add(segment);
              
        }

        public void Enter(ClientSession session)
        {
            _session.Add(session);
            session.Room = this;
        }

        public void Leave(ClientSession session)
        {
             _session.Remove(session);
        }

        
    }
}
