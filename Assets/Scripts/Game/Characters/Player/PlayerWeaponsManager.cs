using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerWeaponsManager : NetworkBehaviour {
    
    public Animator leftGunAnimator;
    public Animator rightGunAnimator; 

    [HideInInspector] [SyncVar(hook = "OnShootRight")] public int rightShootsNum = 0;
    [HideInInspector] [SyncVar(hook = "OnShootLeft")] public int leftShootsNum = 0;

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
        leftGunAnimator.SetTrigger(Constants.weaponShootTrigger);
    }

    void OnShootRight(int num){
        rightGunAnimator.SetTrigger(Constants.weaponShootTrigger);
    }
}
