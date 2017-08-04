using UnityEngine;
using System.Collections;

public class WhereIllBe : MonoBehaviour {

    [Header("adjust the offset as needed depending on the speed of your game")]
    public float offsetAmount = 1;
    public float mySize = 2;
    [Header("Make sure to add the tag 'Target' in the inspector")]
    public bool hasCollider = true;
    public bool canMoveSideways = true;

    private Rigidbody rigidBody;
    private GameObject myTarget;

    // Use this for initialization
    void Start()
    {
        foreach (Transform child in transform) //seems to be necessary given when you destroy and re-create a unity gameobject it retains things created during runtime
        {
            if (child.gameObject.tag == "Target") { Destroy(child.gameObject); }
        }

        //create our 'pathfinder' object, then make it a childobject if we're an object that can only go straight usually, like a car

        myTarget = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Destroy(myTarget.GetComponent<MeshRenderer>());
        if (hasCollider) { myTarget.GetComponent<Collider>().isTrigger = true; }
        else { Destroy(myTarget.GetComponent<Collider>()); }

        myTarget.tag = "Target";
        myTarget.name = "myTarget";
        myTarget.transform.localScale = (new Vector3(1, 1, 1) * mySize);
        if (!canMoveSideways) { myTarget.transform.parent = gameObject.transform; }
        myTarget.transform.localPosition = new Vector3(0, 0, 0);
        rigidBody = GetComponent<Rigidbody>();
    }

	// Update is called once per frame
	void Update () {

        if (canMoveSideways)
        {
            myTarget.transform.forward = transform.forward;
            Vector3 ourVelocity = rigidBody.velocity;
            myTarget.transform.position = (transform.position + (ourVelocity * offsetAmount));
        }
        else {

            //if we are childed we can only send our pathfinder forward or backwards, and need to calculate how much to move

            myTarget.transform.forward = transform.forward;
            Vector3 ourVelocity = rigidBody.velocity;
            float offsetFront = Mathf.Sqrt((ourVelocity.x * ourVelocity.x) + (ourVelocity.z * ourVelocity.z));
            float offsetFinal = (Mathf.Sqrt((offsetFront * offsetFront) + (ourVelocity.y * ourVelocity.y)) * 2); //2 seems to work very well currently

            //we need to determine if our velocity is in the same direction we are facing

            float sign = 1;
            if      (Mathf.Abs(transform.forward.x) > .5f) { if ((transform.forward.x * ourVelocity.x) < 0) sign = -1; }
            else if (Mathf.Abs(transform.forward.z) > .5f) { if ((transform.forward.z * ourVelocity.z) < 0) sign = -1; }
            else if (Mathf.Abs(transform.forward.y) > .5f) { if ((transform.forward.y * ourVelocity.y) < 0) sign = -1; }

            myTarget.transform.localPosition = new Vector3(0, 0, (offsetFinal * offsetAmount * sign));
            //myTarget.transform.localScale = (Vector3.one + (Vector3.one * (offsetFinal / 50)));   //50 seems to be a good number if you want it to expand as you go faster
        }
    }

    //all publicly exposed methods are here

    public void ChangeSize(float ChangeSize)
    {
        myTarget.transform.localScale = (myTarget.transform.localScale * ChangeSize);
    }

    public void ChangeOffset(float changeOffset)
    {
        offsetAmount = changeOffset;
    }
}
