using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIImageProgressBar : MonoBehaviour
{
    [SerializeField] Image fillImage;
    float progressionValue = 0;
    public float ProgressionValue
    {
        get => progressionValue;
        set
        {
            progressionValue = Mathf.Clamp(value, 0, 1);
            ChangeProgression(progressionValue);
        }
    }

    private void ChangeProgression(float value)
        => fillImage.fillAmount = progressionValue;
}
