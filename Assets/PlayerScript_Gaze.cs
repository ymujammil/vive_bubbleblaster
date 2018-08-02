using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript_Gaze : MonoBehaviour {
    Vector3 StandardViewPoint = new Vector3(0.5f, 0.5f, 10f);

    Camera SceneCamera;
    GameController controller;
    BubbleScript bubble;
    //GameObject marker;

    private Vector2 GazeCenter;
    private float GazeTime;

    // Use this for initialization
    void Start () {
        SceneCamera = Camera.main;
        controller = GameObject.FindObjectOfType<Terrain>().GetComponent<GameController>();
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
            RaycastHit hit;
            if (Physics.Raycast(gazeray, out hit) && hit.collider.CompareTag("Bubble"))
            {
                GazeTime += Time.deltaTime;
                if (GazeTime > 1.0f) {
                    bubble = hit.collider.gameObject.GetComponent<BubbleScript>();
                    bubble.SendMessage("Pop");
                    GazeTime = 0.0f;
                }
            }
            else {
                GazeTime = 0.0f;
            }
        }
	}
}
