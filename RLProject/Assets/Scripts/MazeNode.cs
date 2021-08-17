using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeNode : MonoBehaviour
{
    public delegate void changed();
    public event changed notify;

    public static readonly float HEIGHT = 0.8f;
    public static readonly float WIDTH = 0.8f;

    private SpriteRenderer blockSR;
    private SpriteRenderer aliceSR;
    private readonly Color UNBLOCKED_COLOR = new Color(50.0f / 255.0f, 50.0f / 255.0f, 50.0f / 255.0f, 1.0f);
    private readonly Color BLOCKED_COLOR = Color.red;
    private static bool isChangeable = false;

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

        if (blockSR.color == UNBLOCKED_COLOR)
        {
            if (aliceSR.enabled)
            {
                aliceSR.enabled = false;
            }
            else
            {
                blockSR.color = BLOCKED_COLOR;
            }
        } else
        {
            aliceSR.enabled = true;
            blockSR.color = UNBLOCKED_COLOR;
        }

        notify();
    }

    public static void setIsChangeable(bool state)
    {
        isChangeable = state;
    }

}
