using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour {

	// Use this for initialization
	public Animator _animator;
	void Start () {
		
	}

	void FixedUpdate()
	{
		if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("SwordSwing")){
			//PlayIdle();
		}
	}
	public void SwordAttack(){
		Debug.Log("SWORD");
		_animator.SetBool("isSwordAttack", true);
		_animator.SetBool("isIdle", false);
		_animator.SetBool("isWalking", false);
	}
	public void DaggerAttack(){

	}
	public void PlayIdle(){
		_animator.SetBool("isIdle", true);
		_animator.SetBool("isSwordAttack", false);
		_animator.SetBool("isWalking", false);
	}
	public void Walking(){
		_animator.SetBool("isIdle", false);
		_animator.SetBool("isWalking", true);
		_animator.SetBool("isSwordAttack", false);
	}
	// Update is called once per frame
	void Update () {
		
	}
}
