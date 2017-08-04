using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EnemyFinder : MonoBehaviour
{
	[Header("This is spawned by another script by default,")]
	[Header("But you can assign a gameobject here if you wish")]
	public GameObject assignedPlayer;

	private GameObject enemySighted, lockingOnImage;
    private AutoLockOn ourAutoLock;
    private float neededTime = 2, timeSoFar = 0;    //I'll just leave the required time here for now; it's still easily accessible
    private bool lockedOn, useOurTargeting;
    private GameObject targetingSight;
    private Image sightImage;

    // Use this for initialization
    void Start()
    {
        targetingSight = GameObject.Find("Sight");
        sightImage = targetingSight.GetComponent<Image>();
        sightImage.color = new Color(0, 1, .75f, 0);
    }

    // Update is called once per frame
    void Update()
    {
		if (lockedOn)
        {
			if (!ourAutoLock && assignedPlayer)
			{
				ourAutoLock = assignedPlayer.GetComponent<AutoLockOn>();
			}
			if (ourAutoLock.AlreadyTargeted(enemySighted))
            {
                if (useOurTargeting) { sightImage.color = Color.red; }
            }
            else {

                //this cannot be handled by an animator, because it cannot seem to handle the rotation

                timeSoFar += Time.deltaTime;
                targetingSight.transform.rotation = Quaternion.Euler(0, 0, (1080 * (timeSoFar / neededTime)));
                Color newColor = sightImage.color;
                newColor.a = (timeSoFar / neededTime);
                if (useOurTargeting) { sightImage.color = newColor; }
                
                if (timeSoFar >= neededTime)
                {
                    if (useOurTargeting) { sightImage.color = Color.red; }
                    ourAutoLock.NewTarget(enemySighted.gameObject);
                }
            }
        }
    }

    void OnTriggerEnter(Collider obj)
    {
        if (obj.gameObject.tag.Length > 4)
        {
            if (obj.gameObject.tag.Substring(0, 5) == "Enemy")
            {
                enemySighted = obj.gameObject;
            }
        }
    }

    void OnTriggerStay(Collider obj)
    {
        if (obj.gameObject.tag.Length > 4)
        {
            if (obj.gameObject.tag.Substring(0, 5) == "Enemy")
            {
                lockedOn = true;
            }
        }
    }

    void OnTriggerExit(Collider obj)
    {
        if (obj.gameObject.tag.Length > 4)
        {
            if (obj.gameObject.tag.Substring(0, 5) == "Enemy")
            {
                lockedOn = false;
                timeSoFar = 0;
                targetingSight.transform.rotation = Quaternion.Euler(0, 0, 0);
                sightImage.color = new Color(0, 1, .75f, 0);
            }
        }
    }

    //all publicly exposed methods are here

    public void AttachPlayer(GameObject player)
    {
        ourAutoLock = player.GetComponent<AutoLockOn>();
    }

    public GameObject OurPlayer()
    {
        return ourAutoLock.gameObject;
    }

    public void RequiredData(bool useTargeting)
    {
        useOurTargeting = useTargeting;
    }

    public void HideSight()
    {
        sightImage.color = new Color(0, 1, .75f, 0);
    }
}
