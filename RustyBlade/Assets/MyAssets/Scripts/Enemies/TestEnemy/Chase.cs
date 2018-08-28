﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chase : MonoBehaviour {
	public Transform player;
	static Animator anim;
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 pDirection = player.position - this.transform.position;
		float angle = Vector3.Angle(pDirection, this.transform.forward);
		if(Vector3.Distance(player.position, this.transform.position) <10 && angle <30f){
			Vector3 direction = player.position - this.transform.position;
			direction.y =0;
			this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), .1f);
			anim.SetBool("isIdle", false);
			if(direction.magnitude >5 ){
				//walk
				this.transform.Translate (0,0,0.051f);
				anim.SetBool("isWalking", true);
				anim.SetBool("isAttacking", false);
			}else{
				//attack
				anim.SetBool("isAttacking", true);
				anim.SetBool("isWalking", false);
			}
		}else{
			//idle
			anim.SetBool("isIdle", true);
			anim.SetBool("isWalking", false);
			anim.SetBool("isAttacking", false);
		}
	}
}
