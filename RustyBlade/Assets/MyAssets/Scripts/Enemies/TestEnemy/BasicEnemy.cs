using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicEnemy : MonoBehaviour, IEnemy
{
	private float currentHealth;
	private CharacterStats _charStats;

	private NavMeshAgent navAgent;
	private Collider[] _insideAgroCollider;
	private PlayerStatsController player;
	public float maxHealth, power, toughness;
	public LayerMask _agroLayerMask;
	static Animator anim;
	private void Start() {
		currentHealth = maxHealth;
		_charStats = new CharacterStats (40, 10, 100,2);
		anim = GetComponent<Animator>();
		navAgent = GetComponent<NavMeshAgent>();
		player = GameObject.Find("Hero").GetComponent<PlayerStatsController>();
	}
	void FixedUpdate() {
	Vector3 pDirection = player.transform.position - this.transform.position;
		float angle = Vector3.Angle(pDirection, this.transform.forward);
		if(Vector3.Distance(player.transform.position, this.transform.position) <15 && angle <90f){
			Vector3 direction = player.transform.position - this.transform.position;
			direction.y =0;
			anim.SetBool("isIdle", false);
			if(direction.magnitude >5 ){
				if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Attack 0")){
					navAgent.SetDestination(player.transform.position);
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
		}
	}
	public float getEnemyHealthPercentage(){
        return ((float)this.currentHealth/(float)this.maxHealth);
    }
	void ChasePlayer(){

		navAgent.SetDestination(this.player.transform.position);
		Debug.Log(this.player.transform.position);
	}
    public void PreformAttack()
    {
        player.TakeDamage(1);
    }

    public void TakeDamage(int amount)
    {
		currentHealth -= amount;
		if(currentHealth <=0)
		{
			Kill();
		}
    }
	void Kill(){
		Destroy(gameObject);
	}
}
