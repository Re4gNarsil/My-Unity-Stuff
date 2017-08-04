using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections.Generic;

public class AutoLockOn : MonoBehaviour {

    [Header("Turn on any you want the script to use")]
    [Header("Make sure your projectiles have a rigidbody and collider")]
    [Header("Make sure your projectiles have a tag starting with 'Weapon")]
    [Header("a tag starting with 'Enemy' has to be made for this to work")]
    public bool lockOnWhenStriking;
    public bool lockOnWhenHit, lockOnWhenNear, lockOnWhenTargeting, automaticallyLockOn;
    [Header("These need to be set up with Input Manager")]
    public string scrollThroughUp = "ScrollUp";
    public string scrollThroughDown = "ScrollDown";
    public string manualCamera = "Manual";
    public string targetEnemy = "Target";
    [Header("size of our 'enemy detection armor', if you use 'WhenNear'")]
    public float armorSize = 1;
    [Header("size of our 'weapon detection armor', if you use 'whenFiring'")]
    [Header("make this just a tad larger than needed to detect all your weapons")]
    [Header("this tracker also will detect enemy projectiles that get very close")]
    public Vector3 trackerSize = new Vector3(1, 1, 1);
    [Header("this tracker can also tell us when WE are being targeted if we want")]
    public bool alertToTrackers;
    [Header("the projectiles can alert us when we successfully hit an enemy")]
    public bool hitAlert = true;
    public float projectileLifeSpan = 3;
    public Vector3 armorOffset = new Vector3(0, 0, 0);
    [Header("you'll need to set up a canvas with my 'danger warnings' pinned to the corners")]
    public bool dangerWarnings = true;
    //public Vector3 trackerOffset = new Vector3(0, 0, 0); //hopefully this ends up not being needed
    [Header("this 'finder' extends as far as you want to be able to detect enemies")]
    public float lockOnDistance = 50;
    [Header("you'll need to set up a canvas with one my sights wherever near the center is best ")]
    public bool useTargetingSystem = true;
    private GameObject myCamera, myTracker;
    private List<GameObject> allOurEnemies = new List<GameObject>();
    private int currentEnemy = -1;
    private CharacterCamera characterCamera; //wanted to not put this in, but it seems necessary in order to make this work

