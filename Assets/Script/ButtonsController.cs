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

    public event Action OnSavePress;
    public event Action OnLoadPress;
    public event Action OnDoorPress;
    public event Action OnPlacePress;

    [SerializeField] private Button SaveBTM, LoadBTM, PlaceBTM, DoorBTM;

    [SerializeField] private TMPro.TextMeshProUGUI MainText;

    private ARTapToPlaceObject arTap;
  

    private void OnEnable()
    {
        SaveBTM.onClick.AddListener(OnSavePress.Invoke);
        LoadBTM.onClick.AddListener(OnLoadPress.Invoke);
        PlaceBTM.onClick.AddListener(OnPlacePress.Invoke);
        
        DoorBTM.onClick.AddListener(() => 
        {
            SetButtons(true); 
            OnDoorPress.Invoke();
        }); 
     
        SetMainText("Find the Door");

        arTap = FindObjectOfType<ARTapToPlaceObject>();

        arTap.OnChangeText += SetMainText;

    }

   private void SetMainText(string str)
   {
        MainText.text = str;
   }
    


    private void OnDisable()
    {
        Debug.Log("Disable!");

        arTap.OnChangeText -= SetMainText;

    }


    public void SetButtons(bool flag)
    {
        SaveBTM.gameObject.SetActive(flag);
        LoadBTM.gameObject.SetActive(flag);
        PlaceBTM.gameObject.SetActive(flag);
        DoorBTM.gameObject.SetActive(!flag);
    }



}
