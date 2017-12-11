using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Utility;
using Assets.Utility.Behaviours;
using Networking;
using Networking.Data;
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

        public int PlayerScore
        {
            get
            {
                MainPlayer player = GetMainPlayer();
                return player ? player.Score : 0;
            }
        }
        
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
            // TODO: Handle adding active player as main player
            if (GetPlayer(userID))
            {
                return null;
            }
            GameObject playerObject = Instantiate(_playerPrefab, this.transform);
            Player player;
            Debug.Log(userID);
            Debug.Log(ConnectionManager.Instance.UserID);
            if (userID == ConnectionManager.Instance.UserID)
            {
                player = playerObject.AddComponent<MainPlayer>();
                player.Peer = false;
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Follow>().Target = playerObject.transform;
            }
            else
            {
                player = playerObject.AddComponent<Peer>();
                player.Peer = true;
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
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Follow>().Target = null;
            }

            _players.Remove(userID);
            Destroy(player.gameObject);
        }

        public Player GetPlayer(string userID)
        {
            Player player;
            if (_players.TryGetValue(userID, out player))
            {
                return player;
            }
            return null;
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
        public Player GetOrAddPlayer(string userID, int fortressID=-1)
        {
            // TODO: Passing creation info into GetOrAddPlayer is bad
            Player player;
            if (_players.TryGetValue(userID, out player))
            {
                return player;
            }
            else
            {
                // Create player if it doesn't exist
                Fortress fortress = GetFortress(fortressID);
                return AddPlayer(userID, fortress);
            }
        }

        public MainPlayer GetMainPlayer()
        {
            Player player;
            string id = ConnectionManager.Instance.UserID;
            if (id == null) return null;
            if (_players.TryGetValue(id, out player))
            {
                return (MainPlayer)player;
            }
            return null;
        }

        public void SetSkin(string skin)
        {
            SetSkin set = new SetSkin
            {
                UserID = ConnectionManager.Instance.UserID,
                Skin = (Skins)Enum.Parse(typeof(Skins), skin),
            };
            Socket.Instance.SendPacket(set, Packets.SetSkin);
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