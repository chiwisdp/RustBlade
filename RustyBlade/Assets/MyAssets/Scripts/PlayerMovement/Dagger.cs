using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dagger : MonoBehaviour, IWeapon {
	private Animator _animator;
	public int base_dmg;
	public Renderer _weapon;
    public List<BaseStat> Stats {get; set;}
	public CharacterStats CharacterStats { get; set; }
	public int CurrentDamage { get; set; }
	PlayerEffectsController _effectsController;

    private void Awake()
	{
		_animator=GetComponent<Animator>();
		_effectsController = FindObjectOfType<PlayerEffectsController>();

	}
    public void PerformAction(int _dmg)
    {
		CurrentDamage = _dmg;
		if(_animator.GetCurrentAnimatorStateInfo(0).IsName("Dagger_Idle"))
		{
			_animator.SetTrigger("base_attack");
		}
	}
	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Enviornment"){
			SwordRecoil();
		}
		if(other.tag == "Enemy")
		{
			//SwordRecoil();
			other.GetComponent<IEnemy>().TakeDamage(CurrentDamage);
		}
	}
	void SwordRecoil(){
		_animator.SetTrigger("wall_hit");
	}
	void ShowEffect()
	{
		_effectsController.DisplayDaggerSwingEffect();
	}
    public bool GetIsInUse()
    {
        return (_animator.GetCurrentAnimatorStateInfo(0).IsName("Dagger_Swing"));
    }
	public void ToggleWeaponDisplay(bool _displayStatus){
		_weapon.enabled = _displayStatus;
	}
}
