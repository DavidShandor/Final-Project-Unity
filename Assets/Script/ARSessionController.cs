using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System;
using System.IO;

//[RequireComponent(typeof(ARRaycastManager))]
public class ARSessionController : MonoBehaviour
{
    // Initial Even, Game objects and other Components.
    public event Action<string> OnChangeText;

    [SerializeField] private GameObject measurementPointPrefab, Door, DoorIndicator;

    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private Pose placementPose;
    private Vector2 touchPosition = default;
    private GameObject startPoint, endPoint;

    private LineRenderer measureLine;
    private ButtonsController Control;
    private ARRaycastManager arOrigin;

    private bool doorHasPlaced = false;
    private bool placementPoseIsValid = false;

    private List<ScanToSave> ScanList = new List<ScanToSave>();
    private int counter = 0;

    // This Function called once by Unity when the Appliction start before any other functions.
    void Awake()
    {
        // Get the AR Ray Cast Manager component. This component required to raycast against
        // trackables (i.e., detected features in the physical environment) when they do not
        // have a presence in the Physics world.
        arOrigin = GetComponent<ARRaycastManager>();
        
        //var MeasurePointRendered = measurementPointPrefab.GetComponent<Renderer>();
        //MeasurePointRendered.material.SetColor("_Color", Color.blue);

        //startPoint = Instantiate(measurementPointPrefab, Vector3.zero, Quaternion.identity);
        //endPoint = Instantiate(measurementPointPrefab, Vector3.zero, Quaternion.identity);

        //startPoint.SetActive(false);
        //endPoint.SetActive(false);

        //measureLine = GetComponent<LineRenderer>();
        //measureLine.gameObject.SetActive(false);
    }

    // This Function Called when the script is enable by the application.
    private void OnEnable()
    {
       
        // Get the Buttons Controller object reference.
        Control = FindObjectOfType<ButtonsController>();

        // Set listenners to the events from the Button Controller.
        Control.OnDoorPress += Control_OnDoorPress;
        Control.OnSavePress += Control_OnSavePress;
        Control.OnLoadPress += Control_OnLoadPress;
        Control.OnPlacePress += Control_OnPlacePress;

        // Validate Game Object to ensure they are ready to use.
        if (measurementPointPrefab == null)
        {
            Debug.LogError("measurementPointPrefab must be set");
            enabled = false;
        }
      
    }

 
    // This Function called for every frame recieved from the camera.
    void Update()
    {
        //if (!doorHasPlaced)
        //{
        UpdatePlacementPose();
        UpdatePlacementIndicator();
        //}
        //else
        //{
        //    updateline();
        //}

    }

    // This function called with the script disable by the application.
    private void OnDisable()
    {
        // Remove the listenners.
        Control.OnDoorPress -= Control_OnDoorPress;
        Control.OnSavePress -= Control_OnSavePress;
        Control.OnLoadPress -= Control_OnLoadPress;
    }

    // This function called when the user press "load".
    // The function Load the saved scan's data from the phone. 
    private void Control_OnLoadPress()
    {
        Debug.Log("Start Loading Data");
        List<ScanToSave> temp = new List<ScanToSave>();

        if (Directory.Exists(Application.persistentDataPath + "/Scans"))
        {
            String[] info = Directory.GetFiles(Application.persistentDataPath + "/Scans", "*");//"*.scantosave", check this pattern!

            Debug.Log("start convrt to scan");
            Debug.Log("info len is: " + info.Length);
            foreach (string str in info)
            {
                Debug.Log(str);
                temp.Add(JsonUtility.FromJson<ScanToSave>(File.ReadAllText(str)));
            }

            Debug.Log("Start Spawn");
            foreach (ScanToSave s in temp)
            {
                Debug.Log(s.ToString());
                Instantiate(measurementPointPrefab, s.ScanPose.position, Quaternion.identity);
            }

            ScanList = temp;
        }

        Debug.Log("Load had Done!");
    }

