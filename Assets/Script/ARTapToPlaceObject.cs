using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System;
using System.IO;

public class ARTapToPlaceObject : MonoBehaviour
{

    public event Action<string> OnChangeText;

    [SerializeField] private GameObject objectToPlace, Door, DoorInd, pipeIndicator;
    [HideInInspector] public GameObject placementIndicator = null;

    private ButtonsController Control;
    private Pose placementPose;
    private ARRaycastManager arOrigin;
    //private ARPlaneManager arPlanes;
    //private ARAnchorManager anchorManager;
    
    private bool placementPoseIsValid = false;

    private List<ScanToSave> ScanList = new List<ScanToSave>();
    private int counter = 0;

    private void OnEnable()
    {

        Control = FindObjectOfType<ButtonsController>();

        Control.OnDoorPress += Control_OnDoorPress;
        Control.OnPlacePress += Control_OnPlacePress;
        Control.OnSavePress += Control_OnSavePress;
        Control.OnLoadPress += Control_OnLoadPress;

        //arPlanes = FindObjectOfType<ARPlaneManager>();
        //arPlanes.planesChanged += OnPlaneChanged;
    }

    void Start()
    {
        // To reset Scenes or switch? 
        //var xrManagerSettings = UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager;
        //xrManagerSettings.DeinitializeLoader();
        //UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex); // reload current scene
        //xrManagerSettings.InitializeLoaderSync();
        placementIndicator = DoorInd;

        arOrigin = FindObjectOfType<ARRaycastManager>();
    }


    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();

    }

    private void OnDisable()
    {
        Control.OnDoorPress -= Control_OnDoorPress;
        Control.OnPlacePress -= Control_OnPlacePress;
        Control.OnSavePress -= Control_OnSavePress;
        Control.OnLoadPress -= Control_OnLoadPress;

        //arPlanes.planesChanged -= OnPlaneChanged;
    }

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
                Instantiate(objectToPlace, s.ScanPose.position, Quaternion.identity);
            }

            ScanList = temp;
        }

        Debug.Log("Load had Done!");
    }

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

    private void Control_OnPlacePress()
    {
        Debug.Log("Place Pressed!");
        OnChangeText("pose: " + placementPose.position.ToString() + "\nrot: " 
            + placementPose.rotation.ToString());
        Debug.Log("pose: " + placementPose.position.ToString());
        Debug.Log("rot: " + placementPose.rotation.ToString());

        Instantiate(objectToPlace, placementPose.position, Quaternion.identity);
        
        ScanToSave _scan = new ScanToSave(placementPose, counter);
        ScanList.Add(_scan);
        counter++;
        Debug.Log(ScanList.ToString());
        Debug.Log("counter is: " + counter);
    }

    private void Control_OnDoorPress()
    {
        Debug.Log("Door pressed, got:\n" + placementPose.ToString());
        placementIndicator = pipeIndicator;
       
        var DoorAnchor = Instantiate(Door, placementPose.position, Quaternion.identity);
        DoorAnchor.AddComponent<ARAnchor>();
    }



    //private void OnPlaneChanged(ARPlanesChangedEventArgs args)
    //{
    //    if (args.added != null && args.added.Count > 0)
    //    {
    //       foreach (ARPlane plane in arPlanes)
    //        {
    //            if (plane.alignment.IsVertical())
    //            {
                   
    //            }
    //        }
    //    }

    //}


    private void UpdatePlacementIndicator()
    {
        
        if (placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        
        else
        {
            placementIndicator.SetActive(false);
        }
       
    }

    private void UpdatePlacementPose()
    { 
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();

        arOrigin.Raycast(screenCenter, hits, TrackableType.Planes);
        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
        {
            placementPose = hits[0].pose;

            var cameraForward = Camera.current.transform.position;
            var cameraBearing = new Vector3(cameraForward.x, cameraForward.y, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
        
    }
    
 

}

