using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LapTimer : MonoBehaviour
{
    public TextMeshProUGUI lapTimerText;
    private float lapTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        lapTimer += Time.fixedDeltaTime;
    }

            private void UpdateLapTimerText()
    {
        if (lapTimerText != null)
        {
            lapTimerText.text = "Lap Time: " + lapTimer.ToString("F2");
        }
    }
}
