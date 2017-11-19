using System;
using Assets.Utility.Static;
using Networking;
using Networking.Data;
using UnityEngine;

namespace Game
{
    public class MainPlayer : Player
    {
        private const int InputBufferSize = 50;

        [SerializeField] private float _speed = 5f;
        [SerializeField] private float _fireRate = 3f;

        private float _facing = 0;
        private float _fireCountdown = 0;

        private readonly PlayerUpdate[] _inputBuffer = new PlayerUpdate[InputBufferSize];
        private int _inputBufferBackingIndex = 0;

        private int InputBufferIndex
        {
            get
            {
                return _inputBufferBackingIndex;
            }
            set
            {
                _inputBufferBackingIndex = value;
                if (_inputBufferBackingIndex >= InputBufferSize)
                {
                    _inputBufferBackingIndex = _inputBufferBackingIndex % InputBufferSize;
                }
            }
        }

        public override string Team
        {
            get { return TeamManager.Instance.TeamName; }
        }

        private void Update()
        {
            if (!Socket.Instance.Connected) return;

            // Fire rate limiting
            _fireCountdown -= Time.deltaTime;
            if (_fireCountdown < 0) _fireCountdown = 0;

            bool updated = false;

            Vector2 move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * _speed * Time.deltaTime;
            Move(move);

            if (move.sqrMagnitude > 0f) updated = true;

            if (updated)
            {
//                Vector2 mousePosition = Camera.main.ViewportToWorldPoint(Input.mousePosition);
//                float newFacing = Vector2.SignedAngle(transform.position, mousePosition);
                float newFacing = Vector2.SignedAngle(Vector2.down, move);

                _facing = newFacing;
                Face(_facing);

                UpdateServer(move, false);
            }
            if (Input.GetButtonDown("Fire"))
            {
                if (Mathf.Approximately(_fireCountdown, 0))
                {
                    FireSnowball();
                    _fireCountdown = 1f / _fireRate;
                }
            }
        }

        private void Move(Vector2 move)
        {
            if (move.sqrMagnitude > 0f)
            {
                transform.AdjustPosition(move);
            }
        }

        private void FireSnowball()
        {
            SpawnSnowball snowball = new SpawnSnowball
            {
                UserID = UserID,
                PosX = transform.position.x,
                PosY = transform.position.y,
                Direction = _facing,
            };
            Socket.Instance.SendPacket(snowball, Packets.SpawnSnowball);
        }

        private void UpdateServer(Vector2 move, bool fired)
        {
            PlayerUpdate update = new PlayerUpdate
            {
                Time = DateTime.UtcNow,
                UserID = ConnectionManager.Instance.UserID,
                MoveX = move.x,
                MoveY = move.y,
                Fire = fired,
                Facing = _facing,
            };
            Socket.Instance.SendPacket(update, Packets.PlayerUpdate);
            _inputBuffer[InputBufferIndex++] = update;
        }

        public override void UpdateFromServer(PlayerSync sync)
        {
            // Sync team name
            TeamManager.Instance.SyncTeam(sync.Team);

            // Apply sync
            transform.position = new Vector2(sync.PosX, sync.PosY);
            // Find earliest input after sync
            int earliestInputIndex = 0;
            bool found = false;
            for (int i = 0; i < InputBufferSize; i++)
            {
                PlayerUpdate input = _inputBuffer[i];
                if (input == null) continue;
                // Remove inputs older than sync
                if (sync.Time > input.Time)
                {
                    _inputBuffer[i] = null;
                    continue;
                }
                if (found && input.Time > _inputBuffer[earliestInputIndex].Time) continue;
                earliestInputIndex = i;
                found = true;
            }
            if (found)
            {
//                Debug.Log("Latency: " + (_inputBuffer[earliestInputIndex].Time - sync.Time).Milliseconds + "ms");
                // Apply inputs
                int inputsBehind = 0;
                for (InputBufferIndex = earliestInputIndex; _inputBuffer[InputBufferIndex] != null; InputBufferIndex++)
                {
                    inputsBehind++;
                    PlayerUpdate input = _inputBuffer[InputBufferIndex];
                    Move(new Vector2(input.MoveX, input.MoveY));

                    // Prevent infinite loops
                    if (inputsBehind >= InputBufferSize) break;
                }
            }
        }

        private float GetAngle(Vector2 facing)
        {
            return Vector2.SignedAngle(Vector2.down, facing);
        }
    }
}