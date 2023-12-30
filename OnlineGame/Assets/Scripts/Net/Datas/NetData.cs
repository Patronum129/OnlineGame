namespace Net.Datas
{
    //1001
    public class ChatMsgC2S
    {
        public string playerName;
        public string msg;
    }

    //服务器转发给所有在线的客户端
    public class ChatMsgS2C
    {
        public string playerName;
        public string msg;
    }
}