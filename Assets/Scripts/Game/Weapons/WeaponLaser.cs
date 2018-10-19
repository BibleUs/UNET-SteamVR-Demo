using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponLaser : MonoBehaviour {

	private LineRenderer lr;
	public GameObject currentHit;

	// Use this for initialization
	void Start () {
		lr = GetComponent<LineRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		RaycastHit hit;
		if(Physics.SphereCast(transform.position, 0.05f, transform.forward, out hit)){
			if(hit.collider && !hit.transform.tag.Contains(Constants.PlayerPrefix)){
				currentHit = hit.transform.gameObject;
				
				if(lr.enabled)
					lr.SetPosition(1, new Vector3(0,0,hit.distance));
			} else 
				currentHit = null;
		} else {
			if(lr.enabled)
				lr.SetPosition(1, new Vector3(0,0,5000));
		}
	}
}