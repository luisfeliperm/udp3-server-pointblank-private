using Core.Logs;
using Core.server;

namespace Game.global.serverpacket
{
    public class SERVER_MESSAGE_ANNOUNCE_PAK : SendPacket
    {
        private string _message;
        /// <summary>
        /// Envia uma mensagem global ao jogador. (GM)
        /// </summary>
        /// <param name="msg"></param>
        public SERVER_MESSAGE_ANNOUNCE_PAK(string msg)
        {
            _message = msg;
            if (msg.Length >= 1024)
            {
                Printf.danger("[GM] Mensagem com tamanho maior a 1024 enviada! = \"" + msg + "\" ");
                SaveLog.error("[GM] Mensagem com tamanho maior a 1024 enviada! = \"" + msg + "\" ");
            }
                
        }

        public override void write()
        {
            writeH(2055);
            writeD(2); //Tipo da notícia [NOTICE_TYPE_NORMAL - 1 || NOTICE_TYPE_EMERGENCY - 2]
            writeH((ushort)_message.Length);
            writeS(_message);
        }
    }
}