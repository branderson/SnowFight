using Networking;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class Fortress : MonoBehaviour
    {
        private const int LevelTwoCutoff = 20;
        private const int LevelThreeCutoff = 50;

        public int ID;
        public string OwnerID;
        [SerializeField] private Sprite _levelOneSprite;
        [SerializeField] private Sprite _levelTwoSprite;
        [SerializeField] private Sprite _levelThreeSprite;
        private SpriteRenderer _mainRenderer;
        [SerializeField] private SpriteRenderer _minimapRenderer;
        [SerializeField] private Text _ownerText;

        private void Awake()
        {
            _mainRenderer = GetComponent<SpriteRenderer>();
            SetVisible(false);
        }

        public void Initialize(string owner, string team)
        {
            OwnerID = owner;
            _ownerText.text = owner;
            if (!string.IsNullOrEmpty(team)) _ownerText.text += "\n" + team;
        }

        public void SetVisible(bool visible)
        {
            _mainRenderer.enabled = visible;
            _ownerText.enabled = visible;
            if (OwnerID == ConnectionManager.Instance.UserID) _minimapRenderer.enabled = visible;
        }

        public void SetToScore(int score)
        {
            if (score < LevelTwoCutoff) SetLevel(1);
            else if (score < LevelThreeCutoff) SetLevel(2);
            else SetLevel(3);
        }

        private void SetLevel(int level)
        {
            switch (level)
            {
                case 1:
                    _mainRenderer.sprite = _levelOneSprite;
                    break;
                case 2:
                    _mainRenderer.sprite = _levelTwoSprite;
                    break;
                case 3:
                    _mainRenderer.sprite = _levelThreeSprite;
                    break;
            }
        }
    }
}