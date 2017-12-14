using Networking.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class Peer : Player
    {
        [SerializeField] private float _posX;
        [SerializeField] private float _posY;
        private string _team = "";
        private Text _nameText;

        public override string Team
        {
            get { return _team; }
        }

        public void Start()
        {
            _animatorControllers.NameUI.SetActive(true);
            _nameText = _animatorControllers.NameUI.GetComponentInChildren<Text>();
        }

        public void Update()
        {
            HitFlash();
        }

        public void SetPosition(Vector2 position)
        {
            _posX = position.x;
            _posY = position.y;
            transform.position = position;
        }

        public void Move(float x, float y)
        {
            _posX += x;
            _posY += y;
            SetPosition(new Vector2(_posX, _posY));
        }

        public override void UpdateFromServer(PlayerSync sync)
        {
            Score = sync.Score;
            SetSkin(sync.Skin);
            if (!sync.Active)
            {
                Despawn();
                return;
            }
            if (sync.WasHit) Hit();
            Health = sync.Health;
            Spawn();
            SetVelocity((transform.position - new Vector3(sync.PosX, sync.PosY)).sqrMagnitude / Time.deltaTime);
            transform.position = new Vector2(sync.PosX, sync.PosY);
            _team = sync.Team;
            _nameText.text = UserID;
            if (!string.IsNullOrEmpty(_team)) _nameText.text += "\n" + _team;
            Face(sync.Facing);
            Carry(sync.Carrying);
        }
    }
}