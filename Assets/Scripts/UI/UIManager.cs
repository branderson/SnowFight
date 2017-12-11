using Assets.Utility;
using Networking;
using Networking.Data;
using UnityEngine;

namespace UI
{
    public class UIManager : Singleton<UIManager>
    {
        [SerializeField] private GameObject _howToPlayUI;
        [SerializeField] private GameObject _gameUI;
        [SerializeField] private GameObject _loginUI;
        [SerializeField] private GameObject _customizeUI;
        [SerializeField] private GameObject _leaderboardUI;
        [SerializeField] private LeaderboardController _teamLeaderboard;
        [SerializeField] private LeaderboardController _playerLeaderboard;

        protected UIManager() {}

        public void Start()
        {
            CloseHowToPlay();
            OpenLogin();
        }

        public void OpenHowToPlay()
        {
            _howToPlayUI.SetActive(true);
        }

        public void CloseHowToPlay()
        {
            _howToPlayUI.SetActive(false);
        }

        public void OpenLogin()
        {
            _loginUI.SetActive(true);
            _customizeUI.SetActive(false);
            _howToPlayUI.SetActive(false);
            _leaderboardUI.SetActive(false);
            _gameUI.SetActive(false);
        }

        public void OpenCustomize()
        {
            _customizeUI.SetActive(true);
            _loginUI.SetActive(false);
            _howToPlayUI.SetActive(false);
            _leaderboardUI.SetActive(false);
            _gameUI.SetActive(false);
        }

        public void OpenLeaderboards()
        {
            RequestLeaderboardUpdate();
            _customizeUI.SetActive(false);
            _loginUI.SetActive(false);
            _howToPlayUI.SetActive(false);
            _leaderboardUI.SetActive(true);
            _gameUI.SetActive(false);
        }

        public void Close()
        {
            _loginUI.SetActive(false);
            _customizeUI.SetActive(false);
            _howToPlayUI.SetActive(false);
            _leaderboardUI.SetActive(false);
            _gameUI.SetActive(true);
        }

        public void RequestLeaderboardUpdate()
        {
            _playerLeaderboard.Clear();
            _teamLeaderboard.Clear();
            RequestLeaderboardData request = new RequestLeaderboardData
            {
                UserID = ConnectionManager.Instance.UserID,
            };
            Socket.Instance.SendPacket(request, Packets.RequestLeaderboardData);
        }

        public void PopulateLeaderboards(LeaderboardDataEntry entry)
        {
            if (entry.Type == LeaderboardDataType.Player) _playerLeaderboard.AddEntry(entry);
            else _teamLeaderboard.AddEntry(entry);
        }

        public void RefreshLeaderboards()
        {
            _playerLeaderboard.Refresh();
            _teamLeaderboard.Refresh();
        }

        public void Quit()
        {
            Application.Quit();
        }
    }
}