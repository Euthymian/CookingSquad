using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    [SerializeField] private AudioClipRefsSO audioClipRefsSO;

    private void Start()
    {
        DeliveryManager.Instance.OnDeliverRecipeSuccess += DeliverManager_OnDeliverRecipeSuccess;
        DeliveryManager.Instance.OnDeliverRecipeFail += DeliverManager_OnDeliverRecipeFail;
        CuttingCounter.OnAnyCut += CuttingCounter_OnAnyCut;
        Player.Instance.OnPickupSomething += Player_OnPickupSomething;
        BaseCounter.OnAnyObjectDropped += BaseCounter_OnAnyObjectDropped;
        TrashCounter.OnDestroyObject += TrashCounter_OnDestroyObject;
    }

    private void TrashCounter_OnDestroyObject(object sender, System.EventArgs e)
    {
        TrashCounter trashCounter = (TrashCounter)sender;
        PlaySound(audioClipRefsSO.trash, trashCounter.transform.position);
    }

    private void BaseCounter_OnAnyObjectDropped(object sender, System.EventArgs e)
    {
        BaseCounter baseCounter = (BaseCounter)sender;
        PlaySound(audioClipRefsSO.objectDrop, baseCounter.transform.position);
    }

    private void Player_OnPickupSomething(object sender, System.EventArgs e)
    {
        PlaySound(audioClipRefsSO.objectPickup, Player.Instance.transform.position);
    }

    private void CuttingCounter_OnAnyCut(object sender, System.EventArgs e)
    {
        CuttingCounter cuttingCounter = sender as CuttingCounter;
        PlaySound(audioClipRefsSO.chop, cuttingCounter.transform.position);
    }

    private void DeliverManager_OnDeliverRecipeFail(object sender, System.EventArgs e)
    {
        PlaySound(audioClipRefsSO.deliverFail, DeliveryCounter.Instance.transform.position);
    }

    private void DeliverManager_OnDeliverRecipeSuccess(object sender, System.EventArgs e)
    {
        PlaySound(audioClipRefsSO.deliverSuccess, DeliveryCounter.Instance.transform.position);
    }

    private void PlaySound(AudioClip audioClip, Vector3 position, float volume)
    {
        AudioSource.PlayClipAtPoint(audioClip, position, volume);
    }

    private void PlaySound(AudioClip[] audioClipArray, Vector3 position, float volume = 1f)
    {
        AudioSource.PlayClipAtPoint(audioClipArray[Random.Range(0, audioClipArray.Length)], position, volume);
    }

    public void PlayFootstepSound(Vector3 position)
    {
        PlaySound(audioClipRefsSO.footstep, position);
    }
}
