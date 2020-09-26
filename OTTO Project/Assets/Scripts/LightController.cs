using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    private Vector3 mOffset;
    private float mZCoord;
    public GameObject circlePointer;
    
    private GameObject thisPointer, lampPrefab;
    private Rigidbody _rigidbody;
    private bool isDraggable = true;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void OnMouseDown()
    {
        if (isDraggable)
        {
            _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            thisPointer = Instantiate(circlePointer, new Vector3(transform.position.x, transform.position.y - 3.8f, transform.position.z), circlePointer.transform.rotation, transform);
            thisPointer.transform.localScale *= 4f;
            mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;

            // Store offset = gameobject world pos - mouse world pos
            mOffset = gameObject.transform.position - GetMouseAsWorldPoint();
        }
    }

    private Vector3 GetMouseAsWorldPoint()
    {
        // Pixel coordinates of mouse (x,y)
        Vector3 mousePoint = Input.mousePosition;
        
        // z coordinate of game object on screen
        mousePoint.z = mZCoord;

        // Convert it to world points
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
    
    void OnMouseDrag()
    {
        if (isDraggable)
        {
            transform.position = new Vector3(GetMouseAsWorldPoint().x + mOffset.x, 2f, GetMouseAsWorldPoint().z + mOffset.z);
        }
    }

    private void OnMouseUp()
    {
        if(isDraggable)
        {
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            Destroy(thisPointer);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Deadzone")
        {
            StartCoroutine(waitForFreezePos());
        }
    }

    IEnumerator waitForFreezePos()
    {
        _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        transform.position = Vector3.zero;
        yield return new WaitForSeconds(0.1f);
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
    }
}