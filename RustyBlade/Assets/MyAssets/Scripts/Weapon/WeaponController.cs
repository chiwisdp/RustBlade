using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour {
	public GameObject _playerHand;
	public GameObject EquippedWeapon {get; set;}
	IWeapon _equippedItem;
	CharacterStats _charStats;
	PlayerEnergy _energyController;
	void Start(){
		_energyController = GetComponent<PlayerEnergy>();
		_charStats = GetComponent<PlayerStatsController>().characterStats;
	}
	public void EquipItem (Item _itemToEquip) {
		if(EquippedWeapon!= null)
		{
			_charStats.RemoveStatBonus(EquippedWeapon.GetComponent<IWeapon>().Stats);
			Destroy(_playerHand.transform.GetChild(0).gameObject);
		}
		EquippedWeapon = (GameObject)Instantiate(Resources.Load<GameObject>("Weapons/"+_itemToEquip.ItemName),
		new Vector3(_playerHand.transform.position.x,_playerHand.transform.position.y+1,_playerHand.transform.position.z)
		,_playerHand.transform.rotation);
		_equippedItem = EquippedWeapon.GetComponent<IWeapon>();
		EquippedWeapon.transform.SetParent(_playerHand.transform);
		_equippedItem.Stats = _itemToEquip.Stats;
		_charStats.AddStatBonus(_itemToEquip.Stats);
		Debug.Log(_equippedItem.Stats[2].StatName+"  " +_equippedItem.Stats[2].BaseValue);
		Debug.Log(_itemToEquip.Stats+ "  "+ _charStats.GetStat(BaseStat.BaseStatType.Energy));
	}
	public bool GetIsInUse(){
		return  _equippedItem.GetIsInUse();
	}
	public int GetEnergyWeaponCost(){
		return  _equippedItem.Stats[2].BaseValue;
	}
	public void PerformAction(){
		if(_equippedItem !=null && !_equippedItem.GetIsInUse())
		{
			_equippedItem.PerformAction(CalculateDamage());
			_energyController.UseEnergy(GetEnergyWeaponCost());
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
