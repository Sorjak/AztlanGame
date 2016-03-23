using UnityEngine;
using System.Collections;

public class TriggerState : MonoBehaviour {

    public PlayerTriggers triggerNum;

    private Player mPlayer { get { return transform.root.GetComponent<Player>();  } }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.GetComponent<Collider2D>().gameObject.layer == LayerMask.NameToLayer("Environment"))
            mPlayer.triggerState |= triggerNum;
    }

}
