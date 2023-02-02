using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform cam;
    // Update is called once per frame
    void Update()
    {
        transform.LookAt(new Vector3(cam.position.x, transform.position.y, cam.position.z));
    }
}
