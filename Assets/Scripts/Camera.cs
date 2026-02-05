using UnityEngine;

public class cam : MonoBehaviour
{
    
    public float sensitivity = 1.0f;

    float m_CameraVerticalAngle;
    float RotationMultiplier = 1;

    Transform t;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        t = GetComponent<Transform>();
        t.rotation = Quaternion.Euler(180f, 0f, 0f);
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 mouseDelta = UnityEngine.InputSystem.Mouse.current.delta.ReadValue();
        // Debug.Log("mouse y moved by "+ mouseDelta.y + ". The camera Euler Angle is " + Camera.main.transform.eulerAngles.x);

        //mouse movement base taken from unity fps tutorial
        m_CameraVerticalAngle += -mouseDelta.y * RotationMultiplier * sensitivity;

        // limit the camera's vertical angle to min/max
        m_CameraVerticalAngle = Mathf.Clamp(m_CameraVerticalAngle, -70f, 70f);

        // apply the vertical angle as a local rotation to the camera transform along its right axis (makes it pivot up and down)
        t.transform.localEulerAngles = new Vector3(m_CameraVerticalAngle, 0, 0);       
    }
    
}
