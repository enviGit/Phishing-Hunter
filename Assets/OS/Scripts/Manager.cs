using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.fanboatstudios.unitycalculator
{
    public class Manager : MonoBehaviour
    {
        [SerializeField] Text digitLabel;
        [SerializeField] Text operatorLabel;
        bool errorDisplayed;
        bool displayValid;
        bool specialAction;
        double currentValue;
        double storedValue;
        double result;
        char storedOperator;

        private void Start()
        {
            ButtonTapped('c');
        }

        public static void PowerOff()
        {
            if (!Application.isEditor)
            {
                Application.Quit();
            }
        }

        public void ButtonTapped(char value)
        {
            if (errorDisplayed)
            {
                ClearCalc();
            }

            if((value >= '0' && value <= '9') || value == '.')
            {
                if(digitLabel.text.Length < 15 || !displayValid)
                {
                    if (!displayValid)
                    {
                        digitLabel.text = (value == '.' ? "0" : "");
                    }
                    else if (digitLabel.text == "0" && value != '.')
                    {
                        digitLabel.text = "";
                    }

                    digitLabel.text += value;
                    displayValid = true;
                }
            }
            else if(value == 'c')
            {
                ClearCalc();
            }
            else if(value == '±')
            {
                currentValue = -double.Parse(digitLabel.text);
                UpdateDigitLabel();
                specialAction = true;
            }
            else if(value == '%')
            {
                currentValue = double.Parse(digitLabel.text) / 100.0d;
                UpdateDigitLabel();
                specialAction = true;
            }
            else if(displayValid || storedOperator == '=' || specialAction)
            {
                currentValue = double.Parse(digitLabel.text);
                displayValid = false;
                if (storedOperator != ' ')
                {
                    CalculateResult(storedOperator);
                    storedOperator = ' ';
                }

                operatorLabel.text = value.ToString();
                storedOperator = value;
                storedValue = currentValue;
                UpdateDigitLabel();
                specialAction = false;
            }
        }

        void ClearCalc()
        {
            digitLabel.text = "0";
            operatorLabel.text = "";
            specialAction = displayValid = errorDisplayed = false;
            currentValue = result = storedValue = 0;
            storedOperator = ' ';
        }

        void UpdateDigitLabel()
        {
            if (!errorDisplayed)
            {
                digitLabel.text = currentValue.ToString();
            }
            displayValid = false;
        }

        void CalculateResult(char activeOperator)
        {
            switch (activeOperator)
            {
                case '=':
                    result = currentValue;
                    break;
                case '+':
                    result = storedValue + currentValue;
                    break;
                case '-':
                    result = storedValue - currentValue;
                    break;
                case 'x':
                    result = storedValue * currentValue;
                    break;
                case '÷':
                    if(currentValue != 0)
                    {
                        result = storedValue / currentValue;
                    }
                    else
                    {
                        errorDisplayed = true;
                        digitLabel.text = "ERROR";
                    }
                    break;
                default:
                    Debug.LogWarning($"unkown operator: {activeOperator}");
                    break;
            }

            currentValue = result;
            UpdateDigitLabel();
        }

    }
}

//