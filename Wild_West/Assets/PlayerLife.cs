using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLife : MonoBehaviour
{
    public CinemachinePOVExtension povExtension;
    public Light lightSource;
    public LayerMask playerlayerMask;
    private int todescounter = 0;
    public Slider hunger,
        durst;

    public GameObject gameScreen,
        deathScreen;
    public TMP_Text todesgrundText,
        todcounterText,
        restartText;

    [HideInInspector]
    public bool playerAlive;
    public Vector3 startPosition,
        cameraRotation;

    private bool TouchingGras = false;
    private float wärmeZeit;

    private void OnControllerColliderHit(ControllerColliderHit col)
    {
        if (!playerAlive)
            return;
        if (col.gameObject.CompareTag("Kaktus"))
        {
            Kill("Ein Kaktus hat dich getötet.");
        }
        if (col.gameObject.CompareTag("Pfahl"))
        {
            Kill("Du hast dich dazu entschieden, dein Leben am Marterpfahl zu beenden!");
        }
        if (col.gameObject.CompareTag("Pferd"))
        {
            Kill(
                "Dieses Pferd mag dich anscheinend nicht so. Mit seinen Beinen brachte es dich zum Tod!"
            );
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Gras"))
        {
            if (Random.value <= 0.15f)
            {
                if (!TouchingGras)
                {
                    Kill("Eine Schlange hat im hohen Gras gelauert, um dich mit Gift zu töten!");
                }
            }
            TouchingGras = true;
        }

        if (col.gameObject.CompareTag("Feuer"))
        {
            Kill("Die Flammen des Feuers brachten dir den qualvollen Tod!");
        }

        if (col.gameObject.CompareTag("Pferd"))
        {
            Kill(
                "Dieses Pferd mag dich anscheinend nicht so. Mit seinen Beinen brachte es dich um!"
            );
        }
        if (col.gameObject.CompareTag("SEssen"))
        {
            Kill("Diese Mahlzeit stand zu lange in der Sonne, dadurch wurde es ungenießbar!");
        }
    }

    private void OnTriggerStay(Collider col)
    {
        if (col.gameObject.CompareTag("Wasser"))
        {
            if (durst.value < 100)
            {
                durst.value += Time.deltaTime * 2;
            }
            else
            {
                durst.value = 100;
            }
        }
        if (col.gameObject.CompareTag("Essen"))
        {
            if (hunger.value < 100)
            {
                hunger.value += Time.deltaTime * 3;
            }
            else
            {
                hunger.value = 100;
            }
        }
        if (
            col.gameObject.CompareTag("Wärme")
            && (
                GetComponent<DayNightCycle>().currentTime < 25
                || GetComponent<DayNightCycle>().currentTime > 70
            )
        )
        {
            wärmeZeit += Time.deltaTime * 6;
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.CompareTag("Gras"))
        {
            TouchingGras = false;
        }
    }

    private void Update()
    {
        if (!playerAlive)
            return;
        if (hunger.value <= 1)
        {
            Kill("Du bist verhungert");
        }
        if (durst.value <= 1)
        {
            Kill("Du bist verdurstet");
        }
        if (!InSchatten())
        {
            durst.value -= Time.deltaTime * 0.5f;
        }
        if (
            GetComponent<DayNightCycle>().currentTime < 25
            || GetComponent<DayNightCycle>().currentTime > 70
        )
        {
            wärmeZeit -= Time.deltaTime * 3;
        }
        if (
            GetComponent<DayNightCycle>().currentTime > 65
            && GetComponent<DayNightCycle>().currentTime < 70
        )
        {
            wärmeZeit = 30f;
        }
        if (wärmeZeit < -70)
        {
            Kill("Du bist erfroren!");
        }
    }

    public void Kill(string todesgrund)
    {
        playerAlive = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        todescounter++;
        deathScreen.SetActive(true);
        gameScreen.SetActive(false);
        todesgrundText.text = todesgrund;
        if (todescounter == 1)
        {
            restartText.text = "Willst du es gleich nochmal versuchen?";
            todcounterText.text = "";
        }
        else
        {
            restartText.text = "Willst du es trotzdem nochmal versuchen?";
            todcounterText.text = "Das war jetzt schon dein " + todescounter + ". Tod!";
        }
    }

    public void StartAgain()
    {
        playerAlive = true;
        deathScreen.SetActive(false);
        gameScreen.SetActive(true);
        CharacterController characterController = GetComponent<CharacterController>();
        characterController.enabled = false;
        transform.position = startPosition;
        characterController.enabled = true;
        TouchingGras = false;
        povExtension.startingRotation = cameraRotation;
        hunger.value = 90;
        durst.value = 90;
        GetComponent<DayNightCycle>().currentTime = 50f;
        wärmeZeit = 30f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public bool InSchatten()
    {
        Vector3 playerGroundPosition = new Vector3(
            transform.position.x,
            transform.position.y + 0.8f,
            transform.position.z
        );

        Vector3 directionToLight = -lightSource.transform.forward;

        return Physics.Raycast(
            playerGroundPosition,
            directionToLight,
            out RaycastHit hit,
            100f,
            ~playerlayerMask
        );
    }
}
