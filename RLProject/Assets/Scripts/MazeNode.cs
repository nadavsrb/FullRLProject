using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeNode : MonoBehaviour
{
    public delegate void changed();
    public event changed notify;

    private SpriteRenderer sr;
    private readonly Color UNBLOCKED_COLOR = Color.black;
    private readonly Color BLOCKED_COLOR = Color.red;
    private static bool isChangeable = false;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.color = UNBLOCKED_COLOR;
    }

    private void OnMouseDown()
    {
        if (!isChangeable) return;

        sr.color = (sr.color == UNBLOCKED_COLOR) ? BLOCKED_COLOR : UNBLOCKED_COLOR;

        notify();
    }

    public static void setIsChangeable(bool state)
    {
        isChangeable = state;
    }
}
