using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngineInternal;

namespace UI
{
    public enum ProducerType
    {
        Field,
        Property,
        Method
    }

    public class SetTextFromProducer : MonoBehaviour
    {
        [SerializeField] private GameObject _producerObject;
        [SerializeField] private string _producerComponentType;
        [SerializeField] private ProducerType _producerType;
        [SerializeField] private string _producerName;
        [SerializeField] private float _updateRate;

        private Text _text;
        private Component _producerComponent;
        private FieldInfo _producerFieldInfo;
        private PropertyInfo _producerPropertyInfo;
        private MethodInfo _producerMethodInfo;
        private float _updateCountdown;

        private void Awake()
        {
            _text = GetComponent<Text>();
            if (_text == null)
            {
                Destroy(this);
                return;
            };
            _producerComponent = _producerObject.GetComponent(_producerComponentType);
            if (_producerComponent == null)
            {
                Destroy(this);
                return;
            };
            switch (_producerType)
            {
                case ProducerType.Field:
                    _producerFieldInfo = _producerComponent.GetType().GetField(_producerName);
                    break;
                case ProducerType.Property:
                    _producerPropertyInfo = _producerComponent.GetType().GetProperty(_producerName);
                    break;
                case ProducerType.Method:
                    _producerMethodInfo = _producerComponent.GetType().GetMethod(_producerName);
                    break;
            }
            if (_producerFieldInfo == null && _producerPropertyInfo == null && _producerMethodInfo == null)
            {
                Destroy(this);
                return;
            };
        }

        private void Update()
        {
            _updateCountdown -= Time.deltaTime;
            if (_updateCountdown <= 0)
            {
                string newText = "";
                object result = null;
                switch (_producerType)
                {
                    case ProducerType.Field:
//                        newText = _producerFieldInfo.GetValue(_producerComponent) as string;
                        result = _producerFieldInfo.GetValue(_producerComponent);
                        break;
                    case ProducerType.Property:
//                        newText = _producerPropertyInfo.GetValue(_producerComponent, new object[0]) as string;
                        result = _producerPropertyInfo.GetValue(_producerComponent, new object[0]);
                        break;
                    case ProducerType.Method:
//                        newText = _producerMethodInfo.Invoke(_producerComponent, new object[0]) as string;
                        result = _producerMethodInfo.Invoke(_producerComponent, new object[0]);
                        break;
                }
                if (result != null) newText = result.ToString();
                _text.text = newText;
                if (_updateRate > 0) _updateCountdown = 1f / _updateRate;
                else _updateCountdown = 0;
            }
        }
    }
}