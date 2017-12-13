using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LeaderboardUIController : MonoBehaviour
    {
        [SerializeField] private GameObject _playerLeaderboard;
        [SerializeField] private GameObject _teamLeaderboard;

        public void PlayerLeaderboard()
        {
            _playerLeaderboard.SetActive(true);
            _teamLeaderboard.SetActive(false);
        }

        public void TeamLeaderboard()
        {
            _teamLeaderboard.SetActive(true);
            _playerLeaderboard.SetActive(false);
        }
    }
}