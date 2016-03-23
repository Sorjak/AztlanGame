using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PrayerPlatform : MonoBehaviour {


    void OnTriggerStay2D(Collider2D col)
    {
        if (col.GetComponent<Collider2D>().tag == "Player")
        {
            Player p = col.GetComponentInParent<Player>();
            if (p.praying)
            {
                p.currentPrayer += p.prayerWeight;
            }
        }
    }
}
