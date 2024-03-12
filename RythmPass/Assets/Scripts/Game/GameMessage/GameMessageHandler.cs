using System.Collections;
using System.Collections.Generic;
namespace Dev
{
    public interface IGameMessage { }
    public interface IGameMessageReceiver
    {
        public void ProcessGameMessage(GameMessageEnum messageName, IGameMessage message);
    }
    public static class GameMessageCaster
    {
        public static T Cast<T>(this IGameMessage message) where T : struct ,IGameMessage
        {
            if (false == message is T)  
                throw new System.Exception("Can't Cast");

            return (T)message;
        }
    }
    public class GameMessageHandler 
    {
        struct MessagePair
        {
            public GameMessageEnum Key;
            public IGameMessage Message;
        }

        readonly IGameMessageReceiver _receiver;
        readonly Queue<MessagePair> _messageQueue = new Queue<MessagePair>();
        public GameMessageHandler(IGameMessageReceiver receiver)
        {
            _receiver = receiver;
        }
        public void ProcessMessage()
        {
            while(0 != _messageQueue.Count)
            {
                MessagePair pair = _messageQueue.Dequeue();
                if(null == _receiver)
                {
                    throw new System.Exception("Message Receiver is Null");
                }

                _receiver.ProcessGameMessage(pair.Key, pair.Message);
            }
        }
        public void EnqueueMessage(GameMessageEnum name, IGameMessage message)
        {
            _messageQueue.Enqueue(new MessagePair() { Key = name, Message = message });
        }
    }
}