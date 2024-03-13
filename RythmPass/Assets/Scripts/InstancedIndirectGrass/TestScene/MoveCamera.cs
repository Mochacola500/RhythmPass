using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Camera m_Camera;
    public List<Transform> allCameraTransforms = new List<Transform>();

    int index = 0;
    private void OnGUI()
    {
        if(GUI.Button(new Rect(100, 450, 200, 100), "Move camera"))
        {
            index = (index+1) % allCameraTransforms.Count;
        }
    }

    private void LateUpdate()
    {
        Transform target = allCameraTransforms[index];

        m_Camera.transform.position = Vector3.Lerp(m_Camera.transform.position,target.position, Time.deltaTime * 2);
        m_Camera.transform.rotation = Quaternion.Slerp(m_Camera.transform.rotation,target.rotation, Time.deltaTime * 2);
    }
}
