using System.Collections;
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
        if (isDraggable && GameManager.Instance.isGameStarted && !GameManager.Instance.isGameOver)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.grabClip);
            _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            thisPointer = Instantiate(circlePointer, new Vector3(transform.position.x, transform.position.y - 3.8f, transform.position.z), circlePointer.transform.rotation, transform);
            thisPointer.transform.localScale *= 4f;
            mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;

            // Store offset = gameobject world pos - mouse world pos
            mOffset = gameObject.transform.position - GetMouseAsWorldPoint();
        }
    }

    private void OnMouseOver()
    {
        if (isDraggable && GameManager.Instance.isGameStarted && !GameManager.Instance.isGameOver)
        {
            GameManager.Instance.instructText.gameObject.SetActive(true);
            GameManager.Instance.instructText.text = "Lambayı al.";
        }
    }

    private void OnMouseExit()
    {
        GameManager.Instance.instructText.gameObject.SetActive(false);
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
        if (isDraggable && GameManager.Instance.isGameStarted && !GameManager.Instance.isGameOver)
        {
            transform.position = new Vector3(GetMouseAsWorldPoint().x + mOffset.x, 2f, GetMouseAsWorldPoint().z + mOffset.z);
        }
    }

    private void OnMouseUp()
    {
        if(isDraggable && GameManager.Instance.isGameStarted && !GameManager.Instance.isGameOver)
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

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Ground")
            SoundManager.Instance.PlaySound(SoundManager.Instance.dropClip);
    }
}