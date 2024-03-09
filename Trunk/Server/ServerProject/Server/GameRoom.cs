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
            //Console.WriteLine($"Flushed {_pendingList.Count} items");
            _pendingList.Clear();
        }

        public void Broadcast(ArraySegment<byte> segment)
        {
            _pendingList.Add(segment);
        }

        public void Enter(ClientSession session)
        {
            // 플레이어 추가한다.
            _session.Add(session);
            session.Room = this;

            // 신입생한테 모든 플레이어 목록 전송
            S_PlayerList players = new S_PlayerList();
            foreach (ClientSession s in _session)
            {
                players.players.Add(new S_PlayerList.Player()
                { 
                    isSelf = ( s == session),
                    playerId = s.SessionID,
                    posX = s.PosX,
                    posY = s.PosY,
                });
            }
            session.Send(players.Write());

            // 신입생이 입장한 사실을 모든 플레이어에게 전송
            S_BroadcastEnterGame enter = new S_BroadcastEnterGame();
            enter.playerId = session.SessionID;
            enter.posX = 0;
            enter.posY = 0;
            Broadcast(enter.Write());
        }

        public void Leave(ClientSession session)
        {
            // 플레이어 제거
             _session.Remove(session);

            // 모두에게 전송
            S_BroadcastLeaveGame leave = new S_BroadcastLeaveGame();
            leave.playerId = session.SessionID;
            Broadcast(leave.Write());
        }

        public void Move(ClientSession session, C_Move packet) 
        {
            // 좌표를 바꿔주고
            session.PosX = packet.posX;
            session.PosY = packet.posY;

            // 모두에게 알리기
            S_BroadcastMove move = new S_BroadcastMove();
            move.playerId = session.SessionID;
            move.posX = session.PosX;
            move.posY = session.PosY;
            Broadcast(move.Write());
        }
    }
}
