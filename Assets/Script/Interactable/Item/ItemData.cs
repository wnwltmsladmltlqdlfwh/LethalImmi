using UnityEngine;

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
    private string item_prefab_path;
	[SerializeField]
	private string item_icon_path;
	[SerializeField]
	private string itemdata_path;
	
	public string Item_Name { get { return item_name; } }
	public int Item_MinPrice { get { return item_minsellprice; } }
	public int Item_MaxPrice { get { return item_maxsellprice; } }
	public int Item_BuyPrice { get { return item_buyprice; } }
	public int Item_Weight { get { return item_weight; } }
	public bool Item_TwoHanded { get { return item_twoHanded; } }
    public string Item_PrefabPath { get { return item_prefab_path; } }
	public string Item_Icon_Path { get { return item_icon_path; } }
	public string ItemData_Path { get { return itemdata_path; } }
}
