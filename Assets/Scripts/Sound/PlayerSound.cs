using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    private Player player;
    private float footstepTimer;
    private float footstepTime = .1f;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Update()
    {
        footstepTimer -= Time.deltaTime;
        if (footstepTimer < 0 )
        {
            footstepTimer = footstepTime;

            if(player.IsWalking())
                SoundManager.Instance.PlayFootstepSound(player.transform.position);
        }
    }
}
