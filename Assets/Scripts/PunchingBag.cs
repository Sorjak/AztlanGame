using UnityEngine;
using System.Collections;

public class PunchingBag : MonoBehaviour {

    private Vector3 startPos;

	// Use this for initialization
	void Start () {
        startPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        if (transform.position.y < -10f)
            transform.position = startPos;
	}
}
