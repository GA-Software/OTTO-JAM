using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggController : MonoBehaviour
{
    private Vector3 mOffset;
    
    private float mZCoord;
    public GameObject circlePointer;

    private GameObject thisPointer;
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
            _rigidbody.constraints = RigidbodyConstraints.FreezePositionY;
            thisPointer = Instantiate(circlePointer, new Vector3(transform.position.x, -6.5f, transform.position.z), circlePointer.transform.rotation, transform);
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
            transform.position = new Vector3(GetMouseAsWorldPoint().x + mOffset.x, 3f, GetMouseAsWorldPoint().z + mOffset.z);
        }
    }

    private void OnMouseUp()
    {
        if(isDraggable)
        {
            _rigidbody.constraints = RigidbodyConstraints.None;
            Destroy(thisPointer);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Basket")
        {
            isDraggable = false;
            Destroy(GetComponent<Rigidbody>());
            Destroy(GetComponent<CapsuleCollider>());
            GameManager.Instance.CollectEgg();
        }
    }
}