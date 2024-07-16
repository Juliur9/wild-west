using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [SerializeField]
    private Light sun;

    [SerializeField]
    private float fullDayLength = 300f; // Länge eines vollen Tages in Sekunden
    public float currentTime = 50f; // aktuelle Zeit (0 - 100)

    public GameObject feuer;

    private void Update()
    {
        //Debug.Log(currentTime);
        // Zeit aktualisieren
        currentTime += (100 / fullDayLength) * Time.deltaTime;
        if (currentTime >= 95)
        {
            currentTime = 10;
        }
        feuer.SetActive(currentTime < 25 || currentTime > 70);
        // Berechne den Rotationswinkel der Sonne basierend auf der aktuellen Zeit
        float sunAngle = currentTime / 100 * 360f - 90f;
        if (sunAngle < 0)
        {
            sunAngle = 360f + sunAngle;
        }
        sun.transform.rotation = Quaternion.Euler(new Vector3(sunAngle, 0f, 0f));

        if (currentTime < 15 || currentTime > 82) // Nacht
        {
            sun.intensity = 0f;
        }
        else if (currentTime >= 15 && currentTime < 25) // Morgen
        {
            sun.intensity = Mathf.Lerp(0f, 1.5f, (currentTime - 15) / 10f);
        }
        else if (currentTime >= 25 && currentTime < 75) // Tag
        {
            sun.intensity = 1.5f;
        }
        else if (currentTime >= 72 && currentTime < 82) // Abend
        {
            sun.intensity = Mathf.Lerp(1.5f, 0f, (currentTime - 72) / 10f);
        }
    }
}
