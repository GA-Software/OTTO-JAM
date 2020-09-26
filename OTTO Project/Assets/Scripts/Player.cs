using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Player : MonoBehaviour
{
    public enum State { Chicken, Chick };
    
    private Camera cam;
    private NavMeshAgent agent;
    private Animator animator;

    public GameObject egg, chickenPrefab;
    private float eggTimer, transformationTimer;
    private int secondsRequiredForEgg, secondsRequiredForTransformation;
    public State state;

    [SerializeField] private Vector3 targetPos;
    [SerializeField] private bool isReadyForNewTarget = false, waitingForNewTarget = false;
    
    private void Awake()
    {
        cam = Camera.main;
        animator = transform.GetChild(0).GetComponent<Animator>();
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        secondsRequiredForEgg = Random.Range(20, 60);
        secondsRequiredForTransformation = Random.Range(30, 60);

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

        float distance = Vector3.Distance(targetPos, transform.position);

        if (Mathf.Approximately(distance, 0f) && !waitingForNewTarget)
        {
            isReadyForNewTarget = true;
            StartCoroutine(SetRandomDestinationAfterSeconds());
        }
        
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
                secondsRequiredForEgg = Random.Range(20, 60);
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
                Instantiate(chickenPrefab, transform.position, chickenPrefab.transform.rotation, transform.parent);
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
            Vector3 randomDestination = new Vector3(Random.Range(-8f, 8f), transform.position.y, Random.Range(-8f, 8f));

            targetPos = randomDestination;
            agent.destination = randomDestination;
            isReadyForNewTarget = false;
            waitingForNewTarget = false;
        }
    }
}
