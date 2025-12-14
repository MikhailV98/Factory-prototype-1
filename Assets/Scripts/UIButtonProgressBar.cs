using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIButtonProgressBar : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] Image progressBarImage;
    [SerializeField] Color progressColor;
    [SerializeField] Color finalColor;
    float value = 0;
    public float Value 
    {
        get => value;
        set 
        { 
            this.value = Mathf.Clamp01(value);
            CheckIfButtonEnabled();
            UpdateProgressBar();
        }
    }
    void CheckIfButtonEnabled()
    {
        if (value == 1)
        {
            button.interactable = true;
            progressBarImage.color = finalColor;
        }
        else
        {
            button.interactable = false;
            progressBarImage.color = progressColor;
        }
    }
    void UpdateProgressBar() => progressBarImage.fillAmount = value;

    public UnityEvent onSuccessClick;
    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(TryClick);
        button.interactable = false;
        UpdateProgressBar();
    }

    void TryClick()
    {
        if (value == 1)
            onSuccessClick.Invoke();
    }


}
