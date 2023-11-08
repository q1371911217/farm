using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class BaseItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    List<Sprite> spriteList;

    [SerializeField]
    Image img;

    [SerializeField]
    Image imgGem;
    [SerializeField]
    Text lblGet;
    [SerializeField]
    Animation animation;
    [SerializeField]
    Animator animator;

    [SerializeField]
    List<AnimationClip> clipList;
    [SerializeField]
    List<RuntimeAnimatorController> ctrlList;


    protected List<int> produceList = new List<int>() { 1, 2, 4, 8, 16, 32, 64, 128, 256 };

    protected int level = 0;
    protected int type = 0;

    protected float time = 0;

    Vector3 startPos = new Vector3(-18, 0, 0);

    Tweener tweener;

    RectTransform rt;
    Vector3 sourcePos;

    void Start()
    {
        rt = transform.gameObject.GetComponent<RectTransform>();
        
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time > 1.9999f)
        {
            if (type == 1)
                SendMessageUpwards("addBlueGem", produceList[level], SendMessageOptions.RequireReceiver);
            else
                SendMessageUpwards("addRedGem", produceList[level], SendMessageOptions.RequireReceiver);

           // StartCoroutine(show());

            imgGem.gameObject.SetActive(true);
            imgGem.transform.localPosition = startPos;
            lblGet.text = produceList[level].ToString();
            tweener = imgGem.transform.DOLocalMoveY(41, 0.5f).OnComplete(() =>
            {
                imgGem.gameObject.SetActive(false);
            });

            time = 0;
        }
    }

    public void init(int type, int level)
    {
        this.type = type;
        this.level = level;
        img.sprite = spriteList[level];
        img.SetNativeSize();

        checkAnim();
    }

    public void levelUp()
    {
        this.level++;
        checkAnim();
        img.sprite = spriteList[level];
        img.SetNativeSize();

        time = 0;

        if(level == 8)
        {
            if (tweener != null)
                tweener.Kill();

            imgGem.gameObject.SetActive(true);
            imgGem.transform.localPosition = startPos;
            lblGet.text = "500000";
            
            imgGem.transform.DOLocalMoveY(41, 2).OnComplete(() =>
            {
                imgGem.gameObject.SetActive(false);
                destroy();
            });
            if (type == 1)
                SendMessageUpwards("addBlueGem", 500000, SendMessageOptions.RequireReceiver);
            else
                SendMessageUpwards("addRedGem", 500000, SendMessageOptions.RequireReceiver);

        }

        
    }

    void checkAnim()
    {
        if(type == 2)
        {
            if (level == 1)
            {
                //animation.AddClip(clipList[0], "111");
                animation.clip = clipList[0];

               // animator.enabled = true;
                animator.runtimeAnimatorController = ctrlList[0];
            }
            else if(level == 2)
            {
                animation.clip = clipList[1];
               // animator.enabled = true;
                animator.runtimeAnimatorController = ctrlList[1];
            }
            else if(level == 4)
            {
                animation.clip = clipList[2];
                animator.runtimeAnimatorController = ctrlList[2];
            }
            else if (level == 5)
            {
                animation.clip = clipList[3];
                animator.runtimeAnimatorController = ctrlList[3];
            }
            else if (level == 7)
            {
                animation.clip = clipList[4];
                animator.runtimeAnimatorController = ctrlList[4];
            }
            else if (level == 8)
            {
                animation.clip = clipList[5];
                animator.runtimeAnimatorController = ctrlList[5];
            }
            else
            {
                animation.clip = null;
               // animator.enabled = false;
                animator.runtimeAnimatorController = null;
            }
                
        }
    }

    public void setVisiable(bool value)
    {
        img.enabled = value;
    }

    public int getType()
    {
        return type;
    }

    public int getLevel()
    {
        return level;
    }

    public int getProduct()
    {
        return produceList[level];
    }

    //IEnumerator show()
    //{
    //    imgGem.gameObject.SetActive(true);
    //    lblGet.text = produceList[level].ToString();

    //    yield return new WaitForSeconds(1);

    //    imgGem.gameObject.SetActive(false);
    //}

    public void setPosition(Vector3 localPos)
    {
        this.transform.localPosition = localPos;
    }

    public RectTransform getRectTransform()
    {
        return rt;
    }

    public void destroy()
    {
        GameObject.Destroy(this.gameObject);
    }

    public void backToLocalSource()
    {
        rt.position = sourcePos;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
        sourcePos = rt.position;
        SetDraggedPosition(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // throw new System.NotImplementedException();
        SetDraggedPosition(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // throw new System.NotImplementedException();
        SetDraggedPosition(eventData);

        SendMessageUpwards("checkExchange", this, SendMessageOptions.RequireReceiver);
    }

    private void SetDraggedPosition(PointerEventData eventData)
    {
        Vector3 globalMousePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rt, eventData.position, eventData.pressEventCamera, out globalMousePos))
        {
            rt.position = globalMousePos;
        }
    }
}
