using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance = null;

	public Dictionary<string, ItemData> itemDataDic = new Dictionary<string, ItemData>();

    public Dictionary<string, GameObject> mapDataDic = new Dictionary<string, GameObject>();

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
                print($"{item.Item_Name}��/�� �ߺ��˴ϴ�.");
            }
			itemDataDic.Add(item.Item_Name, item);
		}

        GameObject[] mapDatas = Resources.LoadAll<GameObject>("Map");
        foreach (var mapName in mapDatas)
        {
            if (mapDataDic.ContainsKey(mapName.name))
            {
                print($"{mapName.name}��/�� �ߺ��˴ϴ�.");
            }
            mapDataDic.Add(mapName.name, mapName);
        }

        foreach(var printstring in mapDataDic)
        {
            print($"�� Ű : {printstring.Key}, �� �̸� : {printstring.Value.name}");
        }
    }
}
