using Networking.Data;
using NUnit.Framework.Constraints;
using UnityEngine;

namespace Game
{
    public abstract class Player : MonoBehaviour
    {
        public string UserID;
        public bool Peer;
        public Fortress Fortress;
        public int Health = 3;
        public bool Carrying = false;
        public abstract string Team { get; }

        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _animator.speed = 1;
        }

        public void Initialize(string userID, Fortress fortress)
        {
            UserID = userID;
            Fortress = fortress;
            Despawn();
        }

        public void Spawn()
        {
            gameObject.SetActive(true);
        }

        public void Despawn()
        {
            gameObject.SetActive(false);
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

        public void Move(float velocity)
        {
            _animator.SetFloat("Velocity", velocity);
        }

        public void Carry(bool carrying)
        {
            Carrying = carrying;
            _animator.SetBool("Carrying", carrying);
        }

        public abstract void UpdateFromServer(PlayerSync sync);
    }
}