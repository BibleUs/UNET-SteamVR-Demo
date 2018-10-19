using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerHandsManager : NetworkBehaviour {
    
    public GameObject leftHand;
    public GameObject rightHand; 

    [HideInInspector] [SyncVar(hook = "OnShootRight")] public int rightShootsNum = 0;
    [HideInInspector] [SyncVar(hook = "OnShootLeft")] public int leftShootsNum = 0;

    void Awake(){
        if(isLocalPlayer){
			leftHand.GetComponent<WeaponManager>().enabled = true;
			rightHand.GetComponent<WeaponManager>().enabled = true;
        }
    }

    public void ShootLeft(){
        if(leftShootsNum < 1000){
            leftShootsNum++;
        } else {
            leftShootsNum = 0;
        }
    }

    public void ShootRight(){
        if(rightShootsNum < 1000){
            rightShootsNum++;
        } else {
            rightShootsNum = 0;
        }
    }

    void OnShootLeft(int num){
        leftHand.GetComponent<Animator>().SetTrigger(Constants.weaponShootTrigger);
    }

    void OnShootRight(int num){
        rightHand.GetComponent<Animator>().SetTrigger(Constants.weaponShootTrigger);
    }
}
