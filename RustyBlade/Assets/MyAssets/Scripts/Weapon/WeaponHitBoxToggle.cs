using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHitBoxToggle : MonoBehaviour {

	// Use this for initialization
	WeaponController weaponController;
	private void Awake() {
		weaponController = FindObjectOfType<WeaponController>();
	}

	public void EnableHitBox(){
		Debug.Log("Hello WEAPON HIT BOX");
		weaponController.EnbableHitBox();
	}
	public void DisableHitBox(){
		Debug.Log("Bye WEAPON HIT BOX");
		weaponController.DisableHitBox();
	}
}
