using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEnergy : MonoBehaviour {
	[SerializeField] RawImage _energyBar;
	private int _maxEnergy = 0;
	private int _currentEnergy =0;
	public float temp_currentEnergy =0;
	bool _isCharging = false;
	void Start () {
		_maxEnergy = GetComponent<PlayerStatsController>().getPlayerMaxEnergy();
		_currentEnergy = _maxEnergy;
		Debug.Log("PlayerEnergy : "+_currentEnergy);
	}
	void Update () {
		//chargeEnergy
	}
	float EnergyAsPercent(){
		return (float)_currentEnergy/(float)_maxEnergy;
	}
	public void UseEnergy (int _energyUsed){
		float _changedEnergy = _currentEnergy-_energyUsed;
			_currentEnergy = (int)Mathf.Clamp(_changedEnergy, 0, 100);
			UpdateEnergyBarUI();
			Debug.Log("PlayerEnergy : "+_currentEnergy);
	}
	void UpdateEnergyBarUI(){
		float xValue = -(EnergyAsPercent() / 2f) - 0.5f;
       	_energyBar.uvRect = new Rect(xValue, 0f, 0.5f, 1f);
		Debug.Log("PlayerEnergy : "+xValue +"   "+ EnergyAsPercent());
	}
	public void ToggleChargeEnegy (){
		if(_isCharging){
			_isCharging = false;
		}else{
			_isCharging = true;
		}
	}
	public int GetCurrentEnergy(){
		return (int)_currentEnergy;
	}
}
