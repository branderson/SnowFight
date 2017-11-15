using System;
using System.Linq;
using Assets.Utility.Static;
using Networking;
using Networking.Data;
using UnityEngine;

namespace Game
{
    public class Player : MonoBehaviour
    {
        private static int InputBufferSize = 50;

        [SerializeField] private float _speed = 3f;

        private readonly PlayerUpdate[] _inputBuffer = new PlayerUpdate[InputBufferSize];
        private int _inputBufferBackingIndex = 0;

        private int _inputBufferIndex
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

        private void FixedUpdate()
        {
            if (!Socket.Instance.Connected) return;

            bool updated = false;

            Vector2 move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * _speed * Time.deltaTime;
            transform.AdjustPosition(move);

            if (move.sqrMagnitude > 0f) updated = true;

            if (updated)
            {
                UpdateServer(move, false);
            }
        }

        private void UpdateServer(Vector2 move, bool fired)
        {
            PlayerUpdate update = new PlayerUpdate
            {
                Time = DateTime.UtcNow,
                ClientID = Socket.Instance.ClientID,
                MoveX = move.x,
                MoveY = move.y,
                Fire = fired,
            };
            Socket.Instance.SendPacket(update, Packets.PlayerUpdate);
            _inputBuffer[_inputBufferIndex++] = update;
        }

        public void UpdateFromServer(PlayerSync sync)
        {
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
                Debug.Log("Latency: " + (_inputBuffer[earliestInputIndex].Time - sync.Time).Milliseconds + "ms");
                // Apply inputs
                int inputsBehind = 0;
                for (_inputBufferIndex = earliestInputIndex; _inputBuffer[_inputBufferIndex] != null; _inputBufferIndex++)
                {
                    inputsBehind++;
                    PlayerUpdate input = _inputBuffer[_inputBufferIndex];
                    transform.AdjustPosition(input.MoveX, input.MoveY);

                    // Prevent infinite loops
                    if (inputsBehind >= InputBufferSize) break;
                }
            }
        }
    }
}