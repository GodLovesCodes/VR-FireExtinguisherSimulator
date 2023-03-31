using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class InputControl : MonoBehaviour
{
    public GameObject[] ControlUI;
    public DetectInside detectInside;
    public GameObject teleportPoint_ExtinguisherRange;
    public SimpleAttach simpleAttach;
    public Raycast raycast;
    public Text ScoreText, Status;
    private SteamVR_Action_Boolean _defult_Confirm;
    public Interactable interactable;
    public bool AttachedToHand;
    private bool insideTrig = true, TakeExtinguisher = true, pullLatch = true, Aim = true, Squeeze = true;

    void Start()
    {
        ControlUI[0].SetActive(true);
        _defult_Confirm = SteamVR_Actions._default.Confirm;
    }

    void Update()
    {
        if (interactable.attachedToHand != null)
        {
            AttachedToHand = true;
        }
        else
        {
            AttachedToHand = false;
        }

        if (detectInside.IsInside && insideTrig)
        {
            teleportPoint_ExtinguisherRange.SetActive(false);
            ControlUI[0].SetActive(false);
            ControlUI[1].SetActive(true);
            insideTrig = false;
        }

        if (AttachedToHand && TakeExtinguisher)
        {
            ControlUI[1].SetActive(false);
            ControlUI[2].SetActive(true);
            TakeExtinguisher = false;
        }
        if (AttachedToHand && _defult_Confirm.stateDown)
        {
            if (ControlUI[2].activeInHierarchy)
            {
                ControlUI[2].SetActive(false);
                ControlUI[3].SetActive(true);
            }
        }
        if (simpleAttach.anim.GetBool("PullLatch") && pullLatch)
        {
            if (ControlUI[3].activeInHierarchy)
            {
                ControlUI[3].SetActive(false);
                ControlUI[4].SetActive(true);
                pullLatch = false;
            }
        }
        if (simpleAttach.anim.GetBool("Aim") && Aim && !pullLatch)
        {
            if (ControlUI[4].activeInHierarchy)
            {
                ControlUI[4].SetActive(false);
                ControlUI[5].SetActive(true);
                Aim = false;
            }
        }
        if (simpleAttach.anim.GetBool("Extinguishing") && Squeeze && !Aim)
        {
            if (ControlUI[5].activeInHierarchy && raycast.Score > 60)
            {
                ControlUI[5].SetActive(false);
                ControlUI[6].SetActive(true);
                Squeeze = false;
            }
        }
        if (raycast.intensityValue == 0 || simpleAttach.SlerpTime == 0)
        {
            if (ControlUI[6].activeInHierarchy)
            {
                ControlUI[6].SetActive(false);
                ControlUI[7].SetActive(true);
            }
        }
        if (ControlUI[7].activeInHierarchy)
        {
            ScoreText.text = raycast.ScoreText.text;
            Status.text = "存活";
        }
    }
}
