using UnityEngine;
using System.Collections;

public class WeaponTracker : MonoBehaviour {

	[Header("This is spawned by another script by default,")]
	[Header("But you can assign a gameobject here if you wish")]
	public GameObject assignedPlayer;

	private GameObject ourPlayer;
    private AutoLockOn ourAutoLock;
    private bool lockOntoTarget, giveAnAlert, trackerAlert;
    private float weaponLifeSpan;

    // Update is called once per frame
    void Update()
    {
		if (!ourPlayer && assignedPlayer) {
			ourPlayer = assignedPlayer;
			ourAutoLock = ourPlayer.GetComponent<AutoLockOn>();
		}
		transform.forward = ourPlayer.transform.forward;
        transform.position = ourPlayer.transform.position;
    }

    void OnTriggerEnter(Collider obj)
    {
		if (obj.gameObject.tag.Length > 5)
        {
            if (obj.gameObject.tag.Substring(0, 6) == "Weapon")
            {
                if (!obj.gameObject.GetComponent<WeaponScript>())
                {
                    WeaponScript weaponScript = obj.gameObject.AddComponent<WeaponScript>();
                    weaponScript.GetAssigned(ourAutoLock);
                    weaponScript.RequiredData(lockOntoTarget, giveAnAlert, weaponLifeSpan);
                }
                else {

                    // if there is already a weaponscript it came from another gameobject, and we want to know which one it came from

                    WeaponScript weaponScript = obj.gameObject.GetComponent<WeaponScript>();
                    GameObject newObject = weaponScript.OurPlayer();
                    if (newObject.tag.Substring(0, 5) == "Enemy") { ourAutoLock.NewTarget(newObject); }
                }
            }
        }
        if (obj.gameObject.name.Length > 5)
        {
            if ((obj.gameObject.name.Substring(0, 6) == "Finder") && trackerAlert)
            {
                ourAutoLock.NewTarget(obj.gameObject.GetComponent<EnemyFinder>().OurPlayer());
            }
        }
    }

    //all publicly exposed methods are here

    public void AttachPlayer(GameObject player)
    {
        ourPlayer = player;
        ourAutoLock = player.GetComponent<AutoLockOn>();
    }

    public void RequiredData(bool lockOn, bool giveAlert, bool counterTrack, float projectileLifeSpan)
    {
        lockOntoTarget = lockOn;
        giveAnAlert = giveAlert;
        weaponLifeSpan = projectileLifeSpan;
        trackerAlert = counterTrack;
    }
}