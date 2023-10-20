using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandAnimatorController : MonoBehaviour
{
    [SerializeField] private InputActionProperty LtriggerAction;
    [SerializeField] private InputActionProperty LgripAction;
    [SerializeField] private InputActionProperty RtriggerAction;
    [SerializeField] private InputActionProperty RgripAction;

    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float LtriggerValue = LtriggerAction.action.ReadValue<float>();
        float LgripValue = LgripAction.action.ReadValue<float>();

        anim.SetFloat("LTrigger", LtriggerValue);
        anim.SetFloat("LGrip", LgripValue);

        float RtriggerValue = RtriggerAction.action.ReadValue<float>();
        float RgripValue = RgripAction.action.ReadValue<float>();

        anim.SetFloat("RTrigger", RtriggerValue);
        anim.SetFloat("RGrip", RgripValue);
    }
}
