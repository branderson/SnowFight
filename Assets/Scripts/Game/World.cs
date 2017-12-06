using System.Collections.Generic;
using System.Linq;
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

        private Dictionary<Fortress, Player> _fortresses;

        protected World() { }

        public void Awake()
        {
            _players = new Dictionary<string, Player>();
            _worldObjects = new Dictionary<int, GameObject>();
            _fortresses = new Dictionary<Fortress, Player>();
            LoadFortresses();
        }

        /// <summary>
        /// Instantiate a player with the given ID and return a reference to it
        /// </summary>
        /// <param name="userID">
        /// ID to assign to instantiated player
        /// </param>
        /// <param name="fortress">
        /// Fortress to assign to player
        /// </param>
        /// <returns>
        /// Instantiated player
        /// </returns>
        public Player AddPlayer(string userID, Fortress fortress)
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
            player.Initialize(userID, fortress);

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
        /// <param name="fortressID">
        /// ID of fortress to assign to created player
        /// </param>
        /// <returns>
        /// Player with the given ID
        /// </returns>
        public Player GetPlayer(string userID, int fortressID=-1)
        {
            // TODO: Passing creation info into GetPlayer is bad
            Player player;
            if (_players.TryGetValue(userID, out player))
            {
                return player;
            }
            // Create player if it doesn't exist
            Fortress fortress = GetFortress(fortressID);
            return AddPlayer(userID, fortress);
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
        /// Load fortresses into the world
        /// </summary>
        private void LoadFortresses()
        {
            foreach (Fortress fortress in FindObjectsOfType<Fortress>())
            {
                _fortresses[fortress] = null;
            }
        }

        /// <summary>
        /// Get the Fortress with the given ID
        /// </summary>
        /// <param name="fortressID">
        /// ID of Fortress to get
        /// </param>
        /// <returns>
        /// Fortress with the given ID
        /// </returns>
        public Fortress GetFortress(int fortressID)
        {
            Fortress fortress = _fortresses.Keys.FirstOrDefault(item => item.ID == fortressID);
            return fortress;
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