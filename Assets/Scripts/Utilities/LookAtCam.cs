using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCam : MonoBehaviour
{
    private enum Mode
    {
        LookAt,
        LookAtInverted,
        CamForward,
        CamForwardInverted,
    }

    [SerializeField] private Mode mode;

    private void LateUpdate()
    {
        switch (mode)
        {
            case Mode.LookAt:
                transform.LookAt(Camera.main.transform);
                break;
            case Mode.LookAtInverted:
                Vector3 dirFromCam = transform.position - Camera.main.transform.position;
                transform.LookAt(transform.position + dirFromCam);
                break;
            case Mode.CamForward:
                transform.forward = Camera.main.transform.forward;
                break;
            case Mode.CamForwardInverted:
                transform.forward = -Camera.main.transform.forward;
                break;
        }
    }
}
