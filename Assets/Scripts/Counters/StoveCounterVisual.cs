using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterVisual : MonoBehaviour
{
    [SerializeField] private StoveCounter sct;
    [SerializeField] private GameObject stoveOnVisual;
    [SerializeField] private GameObject stoveOnParticleSystem;

    private void Start()
    {
        sct.OnStateChanged += Sct_OnStateChanged;
    }

    private void Sct_OnStateChanged(object sender, StoveCounter.OnStateChangedEventArgs e)
    {
        bool enableVisual = (e.state == StoveCounter.State.Frying || e.state == StoveCounter.State.Fried);
        stoveOnVisual.SetActive(enableVisual);
        stoveOnParticleSystem.SetActive(enableVisual);
    }
}
