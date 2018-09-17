using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEnergy : MonoBehaviour {
	[SerializeField] RawImage _energyBar;
	[SerializeField] float regenEnergyPerSec =1f;
	[SerializeField] float multiplierRegen =1f;
	private float boostedRegenEnergyPerSec =1f;
	private int _maxEnergy = 0;
	private float _currentEnergy =0;
	public float temp_currentEnergy =0;
	bool _isCharging = false;
	void Start () {
		_maxEnergy = GetComponent<PlayerStatsController>().getPlayerMaxEnergy();
		_currentEnergy = _maxEnergy;
	}
	void Update () {
		//chargeEnergy
		if(_currentEnergy<_maxEnergy){
			UpdateEnergyBarUI();
			RegenEnergy();
		}
	}
	private void RegenEnergy(){
		float eneryToAdd = (regenEnergyPerSec*boostedRegenEnergyPerSec)*Time.deltaTime;
		_currentEnergy =Mathf.Clamp((_currentEnergy+eneryToAdd), 0f , _maxEnergy);
	}
	float EnergyAsPercent(){
		return (float)_currentEnergy/(float)_maxEnergy;
	}
	public void UseEnergy (int _energyUsed){
		float _changedEnergy = _currentEnergy-_energyUsed;
			_currentEnergy = (int)Mathf.Clamp(_changedEnergy, 0, 100);
			UpdateEnergyBarUI();
			
	}
	void UpdateEnergyBarUI(){
		float xValue = -(EnergyAsPercent() / 2f) - 0.5f;
       	_energyBar.uvRect = new Rect(xValue, 0f, 0.5f, 1f);
	}
	public void ChargeEnegy (){
		if(boostedRegenEnergyPerSec != multiplierRegen )
		boostedRegenEnergyPerSec = multiplierRegen;
		
		Debug.Log("ToggleChargeEnegy : "+boostedRegenEnergyPerSec);

	}
	public void StopCharging(){
		if(boostedRegenEnergyPerSec == multiplierRegen )
		boostedRegenEnergyPerSec =1f;
	}
	public int GetCurrentEnergy(){
		return (int)_currentEnergy;
	}
}
