using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ArmorScript : MonoBehaviour {

	[Header("This is spawned by another script by default,")]
	[Header("But you can assign a gameobject here if you wish")]
	public GameObject assignedPlayer;

	private GameObject ourPlayer, newestEnemy;
    private GameObject[] dangerSigns = new GameObject[4];
    private Image[] signImages = new Image[4];
    private AutoLockOn ourAutoLock;
    private bool giveUsWarning, enemyNearby;
    private Animator[] signAnimators = new Animator[4];

    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            dangerSigns[i] = GameObject.Find("Danger (" + i.ToString() + ")");
            signImages[i] = dangerSigns[i].GetComponent<Image>();
            signAnimators[i] = dangerSigns[i].GetComponent<Animator>();
        }
    }

    // Update is called once per frame
    void Update() {
		if (!ourPlayer && assignedPlayer) {
			ourPlayer = assignedPlayer;
			ourAutoLock = ourPlayer.GetComponent<AutoLockOn>();
		}
		Vector3 ourOffset = ourAutoLock.ArmorOffset();
        transform.position = (ourPlayer.transform.position + ourOffset);
    }

	void OnTriggerEnter(Collider obj)
    {
		if (obj.gameObject.tag.Length > 4) {
            if (obj.gameObject.tag.Substring(0, 5) == "Enemy")
            {
                newestEnemy = obj.gameObject;
				Invoke("SendData", .125f);
				if ((!ourAutoLock.AlreadyTargeted(newestEnemy)) && giveUsWarning)
				{
					foreach (Animator animator in signAnimators)
					{
						animator.SetTrigger("IsNear");
					}
				}
			}
        }
    }

    void OnTriggerExit(Collider obj)
    {
		if (obj.gameObject.tag.Length > 4) { if (obj.gameObject.tag.Substring(0, 5) == "Enemy") { enemyNearby = false; } }
    }

    void OnTriggerStay(Collider obj)
    {
		if (obj.gameObject.tag.Length > 4) { if (obj.gameObject.tag.Substring(0, 5) == "Enemy") { enemyNearby = true; } }
    }

    void SendData()
    {
        ourAutoLock.NewTarget(newestEnemy);
    }

    //all publicly exposed methods are here

    public void AttachPlayer(GameObject player)
    {
        ourPlayer = player;
        ourAutoLock = player.GetComponent<AutoLockOn>();
    }

    public void RequiredData(bool giveWarning)
    {
        giveUsWarning = giveWarning;
    }
}
