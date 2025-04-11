using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryResultUI : MonoBehaviour
{
    private const string POPUP_ANIMATION = "Popup";
    
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Color successColor;
    [SerializeField] private Color failureColor;
    [SerializeField] private Sprite successSprite;
    [SerializeField] private Sprite failureSprite;

    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        DeliveryManager.Instance.OnDeliverRecipeSuccess += DeliveryManager_OnDeliverRecipeSuccess;
        DeliveryManager.Instance.OnDeliverRecipeFail += DeliveryManager_OnDeliverRecipeFail;

        Hide();
    }

    private void DeliveryManager_OnDeliverRecipeFail(object sender, System.EventArgs e)
    {
        Show();
        anim.SetTrigger(POPUP_ANIMATION);
        backgroundImage.color = failureColor;
        iconImage.sprite = failureSprite;
        resultText.text = "DELIVERY\nFAILURE";
    }

    private void DeliveryManager_OnDeliverRecipeSuccess(object sender, System.EventArgs e)
    {
        Show();
        anim.SetTrigger(POPUP_ANIMATION);
        backgroundImage.color = successColor;
        iconImage.sprite = successSprite;
        resultText.text = "DELIVERY\nSUCCESS";
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
}
