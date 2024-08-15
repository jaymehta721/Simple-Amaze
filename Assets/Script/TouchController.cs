using System;
using UnityEngine;

public class TouchController : MonoBehaviour
{
    private const float MAX_SWIPE_TIME = 0.5f;
    private const float MIN_SWIPE_DISTANCE = 0.17f;

    private Vector2 startPosition;
    private float startTime;

    public bool IsBallMoving { get; set; }

    public Vector2 Direction { get; private set; }

    private void Update()
    {
        if (!IsBallMoving)
        {
#if UNITY_EDITOR
            UpdateDirectionFromKeyboard();
#endif
            UpdateDirectionFromTouch();
        }
    }

    private void UpdateDirectionFromKeyboard()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Direction = Vector2.up;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Direction = Vector2.down;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Direction = Vector2.right;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Direction = Vector2.left;
        }
    }

    private void UpdateDirectionFromTouch()
    {
        if (Input.touches.Length <= 0)
        {
            return;
        }
        
        Touch touch = Input.GetTouch(0);
        switch (touch.phase)
        {
            case TouchPhase.Began:
                startPosition = new Vector2(touch.position.x / Screen.width, touch.position.y / Screen.width);
                startTime = Time.time;
                break;
            case TouchPhase.Ended when Time.time - startTime > MAX_SWIPE_TIME:
                return;
            case TouchPhase.Ended:
            {
                Vector2 endPosition = new Vector2(touch.position.x / Screen.width, touch.position.y / Screen.width);
                Vector2 swipe = new Vector2(endPosition.x - startPosition.x, endPosition.y - startPosition.y);

                if (swipe.magnitude < MIN_SWIPE_DISTANCE)
                    return;

                if (Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y))
                {
                    Direction = swipe.x > 0 ? Vector2.right : Vector2.left;
                }
                else
                {
                    Direction = swipe.y > 0 ? Vector2.up : Vector2.down;
                }

                break;
            }
            case TouchPhase.Moved:
                break;
            case TouchPhase.Stationary:
                break;
            case TouchPhase.Canceled:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
