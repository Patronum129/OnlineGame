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

        public static Action<SyncTransformMsg> SyncTransformHandle;//1006

        public static Action<SyncAnimatorMsg> SyncAnimatorHandle;//1007

        public static Action<string> DieHandle;//1008

        public static Action<string> WinHandle;//1009

        public static Action<int> BombHandle;//1010;

        public static Action<List<int>> CreateBombHandle;//1011

        public static Action ExitGameHandle;//1012
    }
}