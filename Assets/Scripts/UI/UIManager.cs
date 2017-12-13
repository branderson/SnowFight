using Assets.Utility;
using Networking;
using Networking.Data;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIManager : Singleton<UIManager>
    {
        [SerializeField] private HowToPlayController _howToPlayUI;
        [SerializeField] private GameObject _gameUI;
        [SerializeField] private GameObject _loginUI;
        [SerializeField] private GameObject _customizeUI;
        [SerializeField] private GameObject _leaderboardUI;
        [SerializeField] private GameObject _quitConfirmation;
        [SerializeField] private LeaderboardController _teamLeaderboard;
        [SerializeField] private LeaderboardController _playerLeaderboard;
        [SerializeField] private Text _warningText;

        private int _warningCountdown = 0;

        protected UIManager() {}

        public void Start()
        {
            CloseQuitConfirmation();
            CloseHowToPlay();
            OpenLogin();
        }

        public void Update()
        {
            if (_warningCountdown < 0) return;
            if (_warningCountdown == 0) SetWarningText("");
            else _warningCountdown--;
        }

        public void SetWarningText(string text, int lifespan=300)
        {
            _warningText.text = text;
            _warningCountdown = lifespan;
        }

        public void OpenHowToPlay()
        {
            _howToPlayUI.Open();
        }

        public void CloseHowToPlay()
        {
            _howToPlayUI.Close();
        }

        public void OpenQuitConfirmation()
        {
            _quitConfirmation.SetActive(true);
        }

        public void CloseQuitConfirmation()
        {
            _quitConfirmation.SetActive(false);
        }

        public void OpenLogin()
        {
            _loginUI.SetActive(true);
            _customizeUI.SetActive(false);
            _leaderboardUI.SetActive(false);
            _gameUI.SetActive(false);
        }

        public void OpenCustomize()
        {
            _customizeUI.SetActive(true);
            _loginUI.SetActive(false);
            _leaderboardUI.SetActive(false);
            _gameUI.SetActive(false);
        }

        public void OpenLeaderboards()
        {
            RequestLeaderboardUpdate();
            _customizeUI.SetActive(false);
            _loginUI.SetActive(false);
            _leaderboardUI.SetActive(true);
            _gameUI.SetActive(false);
        }

        public void Close()
        {
            _loginUI.SetActive(false);
            _customizeUI.SetActive(false);
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