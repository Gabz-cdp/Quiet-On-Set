using System;
using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool IsOpen = false;
    [SerializeField]
    private bool IsRotatingDoor = true; 
    [SerializeField]
    private float DoorSpeed = 1f; //how quick the rotation is

    [Header("Rotation Configs")]
    [SerializeField]
    private float RotationAmount = 110f; //how much the door rotates when opened
    [SerializeField]
    private float ForwardDirection = 0;

    private Vector3 StartRotation;
    private Vector3 Forward;

    private Coroutine AnimationCoroutine;

    private void Awake()
    {
        StartRotation = transform.rotation.eulerAngles; //forward is actually pointing into the door frame, change to right so that the player enters from one side of the door
        Forward = transform.right; //the direction that the player will enter from, therefore right will be forward
    }

    public void Open(Vector3 UserPosition)
    {
        if(!IsOpen) //checking that the door is not open
        {
            if (AnimationCoroutine != null)
            {
                StopCoroutine(AnimationCoroutine);
            }

            if(IsRotatingDoor) //checking to see that the door is a rotating door
            {
                float dot = Vector3.Dot(Forward, (UserPosition - transform.position).normalized); //the dot checks to see if the player is infront of the player (positive) or behind (negative)
                Debug.Log($"Dot: {dot.ToString("N3")}");
                AnimationCoroutine = StartCoroutine(DoRotationOpen()); //Will play the opening animation
            }
        }
    }

    private IEnumerator DoRotationOpen(float ForwardAmount = 0)
    {
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation;

        if (ForwardAmount >= ForwardDirection) //door will open away from the player
        {
            endRotation = Quaternion.Euler(new Vector3(0, StartRotation.y - RotationAmount, 0)); //forward direction is -1, so door will always open with a negative rotation amount
        }
        else
        {
            endRotation = Quaternion.Euler(new Vector3(0, StartRotation.y + RotationAmount, 0)); //forward direction is +1, so door will always open with a positive rotation amount
        }

        IsOpen = true; //Will be able to close the door after opened
        float time = 0;

        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, time); //Sperical lerp to make rotation a little more interesting
            yield return null;
            time += Time.deltaTime * DoorSpeed; //controls how fast or slow the door opens
        }
    }

    public void Close()
    {
        if(IsOpen)
        {
            if(AnimationCoroutine != null)
            {
                StopCoroutine(AnimationCoroutine);
            }

            if(IsRotatingDoor)
            {
                AnimationCoroutine = StartCoroutine(DoRotationClose());
            }
        }
    }

    private IEnumerator DoRotationClose()
    {
        //handles start and end rotation points for animation
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.Euler(StartRotation);

        IsOpen = false;
        float time = 0;

        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, time);
            yield return null;
            time += Time.deltaTime * DoorSpeed;
        }
    }
}
