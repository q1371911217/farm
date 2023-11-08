using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour
{ 
    [SerializeField]
    AudioSource audioSource;

    [SerializeField]
    List<AudioClip> clipList;

    [SerializeField]
    Button btnVolume;

    [SerializeField]
    Text lblBlueGem, lblBlueSpeed, lblRedGem, lblRedSpeed;

    [SerializeField]
    Transform farmRoot, itemRoot;
    [SerializeField]
    GameObject landPref, plantPref, weatherPref;

    [SerializeField]
    Button btnBox;
    [SerializeField]
    Text lblBoxProgress;
    [SerializeField]
    GameObject boxOpen;
    [SerializeField]
    Image boxGetSprite;

    [SerializeField]
    List<Sprite> plantSpriteList, weatherSpriteList;

    [SerializeField]
    GameObject helpLayer;


    private List<Transform> landList = new List<Transform>();
    //private Dictionary<int, BaseItem> itemDict = new Dictionary<int, BaseItem>();
    private int landCount; //當前地數量
    private long blueGemCount, redGemCount;

    private int maxLandCount = 25;

    private int boxProgress = 0;

    private List<int> typeList;
    private List<int> levelList;

    // Start is called before the first frame update
    void Start()
    {
        audioSource.volume = Loading.volumOpen ? 1 : 0;
        btnVolume.transform.Find("spDisable").gameObject.SetActive(!Loading.volumOpen);
        landCount = PlayerPrefs.GetInt("landCount", 6);

        blueGemCount = long.Parse(PlayerPrefs.GetString("blueGemCount", "0"));
        redGemCount = long.Parse(PlayerPrefs.GetString("redGemCount", "0"));

        btnBox.onClick.AddListener(() =>
        {
            onBoxClick();
        });

        typeList = new List<int>();
        levelList = new List<int>();

        for (int i = 0; i < landCount; i++)
        {
            int type = PlayerPrefs.GetInt(string.Format("land_{0}_type", i), -1);
            int level = PlayerPrefs.GetInt(string.Format("land_{0}_level", i), -1);
            typeList.Add(type);
            levelList.Add(level);
        }

        updateLand();
        updateGem();

        StartCoroutine(save());
    }

    IEnumerator save()
    {
        while(true)
        {
            yield return new WaitForSeconds(5);
            PlayerPrefs.SetString("blueGemCount", blueGemCount.ToString());
            PlayerPrefs.SetString("redGemCount", redGemCount.ToString());
        }
    }

    void addBlueGem(int count)
    {
        blueGemCount += count;
       // PlayerPrefs.SetString("blueGemCount", blueGemCount.ToString());
        updateGem();
    }

    void addRedGem(int count)
    {
        redGemCount += count;
        //PlayerPrefs.SetString("redGemCount", redGemCount.ToString());
        updateGem();
    }

    void updateGem()
    {
        lblBlueGem.text = blueGemCount >= 1000 ? string.Format("{0}K", blueGemCount / 1000) : blueGemCount.ToString();
        lblRedGem.text = redGemCount >= 1000 ? string.Format("{0}K", redGemCount / 1000) : redGemCount.ToString();

        checkExtension();
        addListener();
    }

    void updateLand()
    {
        for (int i = 0; i <= landCount; i++)
        {
            int index = i;
            if (i >= landList.Count && i < maxLandCount )
            {                
                GameObject landGo = GameObject.Instantiate(landPref);
                Transform landTrans = landGo.transform;
                landGo.SetActive(true);
                landGo.name = string.Format("land_{0}", i);
                landTrans.SetParent(farmRoot);
                landTrans.localRotation = Quaternion.identity;
                landTrans.localScale = Vector3.one;
                landTrans.localPosition = new Vector3(-279 + ((i ) % 5) * 139, 298 - (i ) / 5 * 152, 0);
                landTrans.GetComponent<Land>().setIndex(index);                
                landList.Add(landTrans);

                if(i < typeList.Count && typeList[i] != -1)
                {
                    generateItem(i, levelList[i], typeList[i]);
                }

                if (i == landCount)
                {
                    landTrans.Find("Image").gameObject.SetActive(true);
                }
            }
            else
            {
                if (i < landList.Count)
                landList[i].Find("Image").gameObject.SetActive(false);
            }

            if (i < landList.Count)
                landList[i].GetComponent<Land>().setOpen(index < landCount);
        }

        checkLandNull();
    }

    void checkExtension()
    {
        if (landCount >= maxLandCount) return;

        long curGem;
        curGem = landList.Count % 2 == 0 ? redGemCount : blueGemCount;

        long needGem = 30 * (long)Mathf.Pow(2, landCount - 6);
       // Debug.LogError("needGem :" + needGem);
        if(curGem >= needGem)
        {
            Transform lastLand = landList[landList.Count - 1];
            lastLand.Find("Image").gameObject.SetActive(true);
            lastLand.Find("Image/Text").gameObject.SetActive(true);
            lastLand.GetComponent<Button>().onClick.RemoveAllListeners();
            lastLand.GetComponent<Button>().onClick.AddListener(() =>
            {
                audioSource.PlayOneShot(clipList[0]);
                if (landList.Count % 2 == 0)
                    redGemCount -= needGem;
                else
                    blueGemCount -= needGem;
                landCount++;
                PlayerPrefs.SetInt("landCount", landCount);
                updateLand();
                updateGem();
               // lastLand.GetComponent<Button>().onClick.RemoveAllListeners();
            });
        }
    }

    void addListener()
    {
        for(int i = 0; i < landCount; i++)
        {
            Land land = landList[i].GetComponent<Land>();
            Button btn = landList[i].GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() =>
            {
                //exchange(land);
            });
        }
    }

    void clcSpeed()
    {
        int blueSpeed = 0;
        int redSpeed = 0;
       for(int i = 0; i < landList.Count; i++)
        {
            Land land = landList[i].GetComponent<Land>();
            if(land.getOpen())
            {
                BaseItem item = land.getItem();
                if(item != null)
                {
                    if(item.getType() == 1)
                    {
                        blueSpeed += item.getProduct();
                    }
                    else
                    {
                        redSpeed += item.getProduct();
                    }
                }
            }
        }
        lblBlueSpeed.text =  string.Format("{0}/s", (blueSpeed / 2f));
        lblRedSpeed.text = string.Format("{0}/s", (redSpeed / 2f));

    }

    void sendMessageGenerate(int index)
    {
        //generateItem(index);
    }

    BaseItem generateItem(int index, int level = -1, int type = -1, bool visiable = true)
    {
        GameObject curPref;
        if(type == -1)
            type = index % 2 == 0 ? 1 : 2;
        curPref = type == 1 ? plantPref : weatherPref;

        GameObject go = GameObject.Instantiate(curPref);
        Transform trans = go.transform;
        go.SetActive(true);
        trans.SetParent(itemRoot);
        trans.localPosition = new Vector3(-279 + ((index ) % 5) * 139, 352 - (index) / 5 * 152, 0);
        trans.localScale = Vector3.one;
        trans.localRotation = Quaternion.identity;
        BaseItem item = trans.GetComponent<BaseItem>();
        if (level == -1)
            level = Random.Range(0, 3);
        item.init(type, level);
        landList[index].GetComponent<Land>().setItem(item);
        landList[index].GetComponent<Land>().setItemVisiable(visiable);

        //itemDict[index] = item;

        clcSpeed();

        return item;
    }

    void checkLandNull()
    {
        if(boxProgress == 100)
        {
            isStartResidueTime = true;
            return;
        }
        if (boxProgress != 0) return;

        bool hasNull = false;
        for (int i = 0; i < landList.Count; i++)
        {
            Land land = landList[i].GetComponent<Land>();
            if (land.getItem() == null && land.getOpen())
            {
                hasNull = true;
                break;
            }
        }
        if(hasNull)
        {
            if(!btnBox.gameObject.activeInHierarchy)
                StartCoroutine(boxShow());
        }
    }

    Land clickFirst, clickSecond;

    //void exchange(Land land)
    //{
    //    if (!land.getOpen())
    //    {
    //        return;
    //    }
    //    if (clickFirst == null)
    //    {
    //        clickFirst = land;
    //        return;
    //    }
    //    else
    //    {
    //        if (land == clickFirst) return;
    //        clickSecond = land;
    //    }

    //    if (clickFirst != null && clickSecond != null)
    //    {
    //        BaseItem item1 = clickFirst.getItem();
    //        BaseItem item2 = clickSecond.getItem();

    //        bool isCombine = false;

    //        if (item2 != null && item1 != null)
    //        {
    //            if (item2.getType() == item1.getType() && item2.getLevel() == item1.getLevel())
    //            {
    //                isCombine = true;
    //            }
    //        }
    //        if (isCombine) //合并
    //        {
    //            item2.levelUp();

    //            item1.destroy();
    //            clickFirst.setItem(null);
    //            if(item2.getLevel() == 8)
    //            {
    //                clickSecond.setItem(null);
    //            }
    //            else
    //            {
    //                clickSecond.save();
    //            }
    //            clcSpeed();
    //            checkLandNull();

    //            audioSource.PlayOneShot(clipList[1]);
    //        }
    //        else //交換
    //        {
    //            clickFirst.setItem(item2);
    //            clickSecond.setItem(item1);

    //            if (item2 != null)
    //                item2.setPosition(new Vector3(-279 + ((clickFirst.getIndex()) % 5) * 139, 298 - (clickFirst.getIndex()) / 5 * 152, 0));
    //            if (item1 != null)
    //                item1.setPosition(new Vector3(-279 + ((clickSecond.getIndex()) % 5) * 139, 298 - (clickSecond.getIndex()) / 5 * 152, 0));
    //        }

    //        clickFirst = null;
    //        clickSecond = null;
    //    }
    //}

    void exchange(Land clickFirst, Land clickSecond)
    {
        
        //Debug.LogError("exchange");
        BaseItem item1 = clickFirst.getItem();
        BaseItem item2 = clickSecond.getItem();

        bool isCombine = false;

        if (item2 != null && item1 != null)
        {
            if (item2.getType() == item1.getType() && item2.getLevel() == item1.getLevel())
            {
                isCombine = true;
            }
        }
        if (isCombine) //合并
        {
            item2.levelUp();

            item1.destroy();
            clickFirst.setItem(null);
            if (item2.getLevel() == 8)
            {
                clickSecond.setItem(null);
            }
            else
            {
                clickSecond.save();
            }
            clcSpeed();
            checkLandNull();

            audioSource.PlayOneShot(clipList[1]);
        }
        else //交換
        {
            clickFirst.setItem(item2);
            clickSecond.setItem(item1);

            if (item2 != null)
                item2.setPosition(new Vector3(-279 + ((clickFirst.getIndex()) % 5) * 139, 352 - (clickFirst.getIndex()) / 5 * 152, 0));
            if (item1 != null)
                item1.setPosition(new Vector3(-279 + ((clickSecond.getIndex()) % 5) * 139, 352 - (clickSecond.getIndex()) / 5 * 152, 0));
        }
    }

    private bool isStartResidueTime = false;
    float residueTime = 0;

    IEnumerator boxShow()
    {
        btnBox.gameObject.SetActive(true);
        while (boxProgress < 100)
        {
            boxProgress += 1;
            if (boxProgress > 100)
                boxProgress = 100;
            btnBox.transform.localPosition = new Vector3(0, -84 + 84 * boxProgress / 100.0f, 0);
            lblBoxProgress.text = string.Format("{0}%", boxProgress);
            yield return new WaitForSeconds(0.05f);
        }
        
        lblBoxProgress.text = "";
        isStartResidueTime = true;
        residueTime = 0;
        //btnBox.onClick.Invoke();   
    }

    void Update()
    {
        if(isStartResidueTime)
        {
            residueTime += Time.deltaTime;
            if(residueTime > 9.99999f)
            {
                btnBox.onClick.Invoke();
            }
        }
    }

    void checkExchange(BaseItem item)
    {
        RectTransform dragRect = item.getRectTransform();

        Rect rect = RectTransToScreenPos(dragRect, Camera.main);

        Rect rectNew = new Rect(rect.center.x - 40, rect.center.y - 40, 40, 40);

        //Debug.LogError(rectNew);
        Land dragItemLand = new Land();
        Land exchangeLand = new Land();
        for (int i = 0; i < landList.Count; i++)
        {
            Land land = landList[i].GetComponent<Land>();
            if (dragItemLand.getIndex() == -1 && land.getItem() == item)
            {
                dragItemLand = land;
            }
            else if (exchangeLand.getIndex() == -1 && land.getOpen() && RectTransToScreenPos(land.getRectTransform(), Camera.main).Overlaps(rectNew))
            {
                exchangeLand = land;
            }
            if (dragItemLand.getIndex() != -1 && exchangeLand.getIndex() != -1)
            {
                break;
            }
        }


        if (dragItemLand.getIndex() != -1 && exchangeLand.getIndex() != -1)
        {
            exchange(dragItemLand, exchangeLand);
        }
        else
        {
            item.backToLocalSource();
        }


    }

    public static Rect RectTransToScreenPos(RectTransform rt, Camera cam)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);
        Vector2 v0 = RectTransformUtility.WorldToScreenPoint(cam, corners[0]);
        Vector2 v1 = RectTransformUtility.WorldToScreenPoint(cam, corners[2]);
        Rect rect = new Rect(v0, v1 - v0);
        return rect;
    }

    void onBoxClick()
    {
        if (boxProgress < 100)
        {
            return;
        }

        isStartResidueTime = false;
        residueTime = 0;

        audioSource.PlayOneShot(clipList[0]);
        int index = -1;
        Land curLand = new Land();
        for (int i = 0; i < landList.Count; i++)
        {
            Land land = landList[i].GetComponent<Land>();
            if (land.getItem() == null && land.getOpen())
            {
                curLand = land;
                land.setStopCheck(true);
                index = i;
                break;
            }
        }

        if (index != -1)
        {
            boxProgress = 0;
            int level = Random.Range(0, 3);
            List<Sprite> spriteList = index % 2 == 0 ? plantSpriteList : weatherSpriteList;
            boxOpen.SetActive(true);
            boxGetSprite.transform.localPosition = new Vector3(8.8f, 0, 0);
            boxGetSprite.sprite = spriteList[level];
            boxGetSprite.SetNativeSize();
            BaseItem item =  generateItem(index, level, -1, false);
            boxGetSprite.transform.DOMove(landList[index].transform.position, 0.5f).SetDelay(0.3f).OnComplete(() =>
            {
                boxOpen.SetActive(false);
                curLand.setStopCheck(false);
                item.setVisiable(true);
                btnBox.gameObject.SetActive(false);

                StartCoroutine(boxShow());
            });
        }
        else
        {
            
            
            
        }
    }


    public void onBtnClick(string name)
    {
        audioSource.PlayOneShot(clipList[0]);
        if(name == "btnHelp")
        {
            helpLayer.gameObject.SetActive(true);
        }
        else if(name == "btnVolume")
        {
            Loading.volumOpen = !Loading.volumOpen;
            audioSource.volume = Loading.volumOpen ? 1 : 0;
            btnVolume.transform.Find("spDisable").gameObject.SetActive(!Loading.volumOpen);
        }
        else if(name == "btnHome")
        {
            SceneManager.LoadSceneAsync("LoginScene");
        }
        else if(name == "btnOk")
        {
            helpLayer.SetActive(false);
        }
    }


    public void OnDestroy()
    {
       // PlayerPrefs.SetInt("landCount", landCount);
        

        landList.Clear();
        typeList.Clear();
        levelList.Clear();

        landList = null;
        typeList = null;
        levelList = null;        
    }
}
