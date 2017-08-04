using UnityEngine;
using System.Collections;

public class WindManager : MonoBehaviour {

    [Header("Maximum wind force plus how quickly it changes")]
    public float maxWindForce = 15000;
    public float maxWindChange = 150;

    private GameObject uiGauge, uiMeter, uiCompass;
    private Vector3 windDirection;
    private float windForce;

    // Use this for initialization
    void Start () {
        float windAngle = Random.Range(0, 359);
        windForce = Random.Range((maxWindChange), (maxWindForce));
        windDirection = new Vector3(0, windAngle, 0);
        transform.rotation = Quaternion.Euler(windDirection);
    }
	
	// Update is called once per frame
	void Update () {
        float changeDirection = (Random.Range(-60, 60) * Time.deltaTime);
        float changeForce = (Random.Range(-maxWindChange, maxWindChange) * Time.deltaTime);
        windDirection.y += changeDirection;
        transform.rotation = Quaternion.Euler(windDirection);

        windForce += changeForce;
        if (windForce < maxWindChange) { windForce = maxWindChange; }
        if (windForce > maxWindForce) { windForce = maxWindForce; }
    }

    public Vector4 GetWind()
    {
        return new Vector4(windDirection.x, windDirection.y, windDirection.z, windForce);
    }

    public Vector4 AcquireWind()
    {
        return new Vector4(transform.forward.x, transform.forward.y, transform.forward.z, windForce);
    }
}
