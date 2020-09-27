using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private bool hasFoundChicken = false, willReturn = false, chickenIsMoving = false;
    [SerializeField] private Player targetChicken;
    [SerializeField] private Vector3 originalPos;
    public GameObject particleEffect;

    private bool isUfoMoving = false, waitingForStart = true;

    private void Awake()
    {
        originalPos = transform.position;
        particleEffect.SetActive(false);
        StartCoroutine(waitForStart());
    }

    IEnumerator waitForStart()
    {
        yield return new WaitForSeconds(10f);
        waitingForStart = false;
        Debug.Log("enemy started");
    }

    private void Update()
    {
        if(GameManager.Instance.isGameStarted && !GameManager.Instance.isGameOver && GameManager.Instance.chickens.Count > 0 && !waitingForStart)
        {
            foreach (Player chicken in GameManager.Instance.chickens)
            {
                if (!chicken.isSafe && !hasFoundChicken &&  isHome())
                {
                    targetChicken = chicken;
                    hasFoundChicken = true;
                }
            }

            if (hasFoundChicken && targetChicken != null)
            {
                if (isUfoMoving)
                {
                    SoundManager.Instance.PlaySound(SoundManager.Instance.shipMovingClip);
                    isUfoMoving = false;
                }

                Vector3 tempPos = targetChicken.transform.position;
                tempPos.y = 5f;
                transform.position = Vector3.Lerp(transform.position, tempPos, 1f * Time.deltaTime);
            }

            if (targetChicken == null || targetChicken.isSafe)
            {
                hasFoundChicken = false;
                willReturn = true;
            }

            if (willReturn)
            {
                returnToOriginalPos();
            }

            if(chickenIsMoving)
            {
                moveChicken();
            }

            if (isHome())
            {
                Destroy(GameObject.Find("UfoMovingClip"));
                willReturn = false;
                isUfoMoving = true;
            }
        }

        if (GameManager.Instance.chickens.Count == 0)
        {
            GameManager.Instance.GameOver();
        }

    }
    
    public void checkIfSafe()
    {
        if (!targetChicken.isSafe)
        {
            particleEffect.SetActive(true);
            StartCoroutine(TakeChicken());
        }
        else
        {
            willReturn = true;
            isUfoMoving = true;
        }
    }

    public void returnToOriginalPos()
    {
        transform.position = Vector3.Lerp(transform.position, originalPos, 1f * Time.deltaTime);
    }

    IEnumerator TakeChicken()
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.shipKidnapClip);
        yield return new WaitForSeconds(1f);
        chickenIsMoving = true;
        Destroy(targetChicken.agent);
        Destroy(targetChicken.GetComponent<Rigidbody>());
        yield return new WaitForSeconds(3f);
        GameManager.Instance.chickens.Remove(targetChicken);
        GameManager.Instance.ControlChickenCount();
        Destroy(targetChicken.gameObject);
        hasFoundChicken = false;
        chickenIsMoving = false;
        willReturn = true;
        particleEffect.SetActive(false);
    }

    bool isHome()
    {
        return Vector3.Distance(transform.position, originalPos) < 0.1f;
    }

    private void moveChicken()
    {
        if(targetChicken != null)
            targetChicken.transform.position = Vector3.Lerp(targetChicken.transform.position, transform.position, 2f * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Chicken")
        {
            checkIfSafe();
        }
    }
}
