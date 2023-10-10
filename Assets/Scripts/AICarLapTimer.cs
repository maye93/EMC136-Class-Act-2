using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AICarLapTimer : MonoBehaviour
{
    public TextMeshProUGUI lapTimerText;
    public Transform finishLine;

    private int lapCount = 0;
    private float lapTimer = 0f;
    private bool raceFinished = false;

    private void Start()
    {
        lapTimer = 0f;
        lapTimerText.text = "Lap " + (lapCount + 1) + ": " + lapTimer.ToString("F2");
        lapTimer += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider belongs to the finish line and the race is not finished
        if (finishLine != null && other.transform == finishLine && !raceFinished)
        {
            // Increment lap count
            lapCount++;

            // Display lap information
            if (lapCount <= 3)
            {
                Debug.Log("Lap " + lapCount + ": " + lapTimer.ToString("F2"));
                UpdateLapTimerText();
            }

            // Check if all laps are completed
            if (lapCount >= 3)
            {
                Debug.Log("Race finished!");
                raceFinished = true;
                StopMovement();
            }

            // Reset lap timer for the next lap
            lapTimer = 0f;
        }
    }

    private void Update()
    {
        if (!raceFinished)
        {
            lapTimer += Time.deltaTime;
        }
    }

    private void UpdateLapTimerText()
    {
        if (lapTimerText != null)
        {
            lapTimerText.text = "Lap " + lapCount + ": " + lapTimer.ToString("F2");
        }
    }

    private void StopMovement()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
}