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
                    string stringVal = SerializationHandler.Deserialize<string>(envelope.Packet);
                    Debug.Log(stringVal);
                    break;
                case Packets.AckLogin:
                    AckLogin ackLogin = SerializationHandler.Deserialize<AckLogin>(envelope.Packet);
                    HandleAckLogin(ackLogin);
                    break;
                case Packets.AckJoinTeam:
                    AckJoinTeam ackJoinTeam = SerializationHandler.Deserialize<AckJoinTeam>(envelope.Packet);
                    HandleAckJoinTeam(ackJoinTeam);
                    break;
                case Packets.DestroyObject:
                    DestroyObject destroyObject = SerializationHandler.Deserialize<DestroyObject>(envelope.Packet);
                    HandleDestroyObject(destroyObject);
                    break;
                case Packets.PlayerSync:
                    PlayerSync playerSync = SerializationHandler.Deserialize<PlayerSync>(envelope.Packet);
                    HandlePlayerSync(playerSync);
                    break;
                case Packets.SnowballSync:
                    SnowballSync snowballSync = SerializationHandler.Deserialize<SnowballSync>(envelope.Packet);
                    HandleSnowballSync(snowballSync);
                    break;
                default:
                    break;
            }
        }

        private static void HandleAckLogin(AckLogin ack)
        {
            if (ack == null) throw new WrongPacketTypeException();
            if (!ack.Success)
            {
                Debug.Log(string.Format("Unsuccessful login for UserID {0}", ack.UserID));
                return;
            }
            ConnectionManager.Instance.LoggedIn(ack.UserID);
        }

        private static void HandleAckJoinTeam(AckJoinTeam ack)
        {
            if (ack == null) throw new WrongPacketTypeException();
            if (!ack.Success)
            {
                Debug.Log(string.Format("Could not join team {0}", ack.TeamName));
                return;
            }
            TeamManager.Instance.JoinedTeam(ack.TeamName);
        }

        private static void HandleDestroyObject(DestroyObject destroy)
        {
            if (destroy == null) throw new WrongPacketTypeException();
            World.Instance.DestroyObject(destroy.ObjectID);
        }

        private static void HandlePlayerSync(PlayerSync sync)
        {
            if (sync == null) throw new WrongPacketTypeException();
            Player player = World.Instance.GetPlayer(sync.UserID);
            player.UpdateFromServer(sync);
        }

        private static void HandleSnowballSync(SnowballSync sync)
        {
            if (sync == null) throw new WrongPacketTypeException();
            GameObject snowball = World.Instance.GetObject(sync.ObjectID);
            if (snowball == null)
            {
                // Instantiate a snowball
                snowball = GameObject.Instantiate(Prefabs.Instance.SnowballPrefab);
                World.Instance.AddObject(snowball, sync.ObjectID);
            }
            snowball.transform.position = new Vector2(sync.PosX, sync.PosY);
            Vector2 angle = Quaternion.AngleAxis(sync.Direction, Vector3.forward) * Vector3.down;
            snowball.GetComponent<Rigidbody2D>().velocity = angle * sync.Velocity;
        }
    }
}