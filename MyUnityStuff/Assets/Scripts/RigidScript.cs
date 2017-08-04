using UnityEngine;
using System.Collections;

public class RigidScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter(Collision obj)
    {

    }

    void OnCollisionStay(Collision obj)
    {
        if (obj.gameObject.GetComponent<Collider>())
        {
            PhysicMaterial material = obj.gameObject.GetComponent<Collider>().sharedMaterial;
            if (material != null)
            {

            } else
            {

            }

        } else
        {

        }
    }

    void OnTriggerEnter(Collider obj)
    {

    }
}
