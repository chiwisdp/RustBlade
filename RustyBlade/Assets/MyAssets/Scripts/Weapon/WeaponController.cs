using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour {
	public GameObject _playerHand;
	public GameObject EquippedWeapon {get; set;}
	IWeapon _equippedItem;
	CharacterStats _charStats;
	void Start(){
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
		Debug.Log(_equippedItem.Stats[0].BaseValue);
	}
	public bool GetIsInUse(){
		return  _equippedItem.GetIsInUse();
	}
	public void PerformAction(){
		if(_equippedItem !=null && !_equippedItem.GetIsInUse())
		_equippedItem.PerformAction(CalculateDamage());
	}
	private int CalculateDamage()
    {
		//OLD equation   int damageToDeal = (_charStats.GetStat(BaseStat.BaseStatType.Power).GetCalculatedStatValue() * 2)+ Random.Range(2, 8);
        int damageToDeal = (_charStats.GetStat(BaseStat.BaseStatType.Power).GetCalculatedStatValue())
            + Random.Range(0, 3);
        damageToDeal += CalculateCrit(damageToDeal);
        //Debug.Log("Damage dealt: " + damageToDeal + "  "+( _charStats.GetStat(BaseStat.BaseStatType.Power).GetCalculatedStatValue()));
        return damageToDeal;
    }
    private int CalculateCrit(int damage)
    {
        if (Random.value <= .10f)
        {
            int critDamage = (int)(damage * Random.Range(.5f, .75f));
            return critDamage;
        }
        return 0;
    }

}
