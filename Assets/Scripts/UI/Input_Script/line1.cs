using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class line1 : MonoBehaviour
{
    [SerializeField] GameObject Input_line1;
    public static line1 instance;
    public Vector3 mousePos;
    public float Distance;
    public Vector2 Direction;
    public bool swiped = false;
    public bool swipping = false;
    public bool swipeDetected = false;
    public event Action<int> userInputEvent;
    public bool isInputBlocked = false;
    public float MinMovement;
    Action<Vector2> SwipeDetect;
    public float[] diagonals = { 45, 135, 225, 315 };
    public float windowInDeg = 20f;
    void Update(){
       ProcessInput();
    }
    void Awake(){
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        MinMovement = Mathf.Max(screenSize.x, screenSize.y) / 14f;
        Debug.Log("MinSwipeDist:" + MinMovement);

        instance = this;
    }
    public void ProcessInput()
    {
        if (Input.GetMouseButtonDown(0) == true)
        {
            mousePos = Input.mousePosition;
            swiped = false;
        }
        else if (Input.GetMouseButton(0) == true)
        {
            swipeDetected = checkSwipe(mousePos, Input.mousePosition);
            Direction = (Input.mousePosition - mousePos).normalized;
            Distance = Vector2.Distance(Input.mousePosition, mousePos);
            if (swipeDetected)
            {
                onSwipeDetected(Direction);
            }
        }
        else if (Input.GetMouseButtonUp(0) == true)
        {
            float clockwiseDeg = 360f - Quaternion.FromToRotation(Vector2.up, Direction).eulerAngles.z;
            int temp = checkDirection_mouse(clockwiseDeg);
            if(userInputEvent!=null) userInputEvent.Invoke(temp);
            //inputStream += temp.ToString() + ",";
            swiped = true;
            swipping = false;
            swipeDetected = false;
        }
    }
    public int checkDirection_mouse(float Deg)
    {
        if (Distance < MinMovement)
        {
            //Debug.Log("Touch");
            return 0;
        }
        else if ((Deg > diagonals[3] + windowInDeg && Deg <= 360) ||
                   (Deg <= diagonals[0] - windowInDeg && Deg >= 0))
        {
            //Debug.Log("UP");
            return 1;
        }
        else if (Deg > diagonals[0] - windowInDeg && Deg <= diagonals[0] + windowInDeg)
        {
            //Debug.Log("UP_RIGHT");
            return 2;
        }
        else if (Deg > diagonals[0] + windowInDeg && Deg <= diagonals[1] - windowInDeg)
        {
            //Debug.Log("RIGHT");
            return 3;
        }
        else if (Deg > diagonals[1] - windowInDeg && Deg <= diagonals[1] + windowInDeg)
        {
            //Debug.Log("DOWN_RIGHT");
            return 4;
        }
        else if (Deg > diagonals[1] + windowInDeg && Deg <= diagonals[2] - windowInDeg)
        {
            //Debug.Log("DOWN");
            return 5;
        }
        else if (Deg > diagonals[2] - windowInDeg && Deg <= diagonals[2] + windowInDeg)
        {
            //Debug.Log("DOWN_LEFT");
            return 6;
        }
        else if (Deg > diagonals[2] + windowInDeg && Deg <= diagonals[3] - windowInDeg)
        {
            //Debug.Log("LEFT");
            return 7;
        }
        else
        {
            return 8;
            //Debug.Log("UP_LEFT");
        }
    }
    public bool checkSwipe(Vector3 downPos, Vector3 currentPos)
    {
        Vector2 currentSwipe = currentPos - downPos;

        if (swiped == true)
        {//터치됨, 스와프는 완료된 상태
            //Debug.Log("false");
            swipping = false;
            return false;
        }
        if (isInputBlocked == true)
        {//무언가가 인풋을 막고있을때
            return false;
        }
        if (currentSwipe.magnitude >= MinMovement)
        {
            //Debug.Log("true");
            swipping = true;
            return true;
        }
        return false;
    }

    public void setOnSwipeDetected(Action<Vector2> onSwipeDetected)
    {
        SwipeDetect = onSwipeDetected;
    }
    public void onSwipeDetected(Vector2 swipeDirection)
    {
        //swiped = true;
        swipping = true;
        //SwipeDetect(swipeDirection);
    }
    public void blockInput()
    {
        isInputBlocked = true;
    }
    public void unBlockInput()
    {
        isInputBlocked = false;
    }

    private void OnMouseEnter(){
        if(swipping == true){
            Debug.Log("line1");
        }
    }
}