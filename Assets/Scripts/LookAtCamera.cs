using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Camera.main.transform.position);
    }
}
