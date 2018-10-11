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
    GameObject FastForward;
    GameObject Rewind;

    private IEnumerator coroutine;

    private Vector2 GazeCenter;
    private float GazeTime1;
    private float GazeTime2;
    private float GazeTime3;
    private float GazeTime4;
    private double blink_onset;
    private double blink_offset;

    // Use this for initialization
    void Start () {
        SceneCamera = Camera.main;
        controller = GameObject.FindObjectOfType<Terrain>().GetComponent<GameController>();
        vplayer = GameObject.FindGameObjectWithTag("Player").GetComponent<VideoPlayer>();
        FastForward = GameObject.Find("fast-forward");
        Rewind = GameObject.Find("rewind");
        //FastForward.SetActive(false);
        //Rewind.SetActive(false);
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
        CheckIfBlink();
        Vector3 ViewPoint = StandardViewPoint;
        if (PupilTools.IsConnected && PupilTools.IsGazing) {
            GazeCenter = PupilData._2D.GazePosition;
            ViewPoint = new Vector3(GazeCenter.x, GazeCenter.y, 1f);
        }
            Ray gazeray = SceneCamera.ViewportPointToRay(ViewPoint);
            Debug.DrawRay(gazeray.origin, gazeray.direction, Color.green);    
            //Debug.Log("blink = " + CheckIfBlink());
            RaycastHit hit;
            if (Physics.Raycast(gazeray, out hit))
            {                
                if (controller.PLAYING && hit.collider.CompareTag("Bubble")) {
                    GazeTime1 += Time.deltaTime;
                    if (GazeTime1 > 0.5f)
                    {
                        bubble = hit.collider.gameObject.GetComponent<BubbleScript>();
                        bubble.SendMessage("Pop");
                        GazeTime1 = 0.0f;
                    }
                }

                if (hit.collider.name == "Screen")
                {
                    //float rightR = (hit.collider.transform.position.x - (hit.collider.bounds.size.x / 2));
                    //float leftR = (hit.collider.transform.position.x + (hit.collider.bounds.size.x / 2));
                    //double timeSinceLastBlink = PupilTools.blink_timestamp - PupilTools.blink_timestamp_recent;
                    //Debug.Log("The range from " + leftR + " to " + rightR);
                    ////Debug.Log("Hit Point x" + hit.point.x);
                    //if (hit.point.x > (leftR - (0.10 * hit.collider.bounds.size.x)) && timeSinceLastBlink < 2)
                    //{ //left 
                    //    Debug.Log("Seek backward");
                    //    Rewind.SetActive(true);
                    //    coroutine = WaitAndHide(Rewind);
                    //    StartCoroutine(coroutine);
                    //    PupilTools.blink_timestamp_recent = -10000;
                    //    vplayer.time = vplayer.time - 10;
                    //}
                    //if (hit.point.x < (rightR + (0.10 * hit.collider.bounds.size.x)) && timeSinceLastBlink < 2)
                    //{ // right
                    //    Debug.Log("Seek forward");
                    //    FastForward.SetActive(true);
                    //    coroutine = WaitAndHide(FastForward);
                    //    StartCoroutine(coroutine);
                    //    PupilTools.blink_timestamp_recent = -10000;
                    //    vplayer.time = vplayer.time + 10;
                    //}

                    GazeTime2 += Time.deltaTime;
                    if (GazeTime2 > 0.5f)
                    {
                        vplayer.Play();
                    }
                }
                else {
                    GazeTime2 = 0.0f;
                    vplayer.Pause();           
                }
                if (hit.collider.name == "rewind") {
                    GazeTime3 += Time.deltaTime;
                    if (GazeTime3 > 2f)
                    {
                        vplayer.time = vplayer.time - 10;
                        GazeTime3 = 0.0f;
                    }

                }
                else GazeTime3 = 0.0f;
                if (hit.collider.name == "fast-forward") {
                    GazeTime4 += Time.deltaTime;
                    if (GazeTime4 > 2f)
                    {
                        vplayer.time = vplayer.time + 10;
                        GazeTime4 = 0.0f;
                    }
                }
                else GazeTime4 = 0.0f;

        }
            else {
                GazeTime1 = 0.0f;
            }
	}

    bool CheckIfBlink() {
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
            //Debug.Log("difference " + Math.Abs(blink_offset - blink_onset))
            if (Math.Abs(blink_offset - blink_onset) < 0.5)
            {
                PupilTools.blink_timestamp = (double)PupilTools.blinkDictionary["timestamp"];
                PupilTools.blink_timestamp_recent = (double)PupilTools.blinkDictionary["timestamp"];
                blink = true;
                blink_onset = 0;
                blink_offset = 0;
            }
            //Debug.Log(PupilTools.blinkDictionary["timestamp"] + " " + PupilTools.blinkDictionary["type"] + " " + Math.Abs(blink_offset - blink_onset));

            PupilTools.blinkDictionary = null;
        }
        return blink;
    }

    IEnumerator WaitAndHide(GameObject icon)
    {
        yield return (new WaitForSeconds(2));
        icon.SetActive(false);
    }
}
