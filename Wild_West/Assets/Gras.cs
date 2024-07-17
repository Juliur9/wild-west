using UnityEngine;

public class Gras : MonoBehaviour
{
    public bool withSchlange;

    private void Awake()
    {
        withSchlange = Random.value < 0.2;
    }
}
