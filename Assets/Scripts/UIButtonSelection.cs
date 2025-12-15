using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIButtonSelection : MonoBehaviour
{
    [SerializeField] GameObject selectionOutputObject;
    [SerializeField] TextMeshProUGUI buttonText;

    public void IndicationON() => selectionOutputObject.SetActive(true);
    public void IndicationOFF() => selectionOutputObject.SetActive(false);

    public void SetText(string value) => buttonText.text = value;
}
