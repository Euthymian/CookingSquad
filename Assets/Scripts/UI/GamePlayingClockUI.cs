using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayingClockUI : MonoBehaviour
{
    [SerializeField] private Image clockImgae;
    private Color currentColor;

    private void Update()
    {
        float process = GetFillAmount();

        clockImgae.fillAmount = process;
        
        if (process <= 0.5)
            currentColor = Color.Lerp(Color.green, Color.yellow, process*2);
        else
            currentColor = Color.Lerp(Color.yellow, Color.red, (process-0.5f)*2);
        clockImgae.color = currentColor;
    }

    private float GetFillAmount()
    {
        return 1 - GameManager.Instance.GetGamePlayingTimerNormalized();
    }
}
