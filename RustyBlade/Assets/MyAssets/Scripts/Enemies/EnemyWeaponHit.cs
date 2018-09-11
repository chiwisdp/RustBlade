using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponHit : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	void OnTriggerEnter(Collider other)
	{
		Debug.Log(other.name);
		if(other.name =="Hero"){
			other.gameObject.GetComponent<PlayerStatsController>().TakeDamage(1);
		}
	}
}
