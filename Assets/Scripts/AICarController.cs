using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class AICarController : MonoBehaviour
{
    public float maxSpeed = 10f;
    public float accelerationTime = 0.1f;
    public float rotationSpeed = 5f;
    public float detectionDistance = 5f;
    public TextMeshProUGUI lapTimerText; // Reference to the lap timer TextMeshProUGUI
    public Transform finishLine; // Reference to the finish line Transform

    private List<Transform> waypoints = new List<Transform>();
    private bool controlsEnabled = true;
    private float currentSpeed = 0f;
    private float targetSpeed = 0f;
    private Rigidbody rb;
    private Quaternion initialRotation;
    private float maxAngle = 80f;
    private float flipDuration = 3f;
    private float timeSinceLastFlip = 0f;

    private int waypointCounter = 0;
    private int lapCount = 0;
    private float lapTimer = 0f;
    private bool lapTimerEnabled = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        initialRotation = rb.rotation;

        GameObject[] waypointObjects = GameObject.FindGameObjectsWithTag("Waypoint");
        foreach (var waypointObject in waypointObjects)
        {
            waypoints.Add(waypointObject.transform);
        }

        if (waypoints.Count > 0)
        {
            targetSpeed = maxSpeed;
            controlsEnabled = true;
            lapTimerEnabled = false; // Disable lap timer initially
        }
        else
        {
            Debug.LogError("No waypoints found. Please ensure waypoints are properly tagged.");
            controlsEnabled = false;
        }
    }

    private void FixedUpdate()
    {
        if (controlsEnabled)
        {
            float distance = Vector3.Distance(transform.position, waypoints[waypointCounter].position);

            Vector3 targetDirection = waypoints[waypointCounter].position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            float acceleration = maxSpeed / accelerationTime;
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);

            rb.MovePosition(rb.position + transform.forward * currentSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));

            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, detectionDistance))
            {
                if (hit.collider.transform == finishLine && waypointCounter == waypoints.Count - 1)
                {
                    if (!lapTimerEnabled)
                    {
                        lapTimerEnabled = true;
                        lapTimer = 0f; // Reset lap timer when crossing the finish line
                        lapCount++;

                        if (lapCount >= 3)
                        {
                            // Stop the car's movement after completing 3 laps
                            controlsEnabled = false;
                            targetSpeed = 0f;
                        }
                    }
                }
            }
        }
        else
        {
            currentSpeed = 0f;
        }

        if (Vector3.Dot(transform.up, Vector3.down) > 0f)
        {
            timeSinceLastFlip += Time.fixedDeltaTime;

            if (timeSinceLastFlip > flipDuration)
            {
                rb.rotation = initialRotation;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                timeSinceLastFlip = 0f;
            }
            else
            {
                float angle = Vector3.Angle(transform.up, Vector3.up);
                float ratio = Mathf.Clamp01(angle / maxAngle);
                Quaternion targetRotation = Quaternion.Slerp(rb.rotation, initialRotation, ratio);
                rb.MoveRotation(targetRotation);
            }
        }
        else
        {
            timeSinceLastFlip = 0f;
        }

        if (lapTimerEnabled)
        {
            lapTimer += Time.fixedDeltaTime;
            UpdateLapTimerText();
        }

    }

        private void UpdateLapTimerText()
    {
        if (lapTimerText != null)
        {
            lapTimerText.text = lapTimer.ToString("F2");
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Waypoint"))
        {
            int waypointIndex = int.Parse(other.gameObject.name.Split(' ')[1]) - 1;

            if (waypointCounter == waypointIndex)
            {
                waypointCounter++;

                if (waypointCounter >= waypoints.Count)
                {
                    waypointCounter = 0;

                    if (lapTimerEnabled)
                    {
                        lapTimerEnabled = false;
                        Debug.Log("Lap " + lapCount + ": " + lapTimer.ToString("F2") + "s");
                    }
                }
            }
        }
    }

    private void DodgeObject(GameObject objToDodge)
    {
        Vector3 dodgeDirection = transform.right * (Random.value < 0.5f ? -1f : 1f);
        rb.AddForce(dodgeDirection * accelerationTime, ForceMode.Acceleration);
    }

    public void AICarControllerEnableControl()
    {
        controlsEnabled = true;
        targetSpeed = maxSpeed;
    }

    public void AICarControllerDisableControl()
    {
        controlsEnabled = false;
        targetSpeed = 0f;
    }
}