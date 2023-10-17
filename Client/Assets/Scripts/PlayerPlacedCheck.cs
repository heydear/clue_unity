using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlacedCheck : MonoBehaviour
{
    public ROOM room;
    public Vector2 originPos;



    public void OnTriggerEnter(Collider other)
    {
        other.GetComponentInParent<TokenInfo>().UpdateState(room);
    }

    public void OnTriggerExit(Collider other)
    {
        other.GetComponentInParent<TokenInfo>().UpdateState();
    }
}
