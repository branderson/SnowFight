using System;
using Game;
using Networking.Data;
using UI;
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
                case Packets.SnowPileSync:
                    SnowPileSync snowPileSync = SerializationHandler.Deserialize<SnowPileSync>(envelope.Packet);
                    HandleSnowPileSync(snowPileSync);
                    break;
                case Packets.LeaderboardData:
                    LeaderboardDataEntry leaderboardDataResponse = SerializationHandler.Deserialize<LeaderboardDataEntry>(envelope.Packet);
                    HandleLeaderboardDataEntry(leaderboardDataResponse);
                    break;
                case Packets.EndLeaderboardResponse:
                    HandleEndLeaderboardResponse();
                    break;
                case Packets.AckConnection:
                    HandleAckConnection();
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
                UIManager.Instance.SetWarningText(string.Format("User {0} already logged in", ack.UserID));
                return;
            }
            ConnectionManager.Instance.LoggedIn(ack.UserID);
            if (ack.FirstLogin)
            {
                UIManager.Instance.OpenCustomize();
                UIManager.Instance.OpenHowToPlay();
            }
            UIManager.Instance.SetWarningText("", -1);
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
            // Remove inactive players
            Fortress fortress = World.Instance.GetFortress(sync.FortressID);
            fortress.SetVisible(true);
            fortress.Initialize(sync.UserID, sync.Team);
            fortress.SetToScore(sync.Score);
            if (!sync.Active)
            {
                World.Instance.RemovePlayer(sync.UserID);
                return;
            }
            Player player = World.Instance.GetOrAddPlayer(sync.UserID, sync.FortressID);
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

        private static void HandleSnowPileSync(SnowPileSync sync)
        {
            if (sync == null) throw new WrongPacketTypeException();
            Debug.Log("Received snow pile sync");
            GameObject snowPile = GameObject.Instantiate(Prefabs.Instance.SnowPilePrefab);
            World.Instance.AddObject(snowPile, sync.ObjectID);
            snowPile.transform.position = new Vector2(sync.PosX, sync.PosY);
        }

        private static void HandleLeaderboardDataEntry(LeaderboardDataEntry response)
        {
            UIManager.Instance.PopulateLeaderboards(response);
        }

        private static void HandleEndLeaderboardResponse()
        {
            UIManager.Instance.RefreshLeaderboards();
        }

        private static void HandleAckConnection()
        {
            Socket.Instance.VerifyConnection();
        }
    }
}