using Networking.Data;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LeaderboardEntry : MonoBehaviour
    {
        [SerializeField] private Text _rankText;
        [SerializeField] private Text _nameText;
        [SerializeField] private Text _scoreText;

        public void Initialize(LeaderboardDataEntry data)
        {
            _rankText.text = data.Rank.ToString();
            _nameText.text = data.Name;
            _scoreText.text = data.Score.ToString();
        }
    }
}