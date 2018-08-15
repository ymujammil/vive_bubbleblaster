using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Video;

public class PlayerScript_Gaze : MonoBehaviour {
    Vector3 StandardViewPoint = new Vector3(0.5f, 0.5f, 10f);

    Camera SceneCamera;
    GameController controller;
    BubbleScript bubble;
    VideoPlayer vplayer; 
    
    //GameObject marker;

    private Vector2 GazeCenter;
    private float GazeTime1;
    private float GazeTime2;
    private double blink_onset;
    private double blink_offset;

    // Use this for initialization
    void Start () {
        SceneCamera = Camera.main;
        controller = GameObject.FindObjectOfType<Terrain>().GetComponent<GameController>();
        vplayer = GameObject.FindGameObjectWithTag("Player").GetComponent<VideoPlayer>();
    }
    void OnEnable() {
        if (PupilTools.IsConnected)
        {
            PupilTools.IsGazing = true;
            PupilTools.SubscribeTo("gaze");
        }
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 ViewPoint = StandardViewPoint;
        if (PupilTools.IsConnected && PupilTools.IsGazing) {
            //marker = GameObject.Find("Gaze_3D");
            GazeCenter = PupilData._2D.GazePosition;
            ViewPoint = new Vector3(GazeCenter.x, GazeCenter.y, 1f);
        }
        if (controller.PLAYING) {
            Ray gazeray = SceneCamera.ViewportPointToRay(ViewPoint);
            //Ray gaze3d = SceneCamera.ScreenPointToRay(marker.transform.position);
            //Debug.DrawRay(gaze3d.origin, gaze3d.direction, Color.green);    
            Debug.Log("blink = " + checkIfBlink());
            RaycastHit hit;
            if (Physics.Raycast(gazeray, out hit))
            {
                if (hit.collider.CompareTag("Bubble")) {
                    GazeTime1 += Time.deltaTime;
                    if (GazeTime1 > 0.5f)
                    {
                        bubble = hit.collider.gameObject.GetComponent<BubbleScript>();
                        bubble.SendMessage("Pop");
                        GazeTime1 = 0.0f;
                    }
                }

                if (hit.collider.name == "Screen") {
                    GazeTime2 += Time.deltaTime;
                    if (GazeTime2 > 0.5f)
                    {
                        vplayer.Play();
                    }
                }
            }
            else {
                GazeTime1 = 0.0f;
                GazeTime2 = 0.0f;
                vplayer.Pause();
            }
        }
	}

    bool checkIfBlink() {
        bool blink = false;
        if (PupilTools.blinkDictionary != null)
        {            
            if ((string)PupilTools.blinkDictionary["type"] == "onset")
            {
                blink_onset = (double)PupilTools.blinkDictionary["timestamp"];
            }
            else
            {
                blink_offset = (double)PupilTools.blinkDictionary["timestamp"];
            }
            //Debug.Log("difference " + Math.Abs(blink_offset - blink_onset));
            if (Math.Abs(blink_offset - blink_onset) < 0.5)
            {
                //Debug.Log("Actual blink");
                blink = true;
                blink_onset = 0;
                blink_offset = 0;
            }
            //Debug.Log(PupilTools.blinkDictionary["timestamp"] + " " + PupilTools.blinkDictionary["type"] + " " + Math.Abs(blink_offset - blink_onset));

            PupilTools.blinkDictionary = null;
        }
        return blink;
    }
}
