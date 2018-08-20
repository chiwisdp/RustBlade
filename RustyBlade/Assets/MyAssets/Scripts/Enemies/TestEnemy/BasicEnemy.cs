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
	private void Start() {
		currentHealth = maxHealth;
		_charStats = new CharacterStats (40, 10, 2);
		navAgent = GetComponent<NavMeshAgent>();
		player = GameObject.Find("Character").GetComponent<PlayerStatsController>();
	}
	void FixedUpdate() {
		_insideAgroCollider= Physics.OverlapSphere(transform.position, 10, _agroLayerMask);
		if(_insideAgroCollider.Length>0)
		{
			ChasePlayer();
		}
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