    // This function called when the user press "Save".
    // The function save the scan's data to the phone. 
    private void Control_OnSavePress()
    {
        Debug.Log("Start Saving Data");

        if (!Directory.Exists(Application.persistentDataPath + "/Scans"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/Scans");
        }

        foreach (ScanToSave s in ScanList)
        {
            File.WriteAllText(Application.persistentDataPath + "/Scans/" + s.PipeType.ToString(), JsonUtility.ToJson(s));
        }

        Debug.Log("Save had Done!");
    }

    // This function called when the user press "Place".
    // The function Place 1 ball on the plane that we found on the space. 
    private void Control_OnPlacePress()
    {
        Debug.Log("Place Pressed!");
        OnChangeText("pose: " + placementPose.position.ToString() + "\nrot: "
            + placementPose.rotation.ToString());
        Debug.Log("pose: " + placementPose.position.ToString());
        Debug.Log("rot: " + placementPose.rotation.ToString());

        //Place the Ball in position
        Instantiate(measurementPointPrefab, placementPose.position, Quaternion.identity);

        //Save the ball's position so when the user press "Save" we can easily save them all at once.
        ScanToSave _scan = new ScanToSave(placementPose, counter);
        ScanList.Add(_scan);
        counter++;
        Debug.Log(ScanList.ToString());
        Debug.Log("counter is: " + counter);
    }

    // This function called when the user press "Door".
    // The function place a door on a plane. 
    private void Control_OnDoorPress()
    {
        Debug.Log("Door pressed, got:\n" + placementPose.ToString());

        var DoorAnchor = Instantiate(Door, placementPose.position, Quaternion.identity);
        //DoorAnchor.AddComponent<ARAnchor>();
        
        doorHasPlaced = true;
    }

    // This function draw a line between 2 positions on the space.
    //private void UpdateLine()
    //{
    //    if (Input.touchCount > 0)
    //    {
    //        Touch touch = Input.GetTouch(0);
    //        if (touch.phase == TouchPhase.Began)
    //        {
    //            touchPosition = touch.position;

    //            if (arOrigin.Raycast(touchPosition, hits, TrackableType.Planes))
    //            {
    //                startPoint.SetActive(true);

    //                Pose hitPose = hits[0].pose;
    //                startPoint.transform.SetPositionAndRotation(hitPose.position, hitPose.rotation);
    //            }
    //        }

    //        if (touch.phase == TouchPhase.Moved)
    //        {
    //            touchPosition = touch.position;

    //            if (arOrigin.Raycast(touchPosition, hits, TrackableType.Planes))
    //            {
    //                measureLine.gameObject.SetActive(true);
    //                endPoint.SetActive(true);

    //                Pose hitPose = hits[0].pose;
    //                endPoint.transform.SetPositionAndRotation(hitPose.position, hitPose.rotation);
    //            }
    //        }
    //    }

    //    if (startPoint.activeSelf && endPoint.activeSelf)
    //    {
    //        measureLine.SetPosition(0, startPoint.transform.position);
    //        measureLine.SetPosition(1, endPoint.transform.position);

    //        OnChangeText($"Distance: {(Vector3.Distance(startPoint.transform.position, endPoint.transform.position)).ToString("F2")} cm");
    //    }
    //}



    // This function update the placement of the indicator on the screen. 
    private void UpdatePlacementIndicator()
    {
        
        if (placementPoseIsValid)
        {
            DoorIndicator.SetActive(true);
            DoorIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        
        else
        {
            DoorIndicator.SetActive(false);
        }
       
    }

    // This function update the position's placement on the screen,
    // so we can track where the ray cast hit the trackeable object in the world. 
    private void UpdatePlacementPose()
    { 
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 0.5f));
        var _hits = new List<ARRaycastHit>();

        arOrigin.Raycast(screenCenter, _hits, TrackableType.Planes);
        placementPoseIsValid = _hits.Count > 0;
        if (placementPoseIsValid)
        {
            placementPose = _hits[0].pose;

            var cameraForward = Camera.current.transform.position;
            var cameraBearing = new Vector3(cameraForward.x, cameraForward.y, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
        
    }

}






