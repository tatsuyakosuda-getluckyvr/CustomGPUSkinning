using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupTargetFPS : MonoBehaviour
{

    [SerializeField] private float _targetFrameRate = 90f;

    private void Awake()
    {
        OVRPlugin.systemDisplayFrequency = _targetFrameRate;
    }

}
