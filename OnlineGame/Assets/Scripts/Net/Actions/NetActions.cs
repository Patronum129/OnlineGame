using System;
using System.Collections.Generic;
using Net.Datas;

namespace Net.Actions
{
    public static class NetActions
    {
        public static Action<ChatMsg> ChatHandle;//1001

        public static Action<string> ReadyHandle;//1002

        public static Action<string> JoinHandle;//1003
        
        public static Action<List<string>> RoomPlayerUIHandle;//1004

        public static Action StartGameHandle;//1005
    }
}