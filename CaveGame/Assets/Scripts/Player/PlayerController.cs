using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //private PlayerInputs playerInputs;
    //private PlayerInput playerInput;
    private CharacterController characterController;
    private Camera playerCamera;


    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float sensitivity = 0.1f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float sprintSpeedMultiplier = 2f;

    private Vector3 playerVelocity;
    private Vector3 playerRotation;
    private bool isRunning = false;

    private void Awake()
    {
        //playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();
        playerCamera = FindFirstObjectByType<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnSprint(InputValue inputValue)
    {
        isRunning = inputValue.isPressed;
    }

    private void OnMove(InputValue inputValue)
    {
        Vector3 inputVector = inputValue.Get<Vector3>();
        playerVelocity.x = inputVector.x;
        playerVelocity.z = inputVector.z;
    }

    private void OnLook(InputValue inputValue)
    {
        Vector2 mouseMovement = inputValue.Get<Vector2>();
        playerRotation.y += mouseMovement.x * sensitivity;
        playerRotation.x += -mouseMovement.y * sensitivity;
        playerRotation.x = Mathf.Clamp(playerRotation.x, -90f, 90f);
    }

    // Update is called once per frame
    void Update()
    {
        if (characterController.isGrounded && playerVelocity.y < 0) playerVelocity.y = -2f;
        playerVelocity.y += gravity * Time.deltaTime;
        transform.rotation = Quaternion.Euler(playerRotation);
        transform.rotation = Quaternion.Euler(0f, playerRotation.y, 0f);
        Vector3 movement = (isRunning ? sprintSpeedMultiplier : 1f) * moveSpeed * (transform.right * playerVelocity.x + transform.forward * playerVelocity.z);
        movement.y = playerVelocity.y;
        playerCamera.transform.localRotation = Quaternion.Euler(playerRotation.x, 0f, 0f);
        characterController.Move(movement * Time.deltaTime);
    }

    public bool IsMoving()
    {
        return playerVelocity.x != 0 || playerVelocity.z != 0;
    }

    public bool IsRunning()
    {
        return IsMoving() && isRunning;
    }
}
