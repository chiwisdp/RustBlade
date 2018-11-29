using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour {

	// Use this for initialization
	public Animator _animator;
	void Start () {
		
	}
	public void SwordAttack(){
		_animator.SetBool("isSwordAttack", true);
		_animator.SetBool("isDaggerAttack", false);
		_animator.SetBool("isIdle", false);
		_animator.SetBool("isWalking", false);
		_animator.SetBool("isRoll", false);
	}
	public void DaggerAttack(){
		_animator.SetBool("isDaggerAttack", true);
		_animator.SetBool("isIdle", false);
		_animator.SetBool("isSwordAttack", false);
		_animator.SetBool("isWalking", false);
		_animator.SetBool("isRoll", false);
	}
	public void PlayIdle(){
		_animator.SetBool("isIdle", true);
		_animator.SetBool("isDaggerAttack", false);
		_animator.SetBool("isSwordAttack", false);
		_animator.SetBool("isWalking", false);
		_animator.SetBool("isRoll", false);
	}
	public void Walking(){
		_animator.SetBool("isIdle", false);
		_animator.SetBool("isWalking", true);
		_animator.SetBool("isDaggerAttack", false);
		_animator.SetBool("isSwordAttack", false);
		_animator.SetBool("isRoll", false);
	}
	public void PlayRoll(){
		_animator.SetBool("isIdle", false);
		_animator.SetBool("isWalking", false);
		_animator.SetBool("isSwordAttack", false);
		_animator.SetBool("isRoll", true);
		_animator.SetBool("isDaggerAttack", false);
	}
	private void Update() {
		Debug.Log("getIsSwordAttack() : " + getIsSwordAttack());
	}
	// Update is called once per frame
	public bool getIsSwordAttack(){
		return _animator.GetCurrentAnimatorStateInfo(0).IsName("SwordSwing");
	}
	public bool getIsIdle(){
		return _animator.GetCurrentAnimatorStateInfo(0).IsName("Idle");
	}
	public bool getIsWalk(){
		return _animator.GetCurrentAnimatorStateInfo(0).IsName("WalkAni");
	}
	public bool getIsRoll(){
		return _animator.GetCurrentAnimatorStateInfo(0).IsName("Roll");
	}
	public bool getIsDagger(){
		return _animator.GetCurrentAnimatorStateInfo(0).IsName("Dagger");
	}
}
