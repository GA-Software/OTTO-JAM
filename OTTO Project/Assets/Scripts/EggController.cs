using UnityEngine;

public class EggController : MonoBehaviour
{
    private Vector3 mOffset;
    
    private float mZCoord, transformationTimer, destroyTimer;
    private int secondsRequiredForTransformation;
    public GameObject circlePointer, chickPrefab;

    private Transform chickensParent;
    private GameObject thisPointer;
    private Rigidbody _rigidbody;
    private bool isDraggable = true;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        chickensParent = GameObject.FindGameObjectWithTag("ChickensParent").transform;
    }

    private void Update()
    {
        if (isDraggable)
        {
            waitForDestroy();
        }
        else
        {
            waitForTransformation();
        }
    }

    void OnMouseDown()
    {
        if (isDraggable && GameManager.Instance.isGameStarted && !GameManager.Instance.isGameOver)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.grabClip);
            _rigidbody.constraints = RigidbodyConstraints.FreezePositionY;
            thisPointer = Instantiate(circlePointer, new Vector3(transform.position.x, transform.position.y - 3.8f, transform.position.z), circlePointer.transform.rotation, transform);
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
        if (collision.gameObject.tag == "Basket")
        {
            isDraggable = false;
            GameManager.Instance.CollectEgg();
            secondsRequiredForTransformation = Random.Range(10, 30);
        }
    }
    
    public void waitForTransformation()
    {
        if (GameManager.Instance.isGameStarted && !GameManager.Instance.isGameOver)
        {
            transformationTimer += Time.deltaTime;
            if (transformationTimer >= secondsRequiredForTransformation)
            {
                GameManager.Instance.ControlChickenCount();
                GameObject GO = Instantiate(chickPrefab, transform.position, chickPrefab.transform.rotation, chickensParent);
                GameManager.Instance.chickens.Add(GO.GetComponent<Player>());
                StartCoroutine(GameManager.Instance.playParticle(GO.transform.position));

                Destroy(gameObject);
            }
        }
    }

    private void OnMouseOver()
    {

        if (isDraggable && GameManager.Instance.isGameStarted && !GameManager.Instance.isGameOver)
        {
            GameManager.Instance.instructText.gameObject.SetActive(true);
            GameManager.Instance.instructText.text = "Yumurtayı al.";
        }
    }

    private void OnMouseExit()
    {
        GameManager.Instance.instructText.gameObject.SetActive(false);
    }

    public void waitForDestroy()
    {
        if (GameManager.Instance.isGameStarted && !GameManager.Instance.isGameOver)
        {
            destroyTimer += Time.deltaTime;
            if (destroyTimer >= 30f)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
            SoundManager.Instance.PlaySound(SoundManager.Instance.dropClip);
    }
}