using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEnergy : MonoBehaviour {
	[SerializeField] RawImage _energyBar;
	private float _maxEnergy = 0f;
	public float _currentEnergy =0;
	public float temp_currentEnergy =0;
	void Start () {
		_maxEnergy = GetComponent<PlayerStatsController>().getPlayerMaxEnergy();
		_currentEnergy = _maxEnergy;
		Debug.Log("PlayerEnergy : "+_currentEnergy);
	}
	void Update () {
		if(Input.GetMouseButtonDown(0)){
			float _changedEnergy = _currentEnergy-10f;
			_currentEnergy = Mathf.Clamp(_changedEnergy, 0, 100);
			UpdateEnergyBarUI();
			Debug.Log("PlayerEnergy : "+_currentEnergy);
		}
	}
	float EnergyAsPercent(){
		return _currentEnergy/_maxEnergy;
	}
	void UpdateEnergyBarUI(){
		float xValue = -(EnergyAsPercent() / 2f) - 0.5f;
       	_energyBar.uvRect = new Rect(xValue, 0f, 0.5f, 1f);
		Debug.Log("PlayerEnergy : "+xValue +"   "+ EnergyAsPercent());
	}
}
