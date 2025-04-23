using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI keyUpText;
    [SerializeField] private TextMeshProUGUI keyDownText;
    [SerializeField] private TextMeshProUGUI keyLeftText;
    [SerializeField] private TextMeshProUGUI keyRightText;
    [SerializeField] private TextMeshProUGUI keyInteractText;
    [SerializeField] private TextMeshProUGUI keyInteractGamepadText;
    [SerializeField] private TextMeshProUGUI keyInteractAltText;
    [SerializeField] private TextMeshProUGUI keyInteractAltGamepadText;
    [SerializeField] private TextMeshProUGUI keyPauseText;
    [SerializeField] private TextMeshProUGUI keyPauseGamepadText;

    private void Start()
    {
        GameInput.Instance.OnBindingRebind += GameInput_OnBindingRebind;
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
        UpdateVisual();
        Show();
    }

    private void GameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        if (!GameManager.Instance.IsWaiting())
        {
            Hide();
        }
    }

    private void GameInput_OnBindingRebind(object sender, System.EventArgs e)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        keyUpText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveUp);
        keyDownText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveDown);
        keyLeftText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveLeft);
        keyRightText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveRight);
        keyInteractText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Interact);
        keyInteractAltText.text = GameInput.Instance.GetBindingText(GameInput.Binding.InteractAlt);
        keyPauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Pause);
        keyInteractGamepadText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_Interact);
        keyInteractAltGamepadText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_InteractAlt);
        keyPauseGamepadText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_Pause);
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
