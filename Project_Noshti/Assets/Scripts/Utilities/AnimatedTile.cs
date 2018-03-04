using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedTile : MonoBehaviour
{
    public Sprite[] sprites;
    public SpriteRenderer spriteRenderer;
    public float framesPerSecond = 1f;

    public bool cycles = true;

    private int currentIndex;
    private int maxIndex;

    private int increment = 1;

    private void Start()
    {
        currentIndex = 0;
        maxIndex = sprites.Length - 1;
        StartCoroutine(AnimateTile());
    }

    private IEnumerator AnimateTile()
    {
        while(true)
        {
            spriteRenderer.sprite = sprites[currentIndex];
            currentIndex += increment;

            if(cycles)
            {
                if((increment == -1 && currentIndex == 0) || (increment == 1 && currentIndex >= maxIndex))
                {
                    increment *= -1;
                }
            }
            else
            {
                if (currentIndex > maxIndex)
                {
                    currentIndex = 0;
                }
            }


            

            yield return new WaitForSeconds(1f / framesPerSecond);
        }
    }
}
