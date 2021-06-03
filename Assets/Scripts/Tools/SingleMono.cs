// *************************************************************************************************************
// 创建者: DESKTOP-UK19EAP
// 创建时间: 2019/09/04 11:10:38
// 功能: 
// 版 本：v 1.0.0
// *************************************************************************************************************
using UnityEngine;

public class SingleMono<T>  : MonoBehaviour where T: SingleMono<T>
{
    private static T _instance;
    public static T Instance {
        get {

            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(T)) as T;
                if (_instance == null)
                {
                    GameObject go = new GameObject
                    {
                        name = "Single_" + typeof(T).ToString()
                    };
                    _instance = go.AddComponent<T>();
                    //if (GameObject.Find("DontDestroy"))
                    //    go.transform.SetParent(GameObject.Find("DontDestroy").transform);
                }
            }
            return _instance;
        }
    }
}
