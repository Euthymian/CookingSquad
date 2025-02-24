using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] private Image barImage;
    [SerializeField] private GameObject hasProgressGameObject;

    private IHasProgress hasProgress;


    private void Start()
    {
        hasProgress = hasProgressGameObject.GetComponent<IHasProgress>();
        barImage.fillAmount = 0;
        hasProgress.OnProgessUpdate += HasProgessUpdate;
        Hide();
    }

    private void HasProgessUpdate(object sender, float e)
    {
        barImage.fillAmount = e;

        if (e == 0f || e == 1f)
            Hide();
        else
            Show();
    }
    
    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
