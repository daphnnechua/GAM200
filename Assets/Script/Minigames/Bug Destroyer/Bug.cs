using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Bug : MonoBehaviour, IPointerClickHandler
{
    private float minSpeed = 300f;
    private float maxSpeed = 600f;
    [SerializeField] private RectTransform thisTransform;
    [SerializeField] private RectTransform canvasRt;
    private Vector3 targetPos;
    private BugDestroyerMinigame bugDestroyerMinigame;
    private float speed;

    [SerializeField] private List<AudioClip> beepSound;

    // Start is called before the first frame update
    void Start()
    {
        thisTransform = GetComponent<RectTransform>();
        bugDestroyerMinigame = FindObjectOfType<BugDestroyerMinigame>();
        canvasRt = bugDestroyerMinigame.rt;
        NewTargetPos();
        StartCoroutine(RandomSpeed());
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        CheckBounds();
    }

    private void NewTargetPos()
    {
        float offsetWidth = thisTransform.rect.width / 2;
        float minHoriDist = -canvasRt.rect.width / 2 + offsetWidth;
        float maxHoriDist = canvasRt.rect.width / 2 - offsetWidth;

        float offsetHeight = thisTransform.rect.height / 2;
        float minVertDist = -canvasRt.rect.height / 2 + offsetHeight;
        float maxVertDist = canvasRt.rect.height / 2 - offsetHeight;

        Vector2 newPos = new Vector2(Random.Range(minHoriDist, maxHoriDist), Random.Range(minVertDist, maxVertDist));        
        targetPos = new Vector3(newPos.x, newPos.y, 0);
    }

    private void Move()
    {
        thisTransform.localPosition = Vector3.MoveTowards(thisTransform.localPosition, targetPos, speed*Time.deltaTime);
        if(Vector3.Distance(thisTransform.localPosition, targetPos)<0.1f)
        {
            NewTargetPos();
        }
    }

    private IEnumerator RandomSpeed()
    {
        while(true)
        {
            speed = Random.Range(minSpeed, maxSpeed);
            yield return new WaitForSeconds(Random.Range(0.5f, 2f));
        }
    }

    private void CheckBounds()
    {
        Vector3 pos = thisTransform.localPosition;
        pos.x  = Mathf.Clamp(pos.x, -canvasRt.rect.width / 2, canvasRt.rect.width / 2);
        pos.y = Mathf.Clamp(pos.y, -canvasRt.rect.height / 2, canvasRt.rect.height / 2);
        thisTransform.localPosition = pos;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("bug destroyed");

        int random = Random.Range(0, beepSound.Count);
        SoundFXManager.instance.PlaySound(beepSound[random], transform, 1f);
        Destroy(gameObject);
    }
}
