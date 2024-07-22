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
    public bool playerAlive = false;
    public Vector3 startPosition,
        cameraRotation;

    private float wärmeZeit,
        hitzezeit;

    [SerializeField]
    private AudioSource backgroundMusic;

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
        if (!playerAlive)
            return;

        if (col.gameObject.CompareTag("Gras"))
        {
            if (col.gameObject.GetComponent<Gras>().withSchlange)
            {
                Kill("Eine Schlange hat im hohen Gras gelauert, um dich mit Gift zu töten!");
            }
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
        if (!playerAlive)
            return;

        if (col.gameObject.CompareTag("Wasser"))
        {
            if (durst.value < 100)
            {
                durst.value += Time.deltaTime * 4;
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

    private void Update()
    {
        backgroundMusic.enabled = playerAlive;

        if (!playerAlive)
            return;

        durst.value -= Time.deltaTime * 0.2f;
        hunger.value -= Time.deltaTime * 0.2f;
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
            durst.value -= Time.deltaTime * 0.2f;
            hitzezeit += Time.deltaTime * 3;
        }
        else
        {
            hitzezeit -= Time.deltaTime * 5;
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
        if (wärmeZeit < -150)
        {
            Kill("Du bist erfroren!");
        }
        if (hitzezeit > 250)
        {
            Kill("Du bist an einer Hitzeerschöpfung gestorben!");
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
        CharacterController characterController = GetComponent<CharacterController>();
        characterController.enabled = false;
        transform.position = startPosition;
        characterController.enabled = true;
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
        povExtension.startingRotation = cameraRotation;

        hunger.value = 90;
        durst.value = 90;

        GetComponent<DayNightCycle>().currentTime = 50f;

        wärmeZeit = 30f;
        hitzezeit = 30f;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        StartAgain();
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
            ~playerlayerMask,
            QueryTriggerInteraction.Ignore
        );
    }
}
