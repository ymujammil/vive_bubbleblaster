﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameController : MonoBehaviour {

    int MIN = -15;
    int MAX = 15;

    int SCORE = 0;
    float TIMER = 0.0f;

    public bool PLAYING = true;

    public GameObject originalBubble;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        if(PLAYING)
        {
            TIMER += Time.deltaTime;
            DisplayTimeAndScore();

            if (SCORE >= 20)
            {
                EndGame();
            }
        }
        
    }

    // Display the time
    void DisplayTimeAndScore()
    {
        string highscore = (PlayerPrefs.HasKey("BestTime") ? FormatTimer(PlayerPrefs.GetFloat("BestTime")) : "Not Set");
        GameObject.FindGameObjectWithTag("Sign1").GetComponent<Text>().text = "Time: " + FormatTimer(TIMER)
                                                                                      + System.Environment.NewLine
                                                                                      + "Bubbles: " + SCORE + "/20";
        GameObject.FindGameObjectWithTag("Sign2").GetComponent<Text>().text = "High Score: " +  highscore;
    }

    // Helper method convert the timer to minute/second format
    string FormatTimer(float TIMER)
    {
        if(TIMER <= 60.0f)
        {
            return (Mathf.Round(TIMER *100) / 100).ToString();
        }
        else
        {
            int displayTimeMin = (int)(TIMER / 60.0f);
            float displayTimeSec = Mathf.Round((TIMER % 60) * 10 / 10);
            if(displayTimeSec < 10)
            {
                return displayTimeMin.ToString() + ":0" + displayTimeSec.ToString(); 
            }
            else
            {
                return displayTimeMin.ToString() + ":" + displayTimeSec.ToString();
            }
        }
    }

    // Generate a new bubble
    public void NewBubble()
    {
        SCORE++;

        float xVal = Random.Range(MIN, MAX);
        float yVal = Random.Range(2.0f, 5.0f);
        float zVal = Random.Range(MIN, MAX);

        Vector3 bubblePos = new Vector3(xVal, yVal, zVal);
        Instantiate(originalBubble, bubblePos, originalBubble.transform.rotation);
    }

    // End game, save score
    void EndGame()
    {
        PLAYING = false;
        if (!PlayerPrefs.HasKey("BestTime") || TIMER < PlayerPrefs.GetFloat("BestTime"))
        {
            PlayerPrefs.SetFloat("BestTime", TIMER);
            GameObject.FindGameObjectWithTag("Sign1").GetComponent<Text>().text = "Time: " + FormatTimer(TIMER)
                                                                              + System.Environment.NewLine
                                                                              + "New High Score!";
        }
    }
}