    // Use this for initialization
    void Start () {
	    if (lockOnWhenNear)
        {
            CreateArmor();
        }

        if (lockOnWhenNear)
        {
            CreateTracker();
        }
        characterCamera = GetComponentInChildren<CharacterCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!myCamera) { myCamera = transform.GetComponentInChildren<Camera>().gameObject; }
        if (CrossPlatformInputManager.GetButton(targetEnemy))
        {
            if (!myTracker) { CreateFinder(); }
        }
        else if (CrossPlatformInputManager.GetButtonUp(targetEnemy))
        {
            myTracker.GetComponent<EnemyFinder>().HideSight();
            Destroy(myTracker);
        }
        else if (CrossPlatformInputManager.GetButtonDown(scrollThroughUp))
        {
            currentEnemy++;

            //whenever we want to look at a targeted enemy we tell the camera script to simply have the anchor gameobject between the player
            //and camera look at the enemy object, while disabling at least some - if not all - mouse controls until we switch back to manual

            if (CheckEnemies()) { characterCamera.DisableMovement(true, allOurEnemies[currentEnemy]); }
        }
        else if (CrossPlatformInputManager.GetButtonDown(scrollThroughDown))
        {
            //we have the ability to cycle through enemies forwards or backwards

            currentEnemy--;
            if (CheckEnemies()) { characterCamera.DisableMovement(true, allOurEnemies[currentEnemy]); }
        }
        else if (CrossPlatformInputManager.GetButtonDown(manualCamera))
        {
            currentEnemy = -1;
            characterCamera.DisableMovement(false, null);
        }
    }

    // if we want them create our armor, tracker, and finder, and equip them with everything they need

	//Note that the armor and tracker are NOT set as children because we need the triggers to fire separately

    void CreateArmor()
    {
        GameObject myArmor = new GameObject();
        ArmorScript armorScript = myArmor.AddComponent<ArmorScript>();
        SphereCollider armor = myArmor.AddComponent<SphereCollider>();
		armorScript.AttachPlayer(gameObject);
		armorScript.RequiredData(dangerWarnings);

		armor.radius = armorSize;
        myArmor.transform.position = transform.position;
        armor.isTrigger = true;
        myArmor.name = "Armor";

		Rigidbody myRigidBody = myArmor.AddComponent<Rigidbody>();
		myRigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    void CreateTracker()
    {
        GameObject myDetecter = new GameObject();
        WeaponTracker weaponTracker = myDetecter.AddComponent<WeaponTracker>();
        myDetecter.transform.position = transform.position;
        myDetecter.transform.localScale = trackerSize;
        weaponTracker.AttachPlayer(gameObject);
        weaponTracker.RequiredData(lockOnWhenStriking, hitAlert, alertToTrackers, projectileLifeSpan);

        BoxCollider armor = myDetecter.AddComponent<BoxCollider>();
        armor.isTrigger = true;
        armor.size = trackerSize;
        myDetecter.name = "Tracker";

		Rigidbody myRigidBody = myDetecter.AddComponent<Rigidbody>();
		myRigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
	}

	//this can be childed to the gameobject because it has no trigger colliders

    void CreateFinder()
    {
        myTracker = new GameObject();
        myTracker.transform.parent = gameObject.transform;
        myTracker.transform.localScale = new Vector3(1, 1, 1);
        myTracker.transform.localPosition = new Vector3(0, 0, (lockOnDistance / 2));
        myTracker.transform.localRotation = Quaternion.Euler(90, 0, 0);
        myTracker.name = "Finder";

        EnemyFinder myFinder = myTracker.AddComponent<EnemyFinder>();
        myFinder.AttachPlayer(gameObject);
        myFinder.RequiredData(useTargetingSystem);

        CapsuleCollider myCollider = myTracker.AddComponent<CapsuleCollider>();
        myCollider.isTrigger = true;
        myCollider.radius = 1;
        myCollider.height = lockOnDistance;
    }

    bool ExistingEnemy(GameObject thisEnemy)
    {
        foreach(GameObject enemy in allOurEnemies)
        {
            if (enemy == thisEnemy) { return true; }
        }
        return false;
    }

    bool CheckEnemies()
    {
        //every time we hit the scroll button we cycle through the enemy list, removing entries that are null

        if (allOurEnemies.ToArray().Length > 0)
        {
            if (currentEnemy >= allOurEnemies.ToArray().Length) { currentEnemy = 0; }
            else if (currentEnemy < 0) { currentEnemy = (allOurEnemies.ToArray().Length - 1); }
            if (allOurEnemies[currentEnemy]) { return true; }
            else { allOurEnemies.RemoveAt(currentEnemy); }
        }
        return false;
    }

    //all publicly exposed methods are here

    public void NewTarget(GameObject newEnemy)
    {
        if (!ExistingEnemy(newEnemy)) {
            allOurEnemies.Add(newEnemy);
            if (automaticallyLockOn) {
                currentEnemy = allOurEnemies.IndexOf(newEnemy);
                characterCamera.DisableMovement(true, allOurEnemies[currentEnemy]);
            }
        }
    }

    public bool AlreadyTargeted(GameObject newTarget)
    {
        if (ExistingEnemy(newTarget)) { return true; }
        else { return false; }
    }

    public Vector3 ArmorOffset()
    {
        return (transform.forward * armorOffset.z) + (transform.right * armorOffset.x) + (transform.up * armorOffset.y);
    }

    //public Vector3 TrackerOffset()    hopefully we don't need to worry about offsetting the tracker every frame
    //{
    //    return (transform.position + (transform.forward * trackerOffset.z) + (transform.right * trackerOffset.x) + (transform.up * trackerOffset.y));
    //}

    public void ChangeArmorOffset(Vector3 newOffset)
    {
        armorOffset = newOffset;
    }

    //public void ChangeTrackerOffset(Vector3 newOffSet)
    //{
    //    trackerOffset = newOffset;
    //}

    public void ChangeDetectionDistance(float newDistance)
    {
        lockOnDistance = newDistance;
    }
}
