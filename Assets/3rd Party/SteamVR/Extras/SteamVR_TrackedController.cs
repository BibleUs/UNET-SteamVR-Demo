//======= Copyright (c) Valve Corporation, All rights reserved. ===============
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

public struct ClickedEventArgs
{
	public uint controllerIndex;
	public uint flags;
	public float padX, padY, triggerX;
}

public delegate void ClickedEventHandler(object sender, ClickedEventArgs e);

public class SteamVR_TrackedController : MonoBehaviour
{
	public uint controllerIndex;
	public VRControllerState_t controllerState;
	public bool triggerPressed = false;
	public bool triggerTouched = false;
	public bool steamPressed = false;
	public bool menuPressed = false;
	public bool padPressed = false;
	public bool padTouched = false;
	public bool gripped = false;

	public Vector2 padDirection;
	public float triggerDepth;

	//Input values events
	public event ClickedEventHandler MenuButtonClicked;
	public event ClickedEventHandler MenuButtonUnclicked;
	public event ClickedEventHandler TriggerClicked;
	public event ClickedEventHandler TriggerUnclicked;
	public event ClickedEventHandler TriggerTouched;
	public event ClickedEventHandler TriggerUntouched;
	public event ClickedEventHandler SteamClicked;
	public event ClickedEventHandler PadClicked;
	public event ClickedEventHandler PadUnclicked;
	public event ClickedEventHandler PadTouched;
	public event ClickedEventHandler PadUntouched;
	public event ClickedEventHandler Gripped;
	public event ClickedEventHandler Ungripped;

	// Use this for initialization
	protected virtual void Start()
	{
		if (this.GetComponent<SteamVR_TrackedObject>() == null)
		{
			gameObject.AddComponent<SteamVR_TrackedObject>();
		}

		if (controllerIndex != 0)
		{
			this.GetComponent<SteamVR_TrackedObject>().index = (SteamVR_TrackedObject.EIndex)controllerIndex;
			if (this.GetComponent<SteamVR_RenderModel>() != null)
			{
				this.GetComponent<SteamVR_RenderModel>().index = (SteamVR_TrackedObject.EIndex)controllerIndex;
			}
		}
		else
		{
			controllerIndex = (uint)this.GetComponent<SteamVR_TrackedObject>().index;
		}
	}

	public void SetDeviceIndex(int index)
	{
		this.controllerIndex = (uint)index;
	}

	// public void ClearTriggerClicked(){
	// 	TriggerClicked = null;
	// }
	// public void ClearUnclicked(){
	// 	TriggerUnclicked = null;
	// }
	// public void ClearTriggerClicked(){
	// 	TriggerClicked = null;
	// }

	public virtual void OnMenuClicked(ClickedEventArgs e)
	{
		if (MenuButtonClicked != null)
			MenuButtonClicked(this, e);
	}

	public virtual void OnMenuUnclicked(ClickedEventArgs e)
	{
		if (MenuButtonUnclicked != null)
			MenuButtonUnclicked(this, e);
	}
	public virtual void OnTriggerClicked(ClickedEventArgs e)
	{
		if (TriggerClicked != null)
			TriggerClicked(this, e);
	}

	public virtual void OnTriggerUnclicked(ClickedEventArgs e)
	{
		if (TriggerUnclicked != null)			
			TriggerUnclicked(this, e);
		
	}
	public virtual void OnTriggerTouched(ClickedEventArgs e)
	{
		if (TriggerTouched != null)
			TriggerTouched(this, e);
	}

	public virtual void OnTriggerUntouched(ClickedEventArgs e)
	{
		if (TriggerUntouched != null)
			TriggerUntouched(this, e);
	}


	public virtual void OnSteamClicked(ClickedEventArgs e)
	{
		if (SteamClicked != null)
			SteamClicked(this, e);
	}

	public virtual void OnPadClicked(ClickedEventArgs e)
	{
		if (PadClicked != null)
			PadClicked(this, e);
	}

	public virtual void OnPadUnclicked(ClickedEventArgs e)
	{
		if (PadUnclicked != null)
			PadUnclicked(this, e);
	}

	public virtual void OnPadTouched(ClickedEventArgs e)
	{
		if (PadTouched != null)
			PadTouched(this, e);
	}

	public virtual void OnPadUntouched(ClickedEventArgs e)
	{
		if (PadUntouched != null)
			PadUntouched(this, e);
	}

	public virtual void OnGripped(ClickedEventArgs e)
	{
		if (Gripped != null)
			Gripped(this, e);
	}

	public virtual void OnUngripped(ClickedEventArgs e)
	{
		if (Ungripped != null)
			Ungripped(this, e);
	}

