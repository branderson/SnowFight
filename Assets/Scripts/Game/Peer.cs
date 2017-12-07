﻿using Networking.Data;
using UnityEngine;

namespace Game
{
    public class Peer : Player
    {
        [SerializeField] private float _posX;
        [SerializeField] private float _posY;
        private string _team = "";

        public override string Team
        {
            get { return _team; }
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
            if (!sync.Active)
            {
                Despawn();
                return;
            }
            Spawn();
            Move((transform.position - new Vector3(sync.PosX, sync.PosY)).sqrMagnitude / Time.deltaTime);
            transform.position = new Vector2(sync.PosX, sync.PosY);
            _team = sync.Team;
            Face(sync.Facing);
            Carry(sync.Carrying);
        }
    }
}