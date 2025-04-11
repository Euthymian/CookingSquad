using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class StoveBurnFlashingBarUI : MonoBehaviour
{
    private const string IS_FLASING = "IsFlashing";
    private Animator anim;
    [SerializeField] private StoveCounter stoveCounter;

    private void Awake()
    {
        anim = GetComponent<Animator>();

        anim.SetBool(IS_FLASING, false);
    }

    private void Start()
    {
        stoveCounter.OnProgessUpdate += StoveCounter_OnProgessUpdate;
    }

    private void StoveCounter_OnProgessUpdate(object sender, float e)
    {
        float showBurnFlashingAmount = 0.5f;
        bool showBurnFlashing = stoveCounter.IsFried() && e > showBurnFlashingAmount;

        anim.SetBool(IS_FLASING, showBurnFlashing);
    }
}
