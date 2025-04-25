using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    private Transform targetTransform;

    public void SetTargetTransform(Transform targetTransform)
    {
        this.targetTransform = targetTransform;
    }

    private void LateUpdate()
    {
        if (this.targetTransform != null)
        {
            transform.position = this.targetTransform.position;
            transform.rotation = this.targetTransform.rotation;
        }
    }
}
