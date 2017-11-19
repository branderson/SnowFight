using Game;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class JoinTeamButtonController : MonoBehaviour
    {
        [SerializeField] private InputField _inputField;

        public void JoinTeam()
        {
            if (_inputField.text == "") return;
            TeamManager.Instance.JoinTeam(_inputField.text);
        }
    }
}