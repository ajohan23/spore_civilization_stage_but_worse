using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //Settings
    [SerializeField] int edgeSize = 50; //Distance from the cursor to the edge of the screen that the camera begins to scroll
    [SerializeField] float panSpeed = 5f;
    [SerializeField] float zoomSpeed = 2f;
    [SerializeField] Vector3 boundingBoxSize;
    [SerializeField] Vector3 boundBoxCenter;

    //Runtime
    bool isStopped = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) isStopped = !isStopped;
        if (isStopped) return;

        PanCamera();
        Zoom();
        ClampPosition();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boundBoxCenter, boundingBoxSize);
    }

    void PanCamera()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 movementVector = Vector3.zero;

        if (mousePosition.x < edgeSize)
        {
            movementVector.x = -1;
        }
        if (mousePosition.x > Screen.width - edgeSize)
        {
            movementVector.x = 1;
        }

        if (mousePosition.y < edgeSize)
        {
            movementVector.z = -1;
        }
        if (mousePosition.y > Screen.height - edgeSize)
        {
            movementVector.z = 1;
        }

        transform.Translate(movementVector * panSpeed * Time.deltaTime, Space.World);
    }

    void Zoom()
    {
        Vector2 scrollDelta =  Input.mouseScrollDelta;

        float zoomAmount = Time.deltaTime * scrollDelta.y * zoomSpeed;

        transform.Translate(0, zoomAmount, 0, Space.World);
    }

    void ClampPosition()
    {
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, boundBoxCenter.x + -boundingBoxSize.x / 2, boundBoxCenter.x + boundingBoxSize.x / 2),
            Mathf.Clamp(transform.position.y, boundBoxCenter.y + -boundingBoxSize.y / 2, boundBoxCenter.y + boundingBoxSize.y / 2),
            Mathf.Clamp(transform.position.z, boundBoxCenter.z + -boundingBoxSize.z / 2, boundBoxCenter.z + boundingBoxSize.z / 2)
            );
    }
}
