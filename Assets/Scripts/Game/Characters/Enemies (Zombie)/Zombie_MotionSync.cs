using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Zombie_MotionSync : NetworkBehaviour {

	[SyncVar] private Vector3 syncPos;
	[SyncVar] private float syncYRot;

	private Vector3 lastPos;
	private Quaternion lastRot;
	private Transform myTransform;
	private float lerpRate  =10;
	private float posThreshold = 0.5f;
	private float rotThreshold = 5;

	// Use this for initialization
	void Start (){
		myTransform = transform;
	}
	
	// Update is called once per frame
	void Update (){
		if(!isServer)
			return;
			
		TransmitMotion();
	}

	void FixedUpdate(){
		if(isServer)
			return;

		LerpMotion();
	}

	void TransmitMotion(){
		if(Vector3.Distance(myTransform.position, lastPos) > posThreshold || Quaternion.Angle(myTransform.rotation, lastRot)> rotThreshold){
			lastPos = myTransform.position;
			lastRot = myTransform.rotation;

			syncPos = myTransform.position;
			syncYRot = myTransform.localEulerAngles.y;
		}
	}

	void LerpMotion(){
		myTransform.position = Vector3.Lerp(myTransform.position, syncPos, Time.deltaTime * lerpRate);

		Vector3 newRot = new Vector3(0, syncYRot, 0);
		myTransform.rotation = Quaternion.Lerp(myTransform.rotation, Quaternion.Euler(newRot), Time.deltaTime * lerpRate);
	}


}
