using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEnergy : MonoBehaviour {
	[SerializeField] RawImage _healthBar;
	private float _maxEnergy = 100f;
	public float _currentEnergy =0;
	public float temp_currentEnergy =0;
	void Start () {
		//_currentEnergy = GetComponent<PlayerStatsController>().get;
	}
	void Update () {
		
	}
}
