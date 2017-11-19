using Networking.Data;
using UnityEngine;

namespace Game
{
    public abstract class Player : MonoBehaviour
    {
        [SerializeField] public string UserID { get; private set; }
        public abstract string Team { get; }

        public void Initialize(string userID)
        {
            UserID = userID;
        }

        public void Face(float direction)
        {
            
        }

        public abstract void UpdateFromServer(PlayerSync sync);
    }
}