using UnityEngine;
using System.Collections;

public class MagnusEffect : MonoBehaviour {

    [Header("About 5 - 10% of your mass is a good starting point")]
    public float magnusConstant = .05f;

    private Rigidbody rigidBody;
    private PhysicsEngine physicsEngine;

    // Use this for initialization
    void Start () {
        rigidBody = GetComponent<Rigidbody>();
        physicsEngine = GetComponent<PhysicsEngine>();
	}
	
	// Update is called once per frame
	void Update () {
        if (physicsEngine) { physicsEngine.AddForce(Vector3.Cross(rigidBody.angularVelocity, rigidBody.velocity) * magnusConstant); }
        else if (rigidBody) { rigidBody.AddForce(Vector3.Cross(rigidBody.angularVelocity, rigidBody.velocity) * magnusConstant); }
    }
}
