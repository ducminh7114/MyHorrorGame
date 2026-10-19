
using UnityEngine;

[DefaultExecutionOrder(10000)]
[RequireComponent(typeof(CharacterController))]
public class ElevatorPlayerFollow : MonoBehaviour
{
    [Header("Elevator")]
    public Transform elevatorRoot;

    [Header("Settings")]
    public bool followElevator = true;

    private CharacterController controller;
    private bool isRiding;
    private Vector3 previousPosition;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (elevatorRoot == null)
            return;

        if (other.transform == elevatorRoot ||
            other.transform.IsChildOf(elevatorRoot))
        {
            StartRiding();
        }
    }

    public void StartRiding()
    {
        if (elevatorRoot == null || isRiding)
            return;

        isRiding = true;
        previousPosition = elevatorRoot.position;
    }

    public void StopRiding()
    {
        isRiding = false;
    }

    void LateUpdate()
    {
        if (!followElevator || !isRiding || elevatorRoot == null)
            return;

        Vector3 currentPosition = elevatorRoot.position;
        Vector3 delta = currentPosition - previousPosition;

        if (delta.sqrMagnitude > 0.000001f)
        {
            controller.Move(delta);
        }

        previousPosition = currentPosition;
    }

    void OnDisable()
    {
        isRiding = false;
    }
}
