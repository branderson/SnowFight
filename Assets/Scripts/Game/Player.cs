using Networking.Data;
using UnityEngine;

namespace Game
{
    public abstract class Player : MonoBehaviour
    {
        [SerializeField] public string UserID { get; private set; }
        public abstract string Team { get; }

        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _animator.speed = 1;
        }

        public void Initialize(string userID)
        {
            UserID = userID;
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

        public abstract void UpdateFromServer(PlayerSync sync);
    }
}