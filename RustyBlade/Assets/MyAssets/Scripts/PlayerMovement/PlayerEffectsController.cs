using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffectsController : MonoBehaviour {
	public GameObject[] effects;
	// Use this for initialization
	void Start () {
		effects[2].GetComponent<ParticleSystem>().enableEmission = false;
	}
	
	// Update is called once per frame
	public void DisplaySwordSwingEffect () {
		effects[0].SetActive(true);
	}
	public void DisplayDaggerSwingEffect () {
		effects[1].SetActive(true);
	}
	public void DisplayEnergyChargeEffect () {
		effects[2].GetComponent<ParticleSystem>().enableEmission = true;
	}
	public void TurnOffEnergyChargeEffect () {
		effects[2].GetComponent<ParticleSystem>().enableEmission = false;
	}
}
