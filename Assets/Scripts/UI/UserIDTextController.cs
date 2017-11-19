using System;
using System.Reflection;
using Networking;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
    public class UserIDTextController : MonoBehaviour
    {
        private Text _text;

        private void Awake()
        {
            _text = GetComponent<Text>();
            if (_text == null) Destroy(this);
        }

        private void Update()
        {
            _text.text = ConnectionManager.Instance.UserID;
        }
    }
}