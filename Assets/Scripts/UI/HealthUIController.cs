using Game;
using UnityEngine;

namespace UI
{
    public class HealthUIController : MonoBehaviour
    {
        [SerializeField] private RectTransform _heartsUI;
        [SerializeField] private GameObject _healthSpritePrefab;

        private int _hearts = 3;

        private void Start()
        {
            SetHealth(0);
        }

        private void Update()
        {
            Player player = World.Instance.GetMainPlayer();
            if (player == null)
            {
                SetHealth(0);
            }
            else
            {
                SetHealth(player.Health);
            }

        }

        public void SetHealth(int health)
        {
            if (health == _hearts) return;
            foreach (Transform child in _heartsUI)
            {
                Destroy(child.gameObject);
            }
            for (int i = 0; i < health; i++)
            {
                Instantiate(_healthSpritePrefab, _heartsUI);
            }
            _hearts = health;
        }
    }
}