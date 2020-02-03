using System;
using UnityEngine;
using UnityEngine.UI;

namespace Presentation
{
    [RequireComponent(typeof(Button))]
    public abstract class ButtonView : MonoBehaviour
    {
        protected Button _button;

        protected virtual void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClick);
        }

        protected abstract void OnClick();
    }
}