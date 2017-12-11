using System.Collections.Generic;
using System.Linq;
using Networking.Data;
using UnityEngine;

namespace UI
{
    public class LeaderboardController : MonoBehaviour
    {
        [SerializeField] private RectTransform _content;
        [SerializeField] private GameObject _leaderboardEntryPrefab;
        private List<LeaderboardDataEntry> _data = new List<LeaderboardDataEntry>();

        public void Clear()
        {
            _data = new List<LeaderboardDataEntry>();
            foreach (RectTransform t in _content)
            {
                Destroy(t.gameObject);
            }
        }

        public void AddEntry(LeaderboardDataEntry entry)
        {
            _data.Add(entry);
        }

        public void Refresh()
        {
            foreach (LeaderboardDataEntry entry in _data.OrderByDescending(item => item.Score))
            {
                GameObject obj = Instantiate(_leaderboardEntryPrefab);
                LeaderboardEntry e = obj.GetComponent<LeaderboardEntry>();
                e.Initialize(entry);
                e.transform.SetParent(_content);
            }
        }
    }
}