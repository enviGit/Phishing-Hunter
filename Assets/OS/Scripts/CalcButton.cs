using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace com.fanboatstudios.unitycalculator
{
    public class CalcButton : MonoBehaviour
    {
        [SerializeField] Text label;

        private Manager calcManager;

        private void Awake()
        {
            calcManager = GetComponentInParent<Manager>();
        }

        public void OnTapped()
        {
            Debug.Log($"Tapped: {label.text}");
            if (label.text == "OFF")
            {
                Manager.PowerOff();
            }
            else
            {
                calcManager.ButtonTapped(label.text[0]);
            }
        }
    }
}