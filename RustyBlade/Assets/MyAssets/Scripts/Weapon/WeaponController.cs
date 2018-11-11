using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour {
	public GameObject _playerHand;
	public GameObject EquippedWeapon {get; set;}
	IWeapon[] _equippedItem;
	int weaponnum;
	CharacterStats _charStats;
	PlayerEnergy _energyController;
	void Start(){
		_energyController = GetComponent<PlayerEnergy>();
		_charStats = GetComponent<PlayerStatsController>().characterStats;
		_equippedItem = new IWeapon[3];
		 Item item = ItemDatabase.Instance.GetItem("sword");
                //Debug.Log(item.Stats);
		FindObjectOfType<InventoryController>().EquipItem(item);
		Item item2 = ItemDatabase.Instance.GetItem("dagger");
                //Debug.Log(item.Stats);
		FindObjectOfType<InventoryController>().EquipItem(item2);
	}
	public void EquipItem (Item _itemToEquip) {
		if(EquippedWeapon!= null)
		{
			//_charStats.RemoveStatBonus(EquippedWeapon.GetComponent<IWeapon>().Stats);
			//Destroy(_playerHand.transform.GetChild(0).gameObject);
		}
		EquippedWeapon = (GameObject)Instantiate(Resources.Load<GameObject>("Weapons/"+_itemToEquip.ItemName),
		new Vector3(_playerHand.transform.position.x, _playerHand.transform.position.y-1f, _playerHand.transform.position.z)
		,Quaternion.identity);
		_equippedItem[weaponnum] = EquippedWeapon.GetComponent<IWeapon>();
		EquippedWeapon.transform.SetParent(_playerHand.transform);
		_equippedItem[weaponnum].Stats = _itemToEquip.Stats;
		_charStats.AddStatBonus(_itemToEquip.Stats);
		_equippedItem[weaponnum].ToggleWeaponDisplay(false);
		weaponnum++;
	}
	public bool GetIsInUse(){
		return ( _equippedItem[0].GetIsInUse()|| _equippedItem[1].GetIsInUse());
	}
	public int GetEnergyWeaponCost(int weaponnum){
		return  _equippedItem[weaponnum].Stats[2].BaseValue;
	}
	public void PerformAction(int weaponnum){
		if(_equippedItem !=null && !GetIsInUse())
		{
			_equippedItem[weaponnum].PerformAction(CalculateDamage());
			_energyController.UseEnergy(GetEnergyWeaponCost(weaponnum));
			Debug.Log(GetEnergyWeaponCost(weaponnum));
			for (int i = 0; i < _equippedItem.Length-1; i++)
			{
				if(i == weaponnum){
					_equippedItem[weaponnum].ToggleWeaponDisplay(true);
					Debug.Log("daddy "+i);
				}
				else{
					Debug.Log("baby 1 "+i);
					_equippedItem[i].ToggleWeaponDisplay(false);
					Debug.Log("baby "+i);
				}
				Debug.Log(i);
			}
		}
	}
	private int CalculateDamage()
    {
		//OLD equation   int damageToDeal = (_charStats.GetStat(BaseStat.BaseStatType.Power).GetCalculatedStatValue() * 2)+ Random.Range(2, 8);
        int damageToDeal = (_charStats.GetStat(BaseStat.BaseStatType.Power).GetCalculatedStatValue())
            + Random.Range(0, 3);
        damageToDeal += CalculateCrit(damageToDeal);
        return damageToDeal;
    }
    private int CalculateCrit(int damage)
    {
        if (Random.value <= .50f)
        {
            int critDamage = (int)(damage * Random.Range(.5f, .75f));
            return critDamage;
        }
        return 0;
    }

}
