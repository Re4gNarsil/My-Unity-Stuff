using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WeaponScript : MonoBehaviour {

	private AutoLockOn myAutoLock;
    private bool giveHitAlert, sendTargetData;
    private GameObject sight;
    private Animator animator;

    // Use this for initialization
    void Start()
    {
        sight = GameObject.Find("Dot");
        animator = sight.GetComponent<Animator>();
    }

    void OnTriggerEnter(Collider obj)
    {
		if (obj.gameObject.tag.Length > 4)
        {
            if (obj.gameObject.tag.Substring(0, 5) == "Enemy")
            {
                if (giveHitAlert) { animator.SetTrigger("Hit"); }
                if ((!myAutoLock.AlreadyTargeted(obj.gameObject)) && sendTargetData) { myAutoLock.NewTarget(obj.gameObject); }
                Destroy(gameObject);
            }
        }
    }

    void OnCollisionEnter(Collision obj)
    {
		if (obj.gameObject.tag.Length > 4)
        {
            if (obj.gameObject.tag.Substring(0, 5) == "Enemy")
            {
                if (giveHitAlert) { animator.SetTrigger("Hit"); }
                if ((!myAutoLock.AlreadyTargeted(obj.gameObject)) && sendTargetData) { myAutoLock.NewTarget(obj.gameObject); }
            }
        }
        Destroy(gameObject);
    }

    //all publicly exposed methods are here

    public void GetAssigned(AutoLockOn newAutolock)
    {
        myAutoLock = newAutolock;
    }

    public GameObject OurPlayer()
    {
        return myAutoLock.gameObject;
    }

    public void RequiredData(bool sendTarget, bool giveAlert, float myLifeSpan)
    {
        sendTargetData = sendTarget;
        giveHitAlert = giveAlert;
        Invoke("DestroyMyself", myLifeSpan);
    }

    void DestroyMyself()
    {
        Destroy(gameObject);
    }
}
