using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour, IWeapon {
	private Animator _animator;
	public int base_dmg;
	public Renderer _weapon;

    public List<BaseStat> Stats {get; set;}
	public CharacterStats CharacterStats { get; set; }
	public int CurrentDamage { get; set; }
	MeleeWeaponTrail _trail;
	PlayerEffectsController _effectsController;
    private void Awake()
	{
		_animator=GetComponent<Animator>();
		_trail = GetComponent<MeleeWeaponTrail>();
		_effectsController = FindObjectOfType<PlayerEffectsController>();
	}
    public void PerformAction(int _dmg)
    {
		CurrentDamage = _dmg;
		if(_animator.GetCurrentAnimatorStateInfo(0).IsName("Sword_Idle"))
		{
			_animator.SetTrigger("base_attack");
			_trail.Emit = true;
		}
		else{
			_trail.Emit = false;
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

    public bool GetIsInUse()
    {
        return (_animator.GetCurrentAnimatorStateInfo(0).IsName("Sword_Swing"));
    }
	void ShowEffect()
	{
		_effectsController.DisplaySwordSwingEffect();
	}

	public void ToggleWeaponDisplay(bool _displayStatus){
		_weapon.enabled = _displayStatus;
	}
}
