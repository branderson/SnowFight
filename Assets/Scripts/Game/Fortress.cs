using UnityEngine;

namespace Game
{
    public class Fortress : MonoBehaviour
    {
        public int ID;
        public string OwnerID;
        [SerializeField] private Sprite _levelOneSprite;
        [SerializeField] private Sprite _levelTwoSprite;
        [SerializeField] private Sprite _levelThreeSprite;
        private SpriteRenderer _renderer;

        private void Awake()
        {
            _renderer = GetComponentInChildren<SpriteRenderer>();
        }

        private void SetLevel(int level)
        {
            switch (level)
            {
                case 1:
                    _renderer.sprite = _levelOneSprite;
                    break;
                case 2:
                    _renderer.sprite = _levelTwoSprite;
                    break;
                case 3:
                    _renderer.sprite = _levelThreeSprite;
                    break;
            }
        }
    }
}