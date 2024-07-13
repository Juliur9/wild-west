using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cinemachine;

public class PlayerLife : MonoBehaviour
{   
    public CinemachinePOVExtension povExtension;
    private int todescounter = 0;
    public Slider hunger, durst;

    public GameObject gameScreen, deathScreen;
    public TMP_Text todesgrundText, todcounterText, restartText;
    [HideInInspector] public bool playerAlive;
    public Vector3 startPosition, cameraRotation;

    private void OnControllerColliderHit(ControllerColliderHit col)
    {   
        if (!playerAlive) return;
        if (col.gameObject.tag == "Kaktus") {
            Kill("Ein Kaktus hat dich getötet.");
        }
        if (col.gameObject.tag == "Pfahl") {
            Kill("Du hast dich dazu entschieden, dein Leben am Marterpfahl zu beenden!");
        }
    }
    private void Update() {
        if (!playerAlive) return;
        if (hunger.value <= 1) {
            Kill("Du bist verhungert");
        }
        if (durst.value <= 1) {
            Kill("Du bist verdurstet");
        }
    }
    public void Kill(string todesgrund) {
        playerAlive = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        todescounter++;
        deathScreen.SetActive(true);
        gameScreen.SetActive(false);
        todesgrundText.text = todesgrund;
        if (todescounter == 1) {
            restartText.text = "Willst du es gleich nochmal versuchen?";
            todcounterText.text = "";
        } else {
            restartText.text = "Willst du es trotzdem nochmal versuchen?";
            Debug.Log(todescounter);
            todcounterText.text = "Das war jetzt schon dein " + todescounter + ". Tod!";
        }
    }

    public void StartAgain() {
        playerAlive = true;
        deathScreen.SetActive(false);
        gameScreen.SetActive(true);
        transform.position = startPosition;
        povExtension.startingRotation = cameraRotation;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void QuitGame() {
        Application.Quit();
    }
}