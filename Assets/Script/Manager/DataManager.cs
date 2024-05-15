using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance = null;

	public Dictionary<string, ItemData> itemDataDic = new Dictionary<string, ItemData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (Instance != this)
                Destroy(this.gameObject);
        }
    }

    private void Start()
	{
		ItemData[] itemDatas = Resources.LoadAll<ItemData>("Scriptable/ItemScriptable");
        foreach (var item in itemDatas)
        {
            if (itemDataDic.ContainsKey(item.Item_Name))
            {
                print($"{item.Item_Name}은/는 중복됩니다.");
            }
			itemDataDic.Add(item.Item_Name, item);
		}
    }
}
