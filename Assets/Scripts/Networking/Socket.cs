using Assets.Utility;
using Networking.Data;
using UnityEngine;
using UnityEngine.Networking;

namespace Networking
{
    public class Socket : Singleton<Socket>
    {
        [SerializeField] private bool _useWebsocket = false;
        [SerializeField] private int _maxConnections = 1;
        [SerializeField] private int _socketPort = 30991;
        [SerializeField] private string _serverIP = "127.0.0.1";
        [SerializeField] private int _serverPort = 30993;
        [SerializeField] private int _serverPortWebsocket = 30992;
        [SerializeField] private int _maxPacketsPerFrame = 500;
        private int _bufferSize = 1024;
        private int _channelID;
        private int _socketID;
        public bool Connected { get; private set; }
        public int ClientID { get; private set; }

        protected Socket() { }

        private void Start ()
        {
            InitializeSocket();
            Time.fixedDeltaTime = 1f / 30f;
        }

        private void FixedUpdate ()
        {
            PollSocket();
        }

        private void OnApplicationQuit()
        {
            Disconnect();
            NetworkTransport.Shutdown();
        }

        private void InitializeSocket()
        {
            // Initialize socket
            NetworkTransport.Init();
            ConnectionConfig config = new ConnectionConfig();

            // Configure socket
            _channelID = config.AddChannel(QosType.Reliable);

            HostTopology topology = new HostTopology(config, _maxConnections);

            _socketID = NetworkTransport.AddHost(topology, _socketPort);

            #if UNITY_WEBGL
                _useWebsocket = true;
            #endif
        }

        private void PollSocket()
        {
            int packetLimit = _maxPacketsPerFrame;
            while (packetLimit-- >= 0)
            {
                // Poll for messages
                int recHostId;
                int recConnectionId;
                int recChannelId;
                byte[] recBuffer = new byte[_bufferSize];
                int bufferSize = _bufferSize;
                int dataSize;
                byte error;
                NetworkEventType recNetworkEvent = NetworkTransport.Receive(out recHostId, out recConnectionId, out recChannelId, recBuffer, bufferSize, out dataSize, out error);
                if (recNetworkEvent == NetworkEventType.Nothing) break;
                HandleNetworkEvent(recNetworkEvent, recHostId, recConnectionId, recChannelId, recBuffer, dataSize, error);
            }
        }

        private void HandleNetworkEvent(NetworkEventType networkEvent, int hostID, int connectionID, int channelID, byte[] buffer, int dataSize, byte error)
        {
            switch (networkEvent)
            {
                case NetworkEventType.Nothing:
                    break;
                case NetworkEventType.ConnectEvent:
                    Debug.Log("Event: Connected");
                    break;
                case NetworkEventType.DisconnectEvent:
                    Debug.Log("Event: Disconnected");
                    break;
                case NetworkEventType.DataEvent:
//                    Debug.Log("Event: Received Data");
                    MessageReader.ReadMessage(buffer);
                    break;
                case NetworkEventType.BroadcastEvent:
                    break;
            }
        }

        public void Connect()
        {
            byte error;
            if (Connected) return;
            if (_useWebsocket)
            {
                ClientID = NetworkTransport.Connect(_socketID, _serverIP, _serverPortWebsocket, 0, out error);
            }
            else
            {
                ClientID = NetworkTransport.Connect(_socketID, _serverIP, _serverPort, 0, out error);
            }
            if ((NetworkError)error == NetworkError.Ok)
            {
                Connected = true;
            }
        }

        public void Disconnect()
        {
            byte error;
            if (!Connected) return;
            NetworkTransport.Disconnect(_socketID, ClientID, out error);
            if ((NetworkError)error == NetworkError.Ok)
            {
                Connected = false;
            }
        }

        public void SendPacket<T>(T packet, Packets packetType) where T : class
        {
            Envelope envelope = new Envelope
            {
                PacketType = packetType,
                Packet = SerializationHandler.Serialize(packet, _bufferSize - 512)
            };
            byte error;
            byte[] message = SerializationHandler.Serialize(envelope, _bufferSize);
            NetworkTransport.Send(_socketID, ClientID, _channelID, message, _bufferSize, out error);

//            MessageReader.ReadMessage(message);
        }

        public void SendJSONMessage(PlayerUpdate message)
        {
            Debug.Log(string.Format("Event: Send message, Message: {0}", message));

            SendPacket(message, Packets.PlayerUpdate);
        }

        public void SendSocketMessage()
        {
            PlayerUpdate update = new PlayerUpdate
            {
            };
            SendJSONMessage(update);
        }
    }
}
