using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

[System.Serializable] public class BulletHitEventHandler : UnityEvent<GameObject,WeaponHandSide>{}; 

//This class have been used in the current VR Player weapons.
public class Gun_old : NetworkBehaviour {

    [HideInInspector] public BulletHitEventHandler ShootEvent;
	[SerializeField] private Transform gunTrigger;
	[SerializeField] private WeaponHandSide handSide;
	[SerializeField] private WeaponMode mode;
	[SerializeField] private bool holdTrigger = false;
	private float cadency = 0.12f;
    public Transform laserPoint;
	private SteamVR_TrackedController controller;

	void Awake(){
		laserPoint.gameObject.SetActive(true);
		controller = GetComponent<SteamVR_TrackedController>();
	}

	void Start(){
	}

	void OnEnable(){
		if(mode == WeaponMode.Manual)
			controller.TriggerClicked += ShootManual;
		else if(mode == WeaponMode.Auto){
			controller.TriggerClicked += ShootAuto;
			controller.TriggerUnclicked += ReleaseAuto;
		}
	}

	void Update(){
		gunTrigger.localRotation = Quaternion.Euler(0,-26 * controller.triggerDepth,0);
	}

	
	/// <summary>
	/// From delegate SteamVR_TrackedController.ClickedEventHandler
	/// </summary>
	public void ShootManual(object sender, ClickedEventArgs e){
		if(ShootEvent != null){
			if(laserPoint.gameObject.activeInHierarchy)
				ShootEvent.Invoke(laserPoint.GetComponent<WeaponLaser>().currentHit, handSide);
		}
	}

	/// <summary>
	/// From delegate SteamVR_TrackedController.ClickedEventHandler
	/// </summary>
	public void ShootAuto(object sender, ClickedEventArgs e){
		holdTrigger = true;
		OnShootAuto();
	}

	void OnShootAuto(){
		StartCoroutine(ShootAutoCoroutine());
	}

	IEnumerator ShootAutoCoroutine(){
		if(ShootEvent != null){
			if(laserPoint.gameObject.activeInHierarchy)
				ShootEvent.Invoke(laserPoint.GetComponent<WeaponLaser>().currentHit, handSide);
		}
		yield return new WaitForSeconds(cadency);
		if(holdTrigger)
			OnShootAuto();
	}

	/// <summary>
	/// From delegate SteamVR_TrackedController.ClickedEventHandler
	/// </summary>
	public void ReleaseAuto(object sender, ClickedEventArgs e){
		holdTrigger = false;
	}
	
	void OnDisable(){
		//ShootEvent = null;
	}
	
}
