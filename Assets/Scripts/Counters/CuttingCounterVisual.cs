using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounterVisual : MonoBehaviour
{
    private Animator anim;

    private const string CUT = "Cut";

    [SerializeField] private CuttingCounter cct;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        cct.OnCuttingAnimationTrigger += Cct_OnPlayerInteract;
    }

    private void Cct_OnPlayerInteract(object sender, System.EventArgs e)
    {
        anim.SetTrigger(CUT);
    }
}
