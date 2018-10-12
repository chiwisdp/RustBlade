using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour {

	// Use this for initialization
	public Animator _animator;
	void Start () {
		
	}
	
	public void SwordAttack(){

	}
	public void DaggerAttack(){

	}
	public void PlayIdle(){
		_animator.SetBool("isIdle", true);
		_animator.SetBool("isWalking", false);
	}
	public void Walking(){
		_animator.SetBool("isIdle", false);
		_animator.SetBool("isWalking", true);
	}
	// Update is called once per frame
	void Update () {
		
	}
}
