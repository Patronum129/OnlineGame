using UnityEngine;

namespace Net.Datas
{
    //1001
    public struct ChatMsg
    {
        public string playerName;
        public string msg;
    }

    public struct SyncTransformMsg
    {
        public string playerName;
        public SyncVector2 syncPosition;
        public SyncVector2 syncScale;
    }

    public struct SyncAnimatorMsg
    {
        public string playerName;
        public int triggerID;//0 idle 1 run 2 attack
    }

    public struct SyncVector2
    {
        public float x;
        public float y;

        public SyncVector2(Vector2 vec)
        {
            x = vec.x;
            y = vec.y;
        }
    }
}