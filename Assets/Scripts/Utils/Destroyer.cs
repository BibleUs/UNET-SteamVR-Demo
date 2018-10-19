using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour {

    public bool destroyOnAwake;
    public float timeToDestroy;

    void Start(){
        if(destroyOnAwake)
            Destroy(gameObject, timeToDestroy);
    }
    

	public void SelfDestroy(){
        Destroy(gameObject);
    }
}
