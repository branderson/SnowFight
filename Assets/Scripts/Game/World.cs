using System.Collections.Generic;
using Assets.Utility;
using Assets.Utility.Behaviours;
using Networking;
using UnityEngine;

namespace Game
{
    public class World : Singleton<World>
    {
        [SerializeField] private GameObject _playerPrefab;

        // Dictionary<userID, Player>
        private Dictionary<string, Player> _players;

        // Dictionary<objectID, GameObject>
        private Dictionary<int, GameObject> _worldObjects;

        protected World() { }

        public void Awake()
        {
            _players = new Dictionary<string, Player>();
            _worldObjects = new Dictionary<int, GameObject>();
        }

        /// <summary>
        /// Instantiate a player with the given ID and return a reference to it
        /// </summary>
        /// <param name="userID">
        /// ID to assign to instantiated player
        /// </param>
        /// <returns>
        /// Instantiated player
        /// </returns>
        public Player AddPlayer(string userID)
        {
            GameObject playerObject = Instantiate(_playerPrefab, this.transform);
            Player player;
            if (userID == ConnectionManager.Instance.UserID)
            {
                player = playerObject.AddComponent<MainPlayer>();
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Follow>().Target = playerObject.transform;
            }
            else
            {
                player = playerObject.AddComponent<Peer>();
            }
            player.Initialize(userID);

            _players[userID] = player;

            return player;
        }

        /// <summary>
        /// Remove the player with the given ID
        /// </summary>
        /// <param name="userID">
        /// ID of player to remove
        /// </param>
        public void RemovePlayer(string userID)
        {
            Player player = GetPlayer(userID);
            if (player == null) return;

            if (userID == ConnectionManager.Instance.UserID)
            {
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DampedFollow>().Follow = null;
            }

            _players.Remove(userID);
            Destroy(player.gameObject);
        }

        /// <summary>
        /// Get or create the player with the given ID
        /// </summary>
        /// <param name="userID">
        /// ID of player to get
        /// </param>
        /// <returns>
        /// Player with the given ID
        /// </returns>
        public Player GetPlayer(string userID)
        {
            Player player;
            if (_players.TryGetValue(userID, out player))
            {
                return player;
            }
            // Create player if it doesn't exist
            return AddPlayer(userID);
        }

        public MainPlayer GetMainPlayer()
        {
            Player player;
            if (_players.TryGetValue(ConnectionManager.Instance.UserID, out player))
            {
                return (MainPlayer)player;
            }
            return null;
        }

        /// <summary>
        /// Add the given GameObject to the world
        /// </summary>
        /// <param name="obj">
        /// GameObject to add to the world
        /// </param>
        /// <param name="objectID">
        /// ObjectID to assign to GameObject
        /// </param>
        public void AddObject(GameObject obj, int objectID)
        {
            _worldObjects[objectID] = obj;
        }

        /// <summary>
        /// Remove the GameObject with the given ID from the world and destroy it
        /// </summary>
        /// <param name="objectID">
        /// ID of object to destroy
        /// </param>
        public void DestroyObject(int objectID)
        {
            GameObject obj = GetObject(objectID);
            if (obj == null) return;
            _worldObjects.Remove(objectID);
            Destroy(obj);
        }

        /// <summary>
        /// Get the GameObject with the given ID
        /// </summary>
        /// <param name="objectID">
        /// ID of object to get
        /// </param>
        /// <returns>
        /// Object with given ID or null if not found
        /// </returns>
        public GameObject GetObject(int objectID)
        {
            GameObject obj;
            if (_worldObjects.TryGetValue(objectID, out obj))
            {
                return obj;
            }
            return null;
        }
    }
}