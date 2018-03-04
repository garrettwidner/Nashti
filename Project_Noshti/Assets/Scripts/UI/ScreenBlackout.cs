using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ScreenBlackout : MonoBehaviour
{
    public Image blackoutImage;
    public float blackoutSpeed;
    public Color colorFinal;
    public Color colorBegin;

    private Color currentColor;
    private bool isBlackingOut = false;
    private bool isFadingIn = false;

    public delegate void BlackoutAction();
    public event BlackoutAction OnScreenCompletelyBlack;
    public event BlackoutAction OnScreenCompletelyVisible;

    private float fullAlphaApproximation = 0.991f;
    private float emptyAlphaApproximation = 0.009f;

    private float exponentializer = 0f;

    private void Update()
    {
        UpdateBlackout();
    }

    private void UpdateBlackout()
    {
        if (isBlackingOut && blackoutImage != null)
        {
            exponentializer += Time.deltaTime * 2;
            blackoutImage.color = Color.Lerp(blackoutImage.color, currentColor, blackoutSpeed * Time.deltaTime * exponentializer);

            if (blackoutImage.color.a >= fullAlphaApproximation)
            {
                isBlackingOut = false;
                blackoutImage.color = colorFinal;
                if (OnScreenCompletelyBlack != null)
                {
                    OnScreenCompletelyBlack();
                }
            }
        }
        else if (isFadingIn && blackoutImage != null)
        {
            exponentializer += Time.deltaTime * 2;
            blackoutImage.color = Color.Lerp(blackoutImage.color, currentColor, blackoutSpeed * Time.deltaTime * exponentializer);
            if (blackoutImage.color.a <= emptyAlphaApproximation)
            {
                isFadingIn = false;
                blackoutImage.color = colorBegin;
                if (OnScreenCompletelyVisible != null)
                {
                    OnScreenCompletelyVisible();
                }
            }
        }
    }

    public void BlackOut()
    {
        blackoutImage.color = colorBegin;
        isBlackingOut = true;
        isFadingIn = false;
        currentColor = colorFinal;
        exponentializer = 0;
    }

    public void FadeIn()
    {
        blackoutImage.color = colorFinal;
        isFadingIn = true;
        isBlackingOut = false;
        currentColor = colorBegin;
        exponentializer = 0;
    }



}
