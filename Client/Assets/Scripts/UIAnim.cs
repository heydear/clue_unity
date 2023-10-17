using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAnim : MonoBehaviour
{
    public Animator animator;
    public Toggle toggle;
    


    public void SetAnim(bool isActive)
    {
        animator.SetBool("isOpen", isActive);
    }

    public void SetToggleRev(bool isOn)
    {
        toggle.isOn = !isOn;
    }
}
