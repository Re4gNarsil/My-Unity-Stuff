using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GravityManager : MonoBehaviour {

    public const float gravityStrength = 6.67e-11f;   //G is a constant equal to 6.67 × 10-11 N-m2/kg2

    private List<PhysicsEngine> physicsEngineArray;

    // Use this for initialization
    void Start () {
        physicsEngineArray = FindObjectsOfType<PhysicsEngine>().ToList();
    }
	
	// Update is called once per frame
	void FixedUpdate () {

		//Go to each object that has gravity applied to itself, and then calculate the gravitational effect of all the other objects upon it

        foreach (PhysicsEngine physicsEngineA in physicsEngineArray)
        {
            foreach (PhysicsEngine physicsEngineB in physicsEngineArray)
            {
                if ((physicsEngineA.gameObject != physicsEngineB.gameObject) && (physicsEngineA.useGravity)) {
                    
                    GameObject physA = physicsEngineA.gameObject, physB = physicsEngineB.gameObject;
                    Vector3 directionofGravity = (physA.transform.position - physB.transform.position);

                    float gravityForceHere = (gravityStrength * (((physicsEngineA.myMass + physicsEngineB.myMass)) / Mathf.Pow(directionofGravity.magnitude, 2)));
                    physicsEngineA.AddForce(-directionofGravity.normalized * gravityForceHere);
                }
            }
        }
    }

    public void AddNewObject(PhysicsEngine engine)
    {
        physicsEngineArray.Add(engine);
    }

    public void RemoveObject(PhysicsEngine engine)
    {
        physicsEngineArray.Remove(engine);
    }
}
