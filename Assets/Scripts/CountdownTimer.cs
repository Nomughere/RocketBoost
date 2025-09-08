    using UnityEngine;
    using TMPro; // Required for TextMeshPro
    
    public class CountdownTimer : MonoBehaviour
    {
        public float totalTime = 60f; // Set your desired countdown time in seconds
        public TextMeshProUGUI timerText; // Assign your UI TextMeshPro element here
    
        void Update()
        {
            if (totalTime > 0)
            {
                totalTime -= Time.deltaTime;
                DisplayTime(totalTime);
            }
            else
            {
                totalTime = 0;
                DisplayTime(totalTime);
                // Trigger game over, level end, or other events here
                Debug.Log("Time's Up!");
                // For example, disable the timer script after countdown finishes
                enabled = false; 
            }
        }
    
        void DisplayTime(float timeToDisplay)
        {
            if (timeToDisplay < 0)
            {
                timeToDisplay = 0;
            }
    
            float minutes = Mathf.FloorToInt(timeToDisplay / 60);
            float seconds = Mathf.FloorToInt(timeToDisplay % 60);
    
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }