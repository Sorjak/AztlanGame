using UnityEngine;
using System.Collections;

public class AttackHitBox : MonoBehaviour {

    public float strength = 10f;
    public Vector2 direction = new Vector2(1, 0);

	// Use this for initialization
	void Start () {
        //Destroy(this.gameObject, .333f);
	}


    void OnTriggerEnter2D(Collider2D col)
    {
        Rigidbody2D rb = col.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.AddForce(direction * strength, ForceMode2D.Impulse);
        }
    }
}