	// Update is called once per frame
	protected virtual void Update()
	{
		var system = OpenVR.System;
		if (system != null && system.GetControllerState(controllerIndex, ref controllerState, (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(VRControllerState_t))))
		{	
			ulong menu = controllerState.ulButtonPressed & (1UL << ((int)EVRButtonId.k_EButton_ApplicationMenu));
			if (menu > 0L && !menuPressed)
			{
				menuPressed = true;
				ClickedEventArgs e;
				e.controllerIndex = controllerIndex;
				e.flags = (uint)controllerState.ulButtonPressed;
				e.padX = controllerState.rAxis0.x;
				e.padY = controllerState.rAxis0.y;
				e.triggerX = controllerState.rAxis1.x;
				OnMenuClicked(e);
			}
			else if (menu == 0L && menuPressed)
			{
				menuPressed = false;
				ClickedEventArgs e;
				e.controllerIndex = controllerIndex;
				e.flags = (uint)controllerState.ulButtonPressed;
				e.padX = controllerState.rAxis0.x;
				e.padY = controllerState.rAxis0.y;
				e.triggerX = controllerState.rAxis1.x;
				OnMenuUnclicked(e);
			}

			ulong trigger = controllerState.ulButtonTouched & (1UL << ((int)EVRButtonId.k_EButton_SteamVR_Trigger));
			if(trigger > 0L)
			{
				triggerTouched = true;
				ClickedEventArgs e;
				e.controllerIndex = controllerIndex;
				e.flags = (uint)controllerState.ulButtonPressed;
				e.padX = controllerState.rAxis0.x;
				e.padY = controllerState.rAxis0.y;
				e.triggerX = controllerState.rAxis1.x;
				triggerDepth = e.triggerX;
				OnTriggerTouched(e);
			} else if (trigger == 0L && triggerTouched)
			{
				triggerTouched = false;
				ClickedEventArgs e;
				e.controllerIndex = controllerIndex;
				e.flags = (uint)controllerState.ulButtonPressed;
				e.padX = controllerState.rAxis0.x;
				e.padY = controllerState.rAxis0.y;
				e.triggerX = controllerState.rAxis1.x;
				triggerDepth = 0;
				OnTriggerUntouched(e);
			}
			trigger = controllerState.ulButtonPressed & (1UL << ((int)EVRButtonId.k_EButton_SteamVR_Trigger));
			if (trigger > 0L)
			{
				ClickedEventArgs e;
				e.controllerIndex = controllerIndex;
				e.flags = (uint)controllerState.ulButtonPressed;
				e.padX = controllerState.rAxis0.x;
				e.padY = controllerState.rAxis0.y;
				e.triggerX = controllerState.rAxis1.x;
				if(!triggerPressed && e.triggerX == 1){
					triggerPressed = true;
					OnTriggerClicked(e);
				}

			}
			else if (trigger == 0L && triggerPressed)
			{
				triggerPressed = false;
				ClickedEventArgs e;
				e.controllerIndex = controllerIndex;
				e.flags = (uint)controllerState.ulButtonPressed;
				e.padX = controllerState.rAxis0.x;
				e.padY = controllerState.rAxis0.y;
				e.triggerX = controllerState.rAxis1.x;
				triggerDepth = 0;
				OnTriggerUnclicked(e);
			}

			ulong pad = controllerState.ulButtonPressed & (1UL << ((int)EVRButtonId.k_EButton_SteamVR_Touchpad));
			if (pad > 0L && !padPressed)
			{
				padPressed = true;
				ClickedEventArgs e;
				e.controllerIndex = controllerIndex;
				e.flags = (uint)controllerState.ulButtonPressed;
				e.padX = controllerState.rAxis0.x;
				e.padY = controllerState.rAxis0.y;
				e.triggerX = controllerState.rAxis1.x;
				OnPadClicked(e);
			}
			else if (pad == 0L && padPressed)
			{
				padPressed = false;
				ClickedEventArgs e;
				e.controllerIndex = controllerIndex;
				e.flags = (uint)controllerState.ulButtonPressed;
				e.padX = controllerState.rAxis0.x;
				e.padY = controllerState.rAxis0.y;
				e.triggerX = controllerState.rAxis1.x;
				OnPadUnclicked(e);
			}
			pad = controllerState.ulButtonTouched & (1UL << ((int)EVRButtonId.k_EButton_SteamVR_Touchpad));
			if (pad > 0L)
			{
				padTouched = true;
				ClickedEventArgs e;
				e.controllerIndex = controllerIndex;
				e.flags = (uint)controllerState.ulButtonPressed;
				e.padX = controllerState.rAxis0.x;
				e.padY = controllerState.rAxis0.y;
				padDirection.x = e.padX;
				padDirection.y = e.padY;
				e.triggerX = controllerState.rAxis1.x;
				if(!padTouched)
					OnPadTouched(e);
			} 
			else if (pad == 0L && padTouched)
			{
				padTouched = false;
				ClickedEventArgs e;
				e.controllerIndex = controllerIndex;
				e.flags = (uint)controllerState.ulButtonPressed;
				e.padX = controllerState.rAxis0.x;
				e.padY = controllerState.rAxis0.y;
				e.triggerX = controllerState.rAxis1.x;
				padDirection = Vector2.zero;
				OnPadUntouched(e);
			}

			ulong grip = controllerState.ulButtonPressed & (1UL << ((int)EVRButtonId.k_EButton_Grip));
			if (grip > 0L && !gripped)
			{
				gripped = true;
				ClickedEventArgs e;
				e.controllerIndex = controllerIndex;
				e.flags = (uint)controllerState.ulButtonPressed;
				e.padX = controllerState.rAxis0.x;
				e.padY = controllerState.rAxis0.y;
				e.triggerX = controllerState.rAxis1.x;
				OnGripped(e);

			}
			else if (grip == 0L && gripped)
			{
				gripped = false;
				ClickedEventArgs e;
				e.controllerIndex = controllerIndex;
				e.flags = (uint)controllerState.ulButtonPressed;
				e.padX = controllerState.rAxis0.x;
				e.padY = controllerState.rAxis0.y;
				e.triggerX = controllerState.rAxis1.x;
				OnUngripped(e);
			}

		}
	}
}
