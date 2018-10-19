using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Networking;

public abstract class Health : NetworkBehaviour {

    public delegate void ChangeHealthEventHandler(int _health);

    public event ChangeHealthEventHandler changeHealthEvent;

    public const int maxHealth = 100;
 	[SyncVar(hook = "OnChangeHealth")] public int currentHealth = maxHealth;
    public string currentHitActor;
	public RectTransform healthBar;

    public void OnChangeHealth(int _health){
        healthBar.sizeDelta = new Vector2(_health, healthBar.sizeDelta.y);
        healthBar.anchoredPosition = new Vector2((2 * maxHealth - _health)/2,0);

        if(changeHealthEvent != null)
            changeHealthEvent(_health);
    }

    void OnDestroy(){
        changeHealthEvent = null;
    }

}
