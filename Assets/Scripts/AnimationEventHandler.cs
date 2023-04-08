using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventHandler : MonoBehaviour
{
    public Transform player;
    public void attackFinished()
    {
        player.GetComponent<RigidbodyFirstPersonController>().attackFinished = true;
    }
}
