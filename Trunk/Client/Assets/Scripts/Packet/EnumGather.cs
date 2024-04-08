using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Packet
{
    public enum CharacterType : short
    {
        None = 0,
        User = 1,
        Monster = 2,
        // 멀티 플레이어 캐릭터 추가
        another_User = 3,
        Max,
    }
}
