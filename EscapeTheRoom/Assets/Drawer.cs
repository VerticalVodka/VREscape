using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VREscape;

public class Drawer : MonoBehaviour
{
    private HWManager _hwManager;

    public float MoveLength;
    public int Speed;

    private float _startTime;

    private bool opening;

    private bool isOpen = false;

    private bool closing;

    private Vector3 normalPosition;

    private Vector3 currentPos;
    private Vector3 targetPos;

    void Start()
    {
        _hwManager = FindObjectOfType<HWManager>();
        normalPosition = transform.position;
    }

    public void Open()
    {
        if (opening || closing) return;
        if (isOpen) return;
        Debug.Log("Opening");
        opening = true;
        currentPos = transform.position;
        targetPos = currentPos + transform.forward * MoveLength;
        _startTime = Time.time;
        isOpen = true;
    }

    public void Close()
    {
        if (closing || opening) return;
        if(!isOpen) return;
        Debug.Log("Closing");
        closing = true;
        currentPos = transform.position;
        targetPos = currentPos - transform.forward * MoveLength;
        _startTime = Time.time;
        isOpen = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (_hwManager.GetButtonState(Enums.ButtonEnum.Button1))
        {
            Open();
        }
        else if (_hwManager.GetButtonState(Enums.ButtonEnum.Button2))
        {
            Close();
        }

        if (!opening && !closing) return;

        var distCovered = (Time.time - _startTime) * Speed;
        var fracJourney = distCovered / MoveLength;
        transform.position = Vector3.Lerp(currentPos, targetPos, fracJourney);
        if (transform.position == targetPos)
        {
            closing = false;
            opening = false;
        }
    }
}