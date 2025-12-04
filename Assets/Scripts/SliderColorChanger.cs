using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SliderColorChanger : MonoBehaviour
{
    //  Скрипт прикрепляется к слайдеру и меняет цвет в зависимости от значения
    Slider slider;
    Image sliderFillImage;
    public List<ColorPickerDictionary> colorPickerDictionaries;

    private void Start()
    {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(delegate { CheckSliderValue(); });
        sliderFillImage = slider.fillRect.GetComponent<Image>();
        colorPickerDictionaries = colorPickerDictionaries.OrderBy(o => o.value).ToList();
    }

    void CheckSliderValue()
    {
        foreach(ColorPickerDictionary colorDictionary in colorPickerDictionaries)
        {
            if (slider.value >= colorDictionary.value)
                ChangeColorOnSlider(colorDictionary.color);
        }
    }

    void ChangeColorOnSlider(Color color) => sliderFillImage.color = color;

    [Serializable]
    public struct ColorPickerDictionary
    {
        public float value;
        public Color color;
    }
}
