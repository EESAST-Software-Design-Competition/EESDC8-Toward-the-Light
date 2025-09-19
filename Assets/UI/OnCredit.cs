using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnCredit : MonoBehaviour
{
        public GameObject openingCanvas;
        public GameObject creditCanvas;

        private void Start()
        {
            openingCanvas.SetActive(true);
            creditCanvas.SetActive(false);
        }

        public void OnSettingsClicked()
        {
            openingCanvas.SetActive(false);
            creditCanvas.SetActive(true);
        }

        public void OnReturnClicked()
        {
            openingCanvas.SetActive(true);
            creditCanvas.SetActive(false);
        }
}
