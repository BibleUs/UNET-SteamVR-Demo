using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootDestroyer : MonoBehaviour {
    
    public void DestroyRoot(){
        Destroy(transform.root.gameObject);
    }

}