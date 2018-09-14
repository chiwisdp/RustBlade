using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyTest : MonoBehaviour {
	public Transform player;
	static Animator anim;
	NavMeshAgent agent;
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		agent = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update () {
		/* Vector3 pDirection = player.position - this.transform.position;
		float angle = Vector3.Angle(pDirection, this.transform.forward);
		if(Vector3.Distance(player.position, this.transform.position) <10 && angle <30f){
			Vector3 direction = player.position - this.transform.position;
			direction.y =0;
			anim.SetBool("isIdle", false);
			if(direction.magnitude >5 ){
				if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Attack 0")){
					this.transform.Translate (0,0,0.051f);
					anim.SetBool("isWalking", true);
					anim.SetBool("isAttacking", false);
					this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), .1f);

				}
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
		} */
		agent.SetDestination(player.transform.position);
	}
}
