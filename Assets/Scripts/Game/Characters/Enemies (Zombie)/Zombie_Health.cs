using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; 
using UnityEngine.Networking;

//This class has comments in animator lines because it used a 3D Rig Model that´s not my own
public class Zombie_Health : Health {
	public event System.Action deadEvent;
	// public Animator deadAnimator;

	void Awake(){
		changeHealthEvent += ChangeHealthListener;
	}

	public void TakeDamage(string _name, string _tag, System.Action _deadEvent){
		if(!isServer)
			return;
		
		deadEvent = null;
		deadEvent += _deadEvent;
		currentHitActor = _name;
		if(_tag.Contains(Constants.HeadZombieTag))
			currentHealth = 0;
		else if(_tag.Contains(Constants.BodyZombieTag))
			currentHealth -= 50;

	}
	 
	public void ChangeHealthListener(int _health){
		
		if (_health <= 0)
        {
			//Debug.Log("<color=2ccbdc>Destroyed by: "+ currentHitActor +"</color>");
			if(deadEvent != null)
				deadEvent();

			GetComponent<Zombie_Target>().enabled = false;
			GetComponent<NavMeshAgent>().isStopped = true;
			// deadAnimator.SetTrigger(Constants.DeadZombieTrigger);
        }
	}

	void OnDestroy(){
		deadEvent = null;
	}

}
