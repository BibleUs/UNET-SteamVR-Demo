using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public abstract class PlayerMessageHandlerVR : NetworkBehaviour {

    public const short movement_msg = 1337;

    [Serializable]
    public class PlayerMovementMessage : MessageBase {
        public string objectTransformName;
        public Vector3 headPosition;
        public Vector3 leftHandPosition;
        public Vector3 rightHandPosition;
        public Quaternion headRotation;
        public Quaternion leftHandRotation;
        public Quaternion rightHandRotation;
        public float leftTriggerLocalRotationY;
        public float rightTriggerLocalRotationY;
        public float time;
    }

}
