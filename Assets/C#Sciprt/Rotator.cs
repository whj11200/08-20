using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
     float speed = 1f;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0,speed,0*Time.deltaTime));
    }
}
