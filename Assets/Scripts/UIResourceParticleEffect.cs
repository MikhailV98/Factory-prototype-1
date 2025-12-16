using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIResourceParticleEffect : MonoBehaviour
{

    [SerializeField] float resourceEffectTime = 0.5f;
    [SerializeField] float resourceEffectPeriod = 0.1f;
    [SerializeField] float resourceEffectMaxHeight = 100f;

    public void SpawnEffect(Sprite sprite)
    {
        StartCoroutine(SpawnResourceEffect(sprite));
    }

    IEnumerator SpawnResourceEffect(Sprite sprite)
    {
        GameObject effectGO = CreateEffectSprite(sprite);
        RectTransform effectTransform = effectGO.transform as RectTransform;
        int iterationsCount = Mathf.CeilToInt(resourceEffectTime / resourceEffectPeriod);
        float positionModify = resourceEffectMaxHeight / (float)iterationsCount;
        for (int i = 0; i <= iterationsCount; i++)
        {
            yield return new WaitForSeconds(resourceEffectPeriod);
            effectTransform.anchoredPosition += Vector2.up * positionModify;
        }
        Destroy(effectGO);
    }
    GameObject CreateEffectSprite(Sprite sprite)
    {
        GameObject effectGO = new GameObject("resource particle");
        effectGO.transform.SetParent((RectTransform)transform);
        effectGO.AddComponent<CanvasRenderer>();
        effectGO.AddComponent<Image>().sprite = sprite;
        RectTransform effectTransform = effectGO.transform as RectTransform;
        effectTransform.localPosition = new Vector3(0, ((RectTransform)effectTransform.parent).rect.height / 2);
        effectTransform.localScale = Vector3.one * 2;
        effectTransform.localRotation = Quaternion.Euler(Vector3.zero);

        return effectGO;
    }
}
