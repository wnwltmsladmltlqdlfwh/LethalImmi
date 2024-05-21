using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance = null;

	public Dictionary<string, ItemData> itemDataDic = new Dictionary<string, ItemData>();
    public Dictionary<string, string> itemPrefabDic = new Dictionary<string, string>();
    public Dictionary<string, GameObject> mapDataDic = new Dictionary<string, GameObject>();

    public Dictionary<string, GameObject> monsterDic = new Dictionary<string, GameObject>();

	private void Awake()
    {
		if (Instance != null && Instance != this)
		{
			Destroy(this.gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(this.gameObject);
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
            itemPrefabDic.Add(item.Item_Name, item.Item_PrefabPath);
		}

        GameObject[] mapDatas = Resources.LoadAll<GameObject>("Map");
        foreach (var mapName in mapDatas)
        {
            if (mapDataDic.ContainsKey(mapName.name))
            {
                print($"{mapName.name}은/는 중복됩니다.");
            }
            mapDataDic.Add(mapName.name, mapName);
        }

        GameObject[] monsterDatas = Resources.LoadAll<GameObject>("Monster");
        foreach (var monster in monsterDatas)
        {
            if (mapDataDic.ContainsKey(monster.name))
            {
                print($"{monster.name}은/는 중복됩니다.");
            }
            mapDataDic.Add(monster.name, monster);
        }

        foreach (var printstring in mapDataDic)
        {
            print($"맵 키 : {printstring.Key}, 맵 이름 : {printstring.Value.name}");
        }
    }
}
