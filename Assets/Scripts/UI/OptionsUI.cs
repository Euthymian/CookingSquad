using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{
    public static OptionsUI Instance { get; private set; }

    [SerializeField] private Button soundEffectButton;
    [SerializeField] private Button musicButton;
    [SerializeField] private TextMeshProUGUI soundEffectText;
    [SerializeField] private TextMeshProUGUI musicText;
    [SerializeField] private Button backButton;

    [SerializeField] private Button upButton;
    [SerializeField] private Button downButton;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private Button interactButton;
    [SerializeField] private Button interactAlternateButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button interactGamepadButton;
    [SerializeField] private Button interactAlternateGamepadButton;
    [SerializeField] private Button pauseGamepadButton;

    [SerializeField] private TextMeshProUGUI upText;
    [SerializeField] private TextMeshProUGUI downText;
    [SerializeField] private TextMeshProUGUI leftText;
    [SerializeField] private TextMeshProUGUI rightText;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private TextMeshProUGUI interactAlternateText;
    [SerializeField] private TextMeshProUGUI pauseText;
    [SerializeField] private TextMeshProUGUI interactGamepadText;
    [SerializeField] private TextMeshProUGUI interactAlternateGamepadText;
    [SerializeField] private TextMeshProUGUI pauseGamepadText;

    [SerializeField] private Transform pressToRebindTransform;

    private void Awake()
    {
        Instance = this;

        soundEffectButton.onClick.AddListener(() =>
        {
            SoundManager.Instance.ChangeVolume();
            UpdateVisual();
        });

        musicButton.onClick.AddListener(() => 
        { 
            MusicManager.Instance.ChangeVolume();
            UpdateVisual();
        });

        backButton.onClick.AddListener(() =>
        {
            PausedUI.Instance.Show();
            Hide();
        });

        upButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.MoveUp); });
        downButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.MoveDown); });
        leftButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.MoveLeft); });
        rightButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.MoveRight); });
        interactButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Interact); });
        interactAlternateButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.InteractAlt); });
        pauseButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Pause); });
        interactGamepadButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Gamepad_Interact); });
        interactAlternateGamepadButton.onClick.AddListener(() => {RebindBinding(GameInput.Binding.Gamepad_InteractAlt); });
        pauseGamepadButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Gamepad_Pause); });
    }

    private void Start()
    {
        GameInput.Instance.OnInteractPause += GameInput_OnInteractPause;
        UpdateVisual();
        HidePressToRebind();
        Hide();
    }

    private void GameInput_OnInteractPause(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsPausing)
        {
            Hide();
        }
    }

    private void UpdateVisual()
    {
        soundEffectText.text = "Sound Effect: " + Mathf.Round(SoundManager.Instance.GetVolume() * 10f);
        musicText.text = "Music: " + Mathf.Round(MusicManager.Instance.GetVolume() * 10f);

        upText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveUp);
        downText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveDown);
        leftText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveLeft);
        rightText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveRight);
        interactText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Interact);
        interactAlternateText.text = GameInput.Instance.GetBindingText(GameInput.Binding.InteractAlt);
        pauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Pause);
        interactGamepadText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_Interact);
        interactAlternateGamepadText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_InteractAlt);
        pauseGamepadText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_Pause);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        soundEffectButton.Select();
    }

    private void Hide()
    { 
        gameObject.SetActive(false);
    }

    private void ShowPressToRebind()
    {
        pressToRebindTransform.gameObject.SetActive(true);
    }

    private void HidePressToRebind()
    {
        pressToRebindTransform.gameObject.SetActive(false);
    }

    private void RebindBinding(GameInput.Binding binding)
    {
        ShowPressToRebind();
        GameInput.Instance.RebindBinding(binding, () => { 
            HidePressToRebind(); 
            UpdateVisual();
        });
    }
}
