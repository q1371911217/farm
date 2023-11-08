using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Land : MonoBehaviour
{

    BaseItem curItem;

    float time;

    int index =-1;
    bool isOpen;
    bool stopCheck = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void setIndex(int index)
    {
        this.index = index;
    }

    public int getIndex()
    {
        return index;
    }

    public void setOpen(bool isOpen)
    {
        this.isOpen = isOpen;
    }

    public bool getOpen()
    {
        return isOpen;
    }

    public void setItem(BaseItem item)
    {
        curItem = item;
        save();
    }

    public void setItemVisiable(bool value)
    {
        if(curItem)
        {
            curItem.setVisiable(value);
        }
    }

    public void save()
    {
        if (curItem != null)
        {
            PlayerPrefs.SetInt(string.Format("land_{0}_type", index), curItem.getType());
            PlayerPrefs.SetInt(string.Format("land_{0}_level", index), curItem.getLevel());
        }
        else
        {
            PlayerPrefs.SetInt(string.Format("land_{0}_type", index), -1);
            PlayerPrefs.SetInt(string.Format("land_{0}_level", index), -1);
        }
    }


    public BaseItem getItem()
    {
        return curItem;
    }

    public RectTransform getRectTransform()
    {
        return this.transform.GetComponent<RectTransform>();
    }

    public void setStopCheck(bool value)
    {
        stopCheck = value;
    }

    private void OnDestroy()
    {
       // save();
    }

    // Update is called once per frame
    //void Update()
    //{
    //    //if(!stopCheck && isOpen && null == curItem)
    //    //{
    //    //    time += Time.deltaTime;
    //    //    if(time > 9.9999f)
    //    //    {
    //    //        time = 0;
    //    //        SendMessageUpwards("sendMessageGenerate", index, SendMessageOptions.RequireReceiver);                
    //    //    }
    //    //}
    //}
}
