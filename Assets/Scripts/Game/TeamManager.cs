using Assets.Utility;
using Networking;
using Networking.Data;
using UnityEngine;

namespace Game
{
    public class TeamManager : Singleton<TeamManager>
    {
        public string TeamName { get; private set; }

        protected TeamManager() { }

        public void SyncTeam(string teamName)
        {
            if (teamName == TeamName || string.IsNullOrEmpty(teamName)) return;
            TeamName = teamName;
            Debug.Log("Event: Synced Team, Team Name: " + teamName);
        }

        public void JoinTeam(string teamName)
        {
            JoinTeam joinTeam = new JoinTeam
            {
                UserID = ConnectionManager.Instance.UserID,
                TeamName = teamName,
            };
            Socket.Instance.SendPacket(joinTeam, Packets.JoinTeam);
        }

        public void JoinedTeam(string teamName)
        {
            TeamName = teamName;
            Debug.Log("Event: Joined Team, Team Name: " + teamName);
        }
    }
}