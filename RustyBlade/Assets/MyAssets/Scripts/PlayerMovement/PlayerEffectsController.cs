using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffectsController : MonoBehaviour {
	public GameObject[] effects;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	public void DisplaySwordSwingEffect () {
		effects[0].SetActive(true);
	}
}
