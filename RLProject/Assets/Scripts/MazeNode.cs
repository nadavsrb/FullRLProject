using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeNode : MonoBehaviour
{
    public enum States {
        UNBLOCKED,
        BLOCKED,
        TARGERT
    }

    public static readonly float HEIGHT = 0.8f;
    public static readonly float WIDTH = 0.8f;

    private SpriteRenderer blockSR;
    private SpriteRenderer aliceSR;
    private readonly Color UNBLOCKED_COLOR = new Color(50.0f / 255.0f, 50.0f / 255.0f, 50.0f / 255.0f, 1.0f);
    private readonly Color MARKED_COLOR = new Color(255.0f / 255.0f, 255.0f / 255.0f, 0.0f / 255.0f, 1.0f);
    private readonly Color BLOCKED_COLOR = Color.red;
    private static bool isChangeable = false;

    private static bool shouldMarkeWithColor = false;
    private bool markedState = false;
    public bool MarkedState
    {
        get => markedState;
        set {
            if (value == markedState || isChangeable) return;

            if (shouldMarkeWithColor)
            {
                Color color = UNBLOCKED_COLOR;
                if (value)
                {
                    color = MARKED_COLOR;
                }
                blockSR.color = color;
            }

            markedState = value;
        }
    }


    private States state = States.UNBLOCKED;
    public States State{
        get => state;

        set
        {
            if (value == state || !isChangeable) return;

            aliceSR.enabled = false;

            switch (value)
            {
                case States.UNBLOCKED:
                    blockSR.color = UNBLOCKED_COLOR;
                    break;

                case States.BLOCKED:
                    blockSR.color = BLOCKED_COLOR;
                    break;

                case States.TARGERT:
                    blockSR.color = UNBLOCKED_COLOR;
                    aliceSR.enabled = true;
                    break;

                default:
                    Debug.Log("Error: got an undifined state");
                    return;
            }

            state = value;
        }
    }

    private void Start()
    {
        blockSR = GetComponent<SpriteRenderer>();
        
        var transform = GetComponent<Transform>();

        transform.localScale = new Vector3(WIDTH, HEIGHT);

        aliceSR = transform.GetChild(0).GetComponent<SpriteRenderer>();
        aliceSR.enabled = false;

        blockSR.color = UNBLOCKED_COLOR;
    }

    private void OnMouseDown()
    {
        if (!isChangeable) return;

        State = (States)(((int)State + 1) % 3);
    }

    public static void SetIsChangeable(bool state)
    {
        isChangeable = state;
    }

    public static void SetShouldMarkeWithColor(bool state)
    {
        shouldMarkeWithColor = state;
    }
}
