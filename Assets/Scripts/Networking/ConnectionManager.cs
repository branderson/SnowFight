using Assets.Utility;
using Networking.Data;
using UI;
using UnityEngine;

namespace Networking
{
    public class ConnectionManager : Singleton<ConnectionManager>
    {
        public string UserID { get; private set; }

        protected ConnectionManager() { }

        public void Login(string userID)
        {
            Login login = new Login
            {
                ClientID = Socket.Instance.ClientID,
                UserID = userID,
            };
            Socket.Instance.SendPacket(login, Packets.Login);
        }

        public void LoggedIn(string userID)
        {
            UserID = userID;
            Debug.Log("Event: Connected, UserID: " + userID);
            UIManager.Instance.Close();
        }
    }
}