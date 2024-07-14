using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerControler : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private InputManager inputManager;
    private Transform cameraTransform;

    [SerializeField]
    private float playerSpeed = 2.0f;

    [SerializeField]
    private float jumpHeight = 1.0f;

    [SerializeField]
    private float gravityValue = -9.81f;
    private bool isGrounded;
    private PlayerLife playerLife;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        playerLife = GetComponent<PlayerLife>();
        playerLife.StartAgain();
        inputManager = InputManager.Instance;
        cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        if (!playerLife)
            return;
        isGrounded = IsGrounded();

        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector2 movement = inputManager.GetPlayerMovement();
        if (movement != new Vector2(0f, 0f))
        {
            playerLife.hunger.value -= Time.deltaTime * 0.3f;
        }
        Vector3 move = new Vector3(movement.x, 0f, movement.y);
        move = cameraTransform.forward * move.z + cameraTransform.right * move.x;
        move.y = 0f;
        controller.Move(move * Time.deltaTime * playerSpeed);

        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
        }

        // Apply jump if the player jumps and is grounded
        if (inputManager.PlayerJumpedThisFrame() && isGrounded)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -2f * gravityValue);
            playerLife.hunger.value -= 1;
        }

        // Apply gravity
        playerVelocity.y += gravityValue * Time.deltaTime;

        // Move the player with the computed velocity
        controller.Move(playerVelocity * Time.deltaTime);
    }

    bool IsGrounded()
    {
        RaycastHit hit;
        return Physics.Raycast(transform.position, Vector3.down, out hit, 1.6f);
    }
}
