using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerCounterVisual : MonoBehaviour
{
    private Animator anim;

    private const string OPEN_CLOSE = "OpenClose";

    [SerializeField] private ContainerCounter cct;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        cct.OnPlayerInteractAnimationTrigger += Cct_OnPlayerInteract;
    }

    private void Cct_OnPlayerInteract(object sender, System.EventArgs e)
    {
        anim.SetTrigger(OPEN_CLOSE);
    }
}
