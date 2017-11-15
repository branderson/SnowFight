using System;
using UnityEngine;

namespace Networking.Data
{
    public class PlayerUpdate
    {
        public DateTime Time = DateTime.UtcNow;
        public int ClientID = 0;
        public float MoveX = 0f;
        public float MoveY = 0f;
        public bool Fire = false;
    }
}