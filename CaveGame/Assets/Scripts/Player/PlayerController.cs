using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
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
    private bool isTuning = false;

    //Made a hash set to easily add and remove objectives
    private HashSet<ObjectiveItem> objectivesInRange;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerCamera = FindFirstObjectByType<Camera>();
        objectivesInRange = new HashSet<ObjectiveItem>();
        ObjectiveItem.OnObjeciveRangeEnter += AddObjective;
        ObjectiveItem.OnObjectiveRangeExit += RemoveObjective;
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

    private void OnTuneRadio(InputValue inputValue)
    {
        isTuning = inputValue.isPressed;
        if (isTuning) Debug.Log("Tuning radio");
        else Debug.Log("Stopped tuning radio");
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
        if (isTuning && objectivesInRange.Count > 0) ScanForObjectives();
    }

    private void MovePlayer()
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

    private void OnDisable()
    {
        ObjectiveItem.OnObjeciveRangeEnter -= AddObjective;
        ObjectiveItem.OnObjectiveRangeExit -= RemoveObjective;
    }

    private void AddObjective(ObjectiveItem objective)
    {
        objectivesInRange.Add(objective);
        Debug.Log($"Objective in range; {objectivesInRange.Count} objectives in range");
    }

    private void RemoveObjective(ObjectiveItem objective)
    {
        objectivesInRange.Remove(objective);
        Debug.Log($"Objective out of range; {objectivesInRange.Count} objectives in range");
    }

    private void ScanForObjectives()
    {
        float mostDirectDot = 0f;
        ObjectiveItem mostDirectObjective;
        foreach(ObjectiveItem objective in objectivesInRange)
        {
            Vector3 directionToObjective = objective.transform.position - transform.position;
            directionToObjective.Normalize();
            float currentObjectiveDotProduct = Vector3.Dot(directionToObjective, transform.forward);
            if(currentObjectiveDotProduct > mostDirectDot)
            {
                mostDirectDot = currentObjectiveDotProduct;
                mostDirectObjective = objective;
            }
        }
        Debug.Log($"Closeness to objective: {mostDirectDot}");
    }
}
