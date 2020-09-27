using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Player : MonoBehaviour
{
    public enum State { Chicken, Chick };
    
    private Camera cam;
    public NavMeshAgent agent;
    private Animator animator;

    public GameObject egg, chickenPrefab;
    [SerializeField] private float eggTimer, transformationTimer, newTargetTimer;
    private int secondsRequiredForEgg, secondsRequiredForTransformation;
    public State state;

    [SerializeField] private Vector3 targetPos;
    [SerializeField] private bool isReadyForNewTarget = false, waitingForNewTarget = false;
    public bool isSafe = false;
    
    private void Awake()
    {
        cam = Camera.main;
        animator = transform.GetChild(0).GetComponent<Animator>();
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        secondsRequiredForEgg = Random.Range(10, 30);
        secondsRequiredForTransformation = Random.Range(10, 30);

        targetPos = transform.position;
        isReadyForNewTarget = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Chicken)
        {
            spawnEgg();
        }
        else
        {
            waitForTransformation();
        }

        targetPos.y = transform.position.y;
        float distance = Vector3.Distance(targetPos, transform.position);


        if (Mathf.Approximately(distance, 0f) && !waitingForNewTarget)
        {
            isReadyForNewTarget = true;
            StartCoroutine(SetRandomDestinationAfterSeconds());
        }

        waitForNewTarget();
        
    }

    public void spawnEgg()
    {
        if(GameManager.Instance.isGameStarted && !GameManager.Instance.isGameOver)
        {
            eggTimer += Time.deltaTime;
            if (eggTimer >= secondsRequiredForEgg)
            {
                Instantiate(egg, transform.position, egg.transform.rotation);
                eggTimer = 0f;
                secondsRequiredForEgg = Random.Range(10, 30);
            }
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
                GameObject GO = Instantiate(chickenPrefab, transform.position, chickenPrefab.transform.rotation, transform.parent);
                GameManager.Instance.chickens.Remove(this);
                GameManager.Instance.chickens.Add(GO.GetComponent<Player>());
                StartCoroutine(GameManager.Instance.playParticle(GO.transform.position));

                Destroy(gameObject);
            }
        }
    }

    IEnumerator SetRandomDestinationAfterSeconds()
    {
        waitingForNewTarget = true;

        int randomNo = Random.Range(0, 2);

        animator.SetBool("Walk", false);
        switch (state)
        {
            case State.Chicken:
                animator.SetBool(randomNo == 0 ? "Turn Head" : "Eat", true);
                yield return new WaitForSeconds(Random.Range(4f, 10f));
                animator.SetBool(randomNo == 0 ? "Turn Head" : "Eat", false);
                break;
            case State.Chick:
                if (randomNo == 0)
                    animator.SetBool("Eat", true);
                else
                    animator.SetTrigger("Jump");
                yield return new WaitForSeconds(Random.Range(4f, 10f));
                if (randomNo == 0)
                    animator.SetBool("Eat", false);
                break;
            default:
                break;
        }
        
        if (isReadyForNewTarget)
        {
            animator.SetBool("Walk", true);
            //Vector3 randomDestination = new Vector3(Random.Range(-8f, 8f), transform.position.y, Random.Range(-8f, 8f));

            //targetPos = randomDestination;
            //agent.destination = randomDestination;

            NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();

            // Pick the first indice of a random triangle in the nav mesh
            int t = Random.Range(0, navMeshData.indices.Length - 3);

            // Select a random point on it
            Vector3 point = Vector3.Lerp(navMeshData.vertices[navMeshData.indices[t]], navMeshData.vertices[navMeshData.indices[t + 1]], Random.value);
            Vector3.Lerp(point, navMeshData.vertices[navMeshData.indices[t + 2]], Random.value);

            agent.destination = point;

            isReadyForNewTarget = false;
            waitingForNewTarget = false;
        }
    }

    public void waitForNewTarget()
    {
        if (GameManager.Instance.isGameStarted && !GameManager.Instance.isGameOver)
        {
            if (Vector3.Distance(targetPos, transform.position) < 0.1f)
                newTargetTimer += Time.deltaTime;
            else
                newTargetTimer = 0f;
            if (newTargetTimer >= 10f)
            {
                waitingForNewTarget = false;
                isReadyForNewTarget = true;
                newTargetTimer = 0f;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Lamp")
        {
            isSafe = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Lamp")
        {
            isSafe = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Lamp")
        {
            StartCoroutine(waitForSafe());
        }
    }
    
    IEnumerator waitForSafe()
    {
        yield return new WaitForSeconds(3f);
        isSafe = false;
    }
}
