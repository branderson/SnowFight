using UnityEngine;

namespace UI
{
    public class HowToPlayController : MonoBehaviour
    {
        [SerializeField] private GameObject _pageOne;
        [SerializeField] private GameObject _pageTwo;

        public void Open()
        {
            gameObject.SetActive(true);
            _pageOne.SetActive(true);
        }

        public void Close()
        {
            PageOne();
            gameObject.SetActive(false);
        }

        public void PageOne()
        {
            _pageOne.SetActive(true);
            _pageTwo.SetActive(false);
        }

        public void PageTwo()
        {
            _pageTwo.SetActive(true);
            _pageOne.SetActive(false);
        }
    }
}