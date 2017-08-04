using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

public class CharacterHurt : MonoBehaviour {

    public float myHealth = 100;
    public bool autoHeal = true;
    [Header("if this is false you will only auto heal 1/3 of your health over time")]
    public bool regenAll = false;
    [Header("this is how long it takes to recover a third of your health")]
    [Header("the second third will take 2 times longer, and the third 3 times")]
    [Header("the second and third thirds only heal if 'regenAll' is set to true")]
    public float timeToHeal = 4;

    private float healthStage = 1, healthState = 1, currentHealth, currentTime = -1, fadingTime = -1, desiredAlpha = 0;
    private int currentFades = 0, currentAlphas = 0;
    private bool canAutoHeal, wasDamaged, changingAlpha;
    private Image[] signImages = new Image[16];
    private ColorCorrectionCurves colorCorrection;      //this NEEDS standard assets / image effects to work

    // Use this for initialization
    void Start () {
        for (int i = 0; i < 16; i++)
        {
            signImages[i] = GameObject.Find("Damage (" + i.ToString() + ")").GetComponent<Image>();
        }

        currentHealth = myHealth;
        colorCorrection = GetComponentInChildren<ColorCorrectionCurves>();
    }
	
	// Update is called once per frame
	void Update () {

        //each frame we check to see if we're hurt at all, and if so should we be auto healing

        if (currentTime >= 0) {
            currentTime += Time.deltaTime;
            if (regenAll)
            {
                if (currentTime > (timeToHeal * healthState)) { TakeDamage(-((myHealth / 10) * Time.deltaTime)); }
            } else
            {
                if (canAutoHeal && (currentTime > (timeToHeal * healthState))) { TakeDamage(-((myHealth / 10) * Time.deltaTime)); }
            }
        }
        if      ((currentHealth <= 0) && (colorCorrection.saturation > 0)) { colorCorrection.saturation -= (Time.deltaTime / 3); }
        else if ((currentHealth > 0) && (colorCorrection.saturation < 1))  { colorCorrection.saturation += (Time.deltaTime / 3); }

        if (changingAlpha) { ChangeAlpha(); }
        if (fadingTime > -1) { CheckColor(); }
    }

    void TakeDamage(float damage)   //make this negative to increase our health.  autoHealing should basically always be false
    {
        currentHealth -= damage;
        wasDamaged = (damage > 0) ? true : false;

        if (wasDamaged && autoHeal) { currentTime = 0; }
        if (currentHealth >= myHealth) { currentTime = -1; }
        if (currentHealth <= myHealth)
        {
            if (currentHealth > ((myHealth * 2) / 3)) {
                CheckHealthState(1);                //1 means top third of our health, 2 is middle third, and 3 is bottom third
            }
            else if (currentHealth > (myHealth / 3))
            {
                CheckHealthState(2);
            }
            else if (currentHealth > 0)
            {
                CheckHealthState(3);
            }
            else {
                if (healthStage == 4) { print("GAME OVER"); } //GAME OVER stuff begins here
                else {
                    CheckHealthState(4);
                }
            }
        }
    }

    void CheckHealthState(int n)    //n equals our health stage, with 1 being healthy, 2 being hurt, and 3 being dying
    {
        if (healthStage != n)
        {
            ChangeHealthStage(n);
            if (wasDamaged) { if (n > 1) { ChangeColor(((n - 2) * 4), .5f); } }
            else            { if (n < 4) { ChangeColor(((n) * 4), 0); } }
        }

        changingAlpha = true;       //used to set the color immediately, but now want to transition to it instead
        int i = ((n - 1) * 4);      //this little formula turns only some of the danger zones to a specific level of clarity depending on our current health
        currentAlphas = i;
        desiredAlpha = ((1 - ((currentHealth - (((8 - i) / 4) * (myHealth / 3))) / (myHealth / 4))) / 2); //.5f ensures some transparency even with multiple layers

        if (wasDamaged)
        {
            fadingTime = 0;
            if (n < 4) { currentFades = (n * 4); }
        }
    }

    void ChangeHealthStage(float myStage)
    {
        //if (healthStage == (myStage - 1)) { currentHealth = ((myHealth / 3) * (4 - myStage)); }
        healthStage = myStage;
        if (wasDamaged) {
            healthState = 1;
            canAutoHeal = true;
        }
        else {
            healthState++;
            if (!regenAll) { canAutoHeal = false; }
        }
        currentTime = 0;
    }

    void ChangeColor(int n, float setTo)
    {
        for (int i = n; i < (n + 4); i++)
        {
            signImages[i].color = new Color(signImages[i].color.r, signImages[i].color.g, signImages[i].color.b, setTo);
        }
    }

    void ColorFading(float percentage)
    {
        for (int i = currentFades; i < (currentFades + 4); i++)
        {
            Color newColor = signImages[i].color;
            signImages[i].color = new Color(newColor.r, newColor.g, newColor.b, (percentage / 2));
        }
    }

    void ChangeAlpha()
    {
        float sign = (desiredAlpha > signImages[currentAlphas].color.a) ? 1 : -1;
        if (Mathf.Abs(desiredAlpha - signImages[currentAlphas].color.a) < Time.deltaTime)
        {
            for (int i = currentAlphas; i < (currentAlphas + 4); i++)
            {
                signImages[i].color = new Color(signImages[i].color.r, signImages[i].color.g, signImages[i].color.b, desiredAlpha);
                changingAlpha = false;
            }
        }
        else
        {
            float newAlpha = ((signImages[currentAlphas].color.a + Time.deltaTime) * sign);
            for (int i = currentAlphas; i < (currentAlphas + 4); i++)
            {
                signImages[i].color = new Color(signImages[i].color.r, signImages[i].color.g, signImages[i].color.b, newAlpha);
            }
        }
    }

    void CheckColor()
    {
        if (fadingTime > 1)
        {
            fadingTime = -1;
            ColorFading(0);
        }
        else if (fadingTime > .5f)
        {
            fadingTime += Time.deltaTime;
            ColorFading((1 - fadingTime) * 2);
        }
        else if (fadingTime >= 0)
        {
            fadingTime += Time.deltaTime;
            ColorFading(fadingTime * 2);
        }
    }
}
