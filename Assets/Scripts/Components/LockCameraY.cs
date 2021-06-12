using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockCameraY : MonoBehaviour
{
    // Not a pretty camera, but at least it is not jumping with the player
    // It solves its purpose for now!
    private float y = 0.0f;
    void Start()
    {
        y = Camera.main.transform.position.y;
    }

    void LateUpdate()
    {
        var position = Camera.main.transform.position;
        position.y = y;
        Camera.main.transform.position = position;
    }
}
