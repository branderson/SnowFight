using Game;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ScoreUIController : MonoBehaviour
    {
        [SerializeField] private Image _fortressUI;
        [SerializeField] private RectTransform _starUI;
        [SerializeField] private Sprite _levelOneSprite;
        [SerializeField] private Sprite _levelTwoSprite;
        [SerializeField] private Sprite _levelThreeSprite;
        [SerializeField] private GameObject _starPrefab;
        [SerializeField] private Text _scoreText;

        private int _stars = 5;

        private void Start()
        {
            SetStars(0);
        }

        private void Update()
        {
            if (World.Instance.GetMainPlayer() == null)
            {
                _scoreText.text = "";
                _fortressUI.sprite = null;
                SetStars(0);
            }
            else
            {
                SetScore(World.Instance.PlayerScore);
            }
        }

        public void SetScore(int score)
        {
            _scoreText.text = score.ToString();
            if (score < Fortress.LevelTwoCutoff)
            {
                _fortressUI.sprite = _levelOneSprite;
            }
            else if (score < Fortress.LevelThreeCutoff)
            {
                _fortressUI.sprite = _levelTwoSprite;
            }
            else
            {
                _fortressUI.sprite = _levelThreeSprite;
            }
            if (score < Fortress.LevelFourCutoff)
            {
                SetStars(0);
            }
            else if (score < Fortress.LevelFiveCutoff)
            {
                SetStars(1);
            }
            else if (score < Fortress.LevelSixCutoff)
            {
                SetStars(2);
            }
            else if (score < Fortress.LevelSevenCutoff)
            {
                SetStars(3);
            }
            else if (score < Fortress.LevelEightCutoff)
            {
                SetStars(4);
            }
            else
            {
                SetStars(5);
            }
        }

        private void SetStars(int count)
        {
            if (count == _stars) return;
            foreach (Transform child in _starUI)
            {
                Destroy(child.gameObject);
            }
            for (int i = 0; i < count; i++)
            {
                Instantiate(_starPrefab, _starUI);
            }
            _stars = count;
        }
    }
}