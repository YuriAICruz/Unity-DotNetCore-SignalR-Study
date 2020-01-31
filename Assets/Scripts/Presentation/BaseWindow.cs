using System;
using UnityEngine;
using UnityEngine.UI;

namespace Presentation
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class BaseWindow : MonoBehaviour
    {
        private CanvasGroup _cv;
        

        public Button submit;

        protected virtual void Awake()
        {
            _cv = GetComponent<CanvasGroup>();
            
            submit.onClick.AddListener(OnSubmit);
        }

        protected void EnableSubmitButton(bool success)
        {
            submit.interactable = true;
        }

        protected abstract void OnSubmit();

        public void Show()
        {
            _cv.alpha = 1;
            _cv.blocksRaycasts = true;
        }

        public void Hide()
        {
            _cv.alpha = 0;
            _cv.blocksRaycasts = false;
        }
    }
}