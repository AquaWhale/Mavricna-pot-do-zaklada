using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0.0f, 4.0f, 0.0f, Space.World);
        /*transform.RotateAround(this.transform.position, new Vector3(0.0f, 4.0f, 0.0f), 120 * Time.deltaTime);
        foreach (Transform child in transform)
        {
            child.transform.RotateAround(this.transform.position, new Vector3(0.0f, -4.0f, 0.0f), 120 * Time.deltaTime);
        }*/
        
    }
  
}
