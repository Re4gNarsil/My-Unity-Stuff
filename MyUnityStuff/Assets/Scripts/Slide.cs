using UnityEngine;
using System.Collections;

public class Slide : MonoBehaviour {

    public bool move;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.Space)) { move = true; }
        if (move)
        {
            move = false;
            Vector3 newRotation = transform.root.eulerAngles;
            newRotation.z += .5f;
            transform.rotation = Quaternion.Euler(newRotation);
        }
	}
}
