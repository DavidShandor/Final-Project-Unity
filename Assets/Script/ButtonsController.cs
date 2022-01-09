using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ButtonsController : MonoBehaviour
{
    //Declare events, Gameobjects and text. 
    public event Action OnSavePress;
    public event Action OnLoadPress;
    public event Action OnDoorPress;
    public event Action OnPlacePress;

    [SerializeField] private Button SaveBTM, LoadBTM, PlaceBTM, DoorBTM;

    [SerializeField] private TMPro.TextMeshProUGUI MainText;

    private ARSessionController ARController;

    // This Function Called when the script is enable by the application.
    private void OnEnable()
    {
        // Set listenners to the events from the user,
        // and send the proper message to the AR Session Controller 
        SaveBTM.onClick.AddListener(OnSavePress.Invoke);
        LoadBTM.onClick.AddListener(OnLoadPress.Invoke);
        PlaceBTM.onClick.AddListener(OnPlacePress.Invoke);
        
        DoorBTM.onClick.AddListener(() => 
        {
            SetButtons(true); 
            OnDoorPress.Invoke();
        }); 
     
        // Set main text
        SetMainText("Find the Door");

        // Get The AR Session Controller object's reference.
        ARController = FindObjectOfType<ARSessionController>();

        // Set an event listenners to the AR Session Controller event's messages. 
        ARController.OnChangeText += SetMainText;

    }

    // Set the Main text on the screen
   private void SetMainText(string str)
   {
        MainText.text = str;
   }


    // This function called with the script disable by the application.
    private void OnDisable()
    {
        Debug.Log("Disable!");

        // Remove the listenner.
        ARController.OnChangeText -= SetMainText;

    }

    // Hide or Show the buttons on the screen.
    public void SetButtons(bool flag)
    {
        SaveBTM.gameObject.SetActive(flag);
        LoadBTM.gameObject.SetActive(flag);
        PlaceBTM.gameObject.SetActive(flag);
        DoorBTM.gameObject.SetActive(!flag);
    }



}
