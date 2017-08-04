using UnityEngine;
using System.Collections;

public class Movable : MonoBehaviour {

    private PhysicsEngine physicsEngine;

	// Use this for initialization
	void Start () {
        physicsEngine = gameObject.GetComponent<PhysicsEngine>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider obj)    //Enter handles bounces(?); Stay handles the rest of friction
    {

    }

    void OnTriggerStay(Collider obj)
    {

    }

    void OnTriggerExit(Collider obj)
    {

    }
}
