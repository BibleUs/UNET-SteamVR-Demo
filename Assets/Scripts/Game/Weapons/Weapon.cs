using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public enum WeaponHandSide {Left, Right}
public enum WeaponMode {Manual, Auto}

//I created this class for using a weapon system.
public abstract class Weapon : NetworkBehaviour {

#region Events
	event System.Action shootEvent;
	event System.Action reloadEvent;
	event System.Action emptyEvent;
#endregion

#region Parameters
	WeaponMode mode;
	WeaponHandSide handSide;
	Transform shootStartPoint;
	Animator animator;
	int ammoSize;
	int clipSize;
	int ammoAmmount;
	int clipAmmount;
	float shootWaitTime;
	float cadencyTime;

	bool triggerPressed;
	bool isReloading;
	bool isWaitingForNextShoot;
#endregion

#region NetworkBehaviour Functions
	void OnEnable(){
		switch(mode){
			case WeaponMode.Manual: 
				transform.parent.parent.GetComponent<SteamVR_TrackedController>().TriggerClicked += ShootManual; break;
			case WeaponMode.Auto: 
				transform.parent.parent.GetComponent<SteamVR_TrackedController>().TriggerClicked += ShootAuto;
				transform.parent.parent.GetComponent<SteamVR_TrackedController>().TriggerUnclicked += ReleaseTrigger; break;
		}
	}

	void OnDisable(){
		switch(mode){
			case WeaponMode.Manual: 
				transform.parent.parent.GetComponent<SteamVR_TrackedController>().TriggerClicked -= ShootManual; break;
			case WeaponMode.Auto: 
				transform.parent.parent.GetComponent<SteamVR_TrackedController>().TriggerClicked -= ShootAuto;
				transform.parent.parent.GetComponent<SteamVR_TrackedController>().TriggerUnclicked -= ReleaseTrigger; break;
		}		
	}
#endregion

#region Functions
	protected void ShootManual(object sender, ClickedEventArgs e){
		
	}
	protected void ShootAuto(object sender, ClickedEventArgs e){}
	protected void ReleaseTrigger(object sender, ClickedEventArgs e){}
	protected void Reload(){}
#endregion

#region Network Callbacks
	void OnClipModified(int _bullets){

	}

	void OnAmmoModified(int _ammo){

	}

	void OnEndReload(){

	}
#endregion
}
