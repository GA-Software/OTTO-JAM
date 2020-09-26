using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private bool hasFoundChicken = false, willReturn = false, chickenIsMoving = false;
    [SerializeField] private Player targetChicken;
    [SerializeField] private Vector3 originalPos;

    private void Awake()
    {
        originalPos = transform.position;
    }

    private void Update()
    {
        if(GameManager.Instance.isGameStarted && !GameManager.Instance.isGameOver && GameManager.Instance.chickens.Count > 0)
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
                Vector3 tempPos = targetChicken.transform.position;
                tempPos.y = 5f;
                transform.position = Vector3.Lerp(transform.position, tempPos, 2f * Time.deltaTime);
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
                willReturn = false;
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
            StartCoroutine(TakeChicken());
        }
    }

    public void returnToOriginalPos()
    {
        transform.position = Vector3.Lerp(transform.position, originalPos, 2f * Time.deltaTime);
    }

    IEnumerator TakeChicken()
    {
        chickenIsMoving = true;
        yield return new WaitForSeconds(3f);
        GameManager.Instance.chickens.Remove(targetChicken);
        GameManager.Instance.ControlChickenCount();
        Destroy(targetChicken.gameObject);
        hasFoundChicken = false;
        chickenIsMoving = false;
        willReturn = true;
    }

    bool isHome()
    {
        return Vector3.Distance(transform.position, originalPos) < 0.1f;
    }

    private void moveChicken()
    {
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
