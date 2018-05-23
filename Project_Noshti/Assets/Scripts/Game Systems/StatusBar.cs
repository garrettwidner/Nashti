using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusBar : MonoBehaviour
{
    [SerializeField] private StatusLevel statusLevel;

    [SerializeField] private RectTransform bar;
    [SerializeField] private RectTransform fill;

    private float maxWidthWhenFull;
    private float containerStartWidth;
    private float barWidthToMaxStatusLevelRatio;

    private void Start()
    {
        containerStartWidth = bar.rect.width;
        barWidthToMaxStatusLevelRatio = containerStartWidth / statusLevel.MaxLevel;
    }

    private void Update()
    {
        maxWidthWhenFull = bar.rect.width;
        float currentStatRatio = statusLevel.CurrentLevel / statusLevel.MaxLevel;
        float statFillWidth = currentStatRatio * maxWidthWhenFull;
        fill.sizeDelta = new Vector2(statFillWidth, fill.sizeDelta.y);
    }

}
