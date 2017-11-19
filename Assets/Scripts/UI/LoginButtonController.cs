using Networking;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LoginButtonController : MonoBehaviour
    {
        [SerializeField] private InputField _inputField;

        public void Login()
        {
            if (_inputField.text == "") return;
            ConnectionManager.Instance.Login(_inputField.text);
        }
    }
}