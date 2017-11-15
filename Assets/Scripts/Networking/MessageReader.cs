using Game;
using Networking.Data;
using UnityEngine;

namespace Networking
{
    public static class MessageReader
    {
        public static void ReadMessage(byte[] message)
        {
            Envelope envelope = SerializationHandler.Deserialize<Envelope>(message);
            if (envelope == null) throw new NotAnEnvelopeException();
            switch (envelope.PacketType)
            {
                case Packets.None:
                    Debug.Log("None");
                    break;
                case Packets.String:
                    string value = SerializationHandler.Deserialize<string>(envelope.Packet);
                    Debug.Log(value);
                    break;
                case Packets.PlayerUpdate:
                    PlayerUpdate update = SerializationHandler.Deserialize<PlayerUpdate>(envelope.Packet);
                    break;
                case Packets.PlayerSync:
                    PlayerSync sync = SerializationHandler.Deserialize<PlayerSync>(envelope.Packet);
                    // TODO: Find a better way to have the player here
                    Player player = GameObject.FindObjectOfType<Player>();
                    player.UpdateFromServer(sync);
                    break;
                default:
                    break;
            }
        }
    }
}