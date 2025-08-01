using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleTon<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance 
    { 
        get
        {
            if (instance == null)
            {
                instance = (T)FindObjectOfType(typeof(T));

                if(instance == null )
                {
                    GameObject obj = new GameObject(typeof(T).Name, typeof(T));
                    instance = obj.GetComponent<T>();
                }
            }
            return instance;
        } 
    }

    private void Awake()
    {
        if(transform.parent != null && transform.root != null)
            DontDestroyOnLoad(this.transform.root.gameObject);              // 최상위의 위치에 따른 결과값
        else 
            DontDestroyOnLoad (this.gameObject);
    }
}
