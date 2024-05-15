using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ItemData", menuName = "Item_ScriptableObject/Data", order = int.MaxValue)]
public class ItemData : ScriptableObject
{

	[SerializeField]
	private string item_name;
	[SerializeField]
	private int item_minsellprice;
	[SerializeField]
	private int item_maxsellprice;
	[SerializeField]
	private int item_buyprice;
	[SerializeField]
	private int item_weight;
	[SerializeField]
	private bool item_twoHanded;
	[SerializeField]
	private GameObject item_prefab;
	[SerializeField]
	private Sprite item_icon;
	
	public string Item_Name { get { return item_name; } }
	public int Item_MinPrice { get { return item_minsellprice; } }
	public int Item_MaxPrice { get { return item_maxsellprice; } }
	public int Item_BuyPrice { get { return item_buyprice; } }
	public int Item_Weight { get { return item_weight; } }
	public bool Item_TwoHanded { get { return item_twoHanded; } }
	public string Item_PrefabName { get { return item_prefab.name; } }
	public GameObject Item_Prefab {  get { return item_prefab; } }
	public Sprite Item_Icon { get { return item_icon; } }
}
