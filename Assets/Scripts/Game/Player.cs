using Networking.Data;
using UnityEngine;

namespace Game
{
    public abstract class Player : MonoBehaviour
    {
        [SerializeField] private GameObject _mainPlayerMinimap;
        [SerializeField] private GameObject _enemyMinimap;
        public string UserID;
        public int Score = 0;
        public Skins Skin = Skins.WhiteBlue;
        public bool Peer;
        public Fortress Fortress;
        public int Health = 3;
        public bool Carrying = false;
        public abstract string Team { get; }

        private PlayerAnimatorControllers _animatorControllers;
        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _animatorControllers = GetComponent<PlayerAnimatorControllers>();
            _animator.speed = 1;
        }

        public void Initialize(string userID, Fortress fortress)
        {
            UserID = userID;
            Fortress = fortress;
            _mainPlayerMinimap = transform.Find("MainPlayerMinimap").gameObject;
            _enemyMinimap = transform.Find("EnemyMinimap").gameObject;
            Despawn();
        }

        public void Spawn()
        {
            gameObject.SetActive(true);
            if (Peer)
            {
                _mainPlayerMinimap.SetActive(false);
                _enemyMinimap.SetActive(true);
            }
            else
            {
                _mainPlayerMinimap.SetActive(true);
                _enemyMinimap.SetActive(false);
            }
        }

        public void Despawn()
        {
            gameObject.SetActive(false);
            _mainPlayerMinimap.SetActive(false);
            _enemyMinimap.SetActive(false);
        }

        public void Face(float direction)
        {
            if (direction >= -45f && direction < 45f)
            {
                _animator.SetInteger("Facing", 3);
            }
            else if (direction >= 45f && direction < 135f)
            {
                _animator.SetInteger("Facing", 0);
            }
            else if (direction >= 135f || direction < -135f)
            {
                _animator.SetInteger("Facing", 1);
            }
            else
            {
                _animator.SetInteger("Facing", 2);
            }
        }

        public void SetVelocity(float velocity)
        {
            _animator.SetFloat("Velocity", velocity);
        }

        public void Carry(bool carrying)
        {
            Carrying = carrying;
            _animator.SetBool("Carrying", carrying);
        }

        public void SetSkin(Skins skin)
        {
            if (skin == Skin) return;
            // Reset scale so we don't always run right
            GetComponentInChildren<SpriteRenderer>().transform.localScale = Vector3.one;
            switch (skin)
            {
                case Skins.WhiteBlue:
                    _animator.runtimeAnimatorController = _animatorControllers.WhiteBlue;
                    break;
                case Skins.WhiteOrange:
                    _animator.runtimeAnimatorController = _animatorControllers.WhiteOrange;
                    break;
                case Skins.WhitePink:
                    _animator.runtimeAnimatorController = _animatorControllers.WhitePink;
                    break;
                case Skins.BlackBlue:
                    _animator.runtimeAnimatorController = _animatorControllers.BlackBlue;
                    break;
                case Skins.BlackOrange:
                    _animator.runtimeAnimatorController = _animatorControllers.BrownOrange;
                    break;
                case Skins.BlackPink:
                    _animator.runtimeAnimatorController = _animatorControllers.BlackPink;
                    break;
                case Skins.BrownBlue:
                    _animator.runtimeAnimatorController = _animatorControllers.BrownBlue;
                    break;
                case Skins.BrownOrange:
                    _animator.runtimeAnimatorController = _animatorControllers.BrownOrange;
                    break;
                case Skins.BrownPink:
                    _animator.runtimeAnimatorController = _animatorControllers.BrownPink;
                    break;
            }
            Skin = skin;
        }

        public abstract void UpdateFromServer(PlayerSync sync);
    }
}