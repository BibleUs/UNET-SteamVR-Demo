using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using PlayerManager;
using UnityEngine.UI;

public class NetworkPlayerVR : PlayerMessageHandlerVR {
    
    [Header("Player Properties")]
    public string playerID;

    [Header("Ship Movement Properties")]
    public bool canSendNetworkMovement;
    public float speed;
    public float networkSendRate = 5;
    public float timeBetweenMovementStart;
    public float timeBetweenMovementEnd;

    [Header("Lerping Properties")]
    public bool isLerpingPosition;
    public bool isLerpingRotation;
    public bool isLerpingWeaponTrigger;
    public Vector3 realHeadPosition;
    public Vector3 realLeftHandPosition;
    public Vector3 realRightHandPosition;
    public Quaternion realHeadRotation;
    public Quaternion realLeftHandRotation;
    public Quaternion realRightHandRotation;
    public float timeStartedLerping;
    public float timeToLerp;
    [HideInInspector] public float realLeftTriggerLocalRotationY;
    [HideInInspector] public float realRightTriggerLocalRotationY;
    [HideInInspector] public Vector3 lastRealHeadPosition;
    [HideInInspector] public Vector3 lastRealLeftHandPosition;
    [HideInInspector] public Vector3 lastRealRightHandPosition;
    [HideInInspector] public Quaternion lastRealHeadRotation;
    [HideInInspector] public Quaternion lastRealRightHandRotation;
    [HideInInspector] public Quaternion lastRealLeftHandRotation;
    [HideInInspector] public float lastRealLeftTriggerLocalRotationY;
    [HideInInspector] public float lastRealRightTriggerLocalRotationY;
    
    [Header("Models")]
    public PlayerVRModel hostModel;
    public PlayerVRModel clientModel;

    [Header("UI")]
    public Canvas playerCanvas;
    public Text killsText;

    public GameObject explosionTestProbe;

    private Transform headTransform;
    private Transform rightHandTransform;
    private Transform leftHandTransform;
    private Transform leftTriggerTransform;
    private Transform rightTriggerTransform;

    private SteamVR_ControllerManager controlManager;
    [SyncVar(hook="OnKillCountUpdate")] private int killsCount = 0;


#region NetworkBehaviour functions
    
    private void Start() {
        playerID = Constants.PlayerPrefix + GetComponent<NetworkIdentity>().netId.ToString();
        transform.name = playerID;
        Manager.Instance.AddPlayerToConnectedPlayers(playerID, gameObject);

        if (isLocalPlayer)
        {
            Destroy(clientModel.gameObject);
            hostModel.gameObject.SetActive(true);
            Manager.Instance.SetLocalPlayerID(playerID);

            controlManager = GetComponentInChildren<SteamVR_ControllerManager>();
            controlManager.left.GetComponent<Gun_old>().ShootEvent.AddListener(Fire);
            controlManager.right.GetComponent<Gun_old>().ShootEvent.AddListener(Fire);

            headTransform = hostModel.head;
            leftHandTransform = hostModel.leftHand;
            rightHandTransform = hostModel.rightHand;
            leftTriggerTransform = hostModel.leftTrigger;
            rightTriggerTransform = hostModel.rightTrigger;

            canSendNetworkMovement = false;
            RegisterNetworkMessages();
        }
        else
        {
            hostModel.leftHand.SetParent(clientModel.transform);
            hostModel.rightHand.SetParent(clientModel.transform);
            clientModel.leftHand = hostModel.leftHand;
            clientModel.rightHand = hostModel.rightHand;
            clientModel.leftTrigger = hostModel.leftTrigger;
            clientModel.rightTrigger = hostModel.rightTrigger;

            Destroy(hostModel.gameObject);
            clientModel.gameObject.SetActive(true);

            isLerpingPosition = false;
            isLerpingRotation = false;
            isLerpingWeaponTrigger = false;

            headTransform = clientModel.head;
            leftHandTransform = clientModel.leftHand;
            rightHandTransform = clientModel.rightHand;
            leftTriggerTransform = clientModel.leftTrigger;
            rightTriggerTransform = clientModel.rightTrigger;

            realHeadPosition = headTransform.localPosition;
            realLeftHandPosition = leftHandTransform.position;
            realRightHandPosition = rightHandTransform.position;
            realHeadRotation = headTransform.rotation;
            realLeftHandRotation = leftHandTransform.rotation;
            realRightHandRotation = rightHandTransform.rotation;
        }

        playerCanvas.transform.SetParent(headTransform);
    }
    
    public override void OnStartClient(){
        OnKillCountUpdate(killsCount);
    }

    private void Update(){
        if(isLocalPlayer)
            UpdatePlayerBehaviour();
    }

    private void FixedUpdate(){
        if(!isLocalPlayer)
            NetworkLerp();
    }

    void OnDestroy(){
         Manager.Instance.RemovePlayerFromConnectedPlayers(playerID);
    }

#endregion

#region Network functions 

    private IEnumerator StartNetworkSendCooldown(){
        timeBetweenMovementStart = Time.time;
        yield return new WaitForSeconds((1 / networkSendRate));
        SendNetworkMovement();
    }

    private void SendNetworkMovement(){
        timeBetweenMovementEnd = Time.time;
        SendMovementMessage(playerID, headTransform.position, leftHandTransform.position, rightHandTransform.position,
            headTransform.rotation, leftHandTransform.rotation, rightHandTransform.rotation,
            leftTriggerTransform.localRotation.y, rightTriggerTransform.localRotation.y,
            (timeBetweenMovementEnd - timeBetweenMovementStart));
        canSendNetworkMovement = false;
    }

    public void SendMovementMessage(string _playerID, Vector3 _hPosition, Vector3 _lhPosition, Vector3 _rhPosition, 
                                    Quaternion _hRotation, Quaternion _lhRotation, Quaternion _rhRotation, 
                                    float _ltRotationY, float _rtRotationY, 
                                    float _timeToLerp){
        PlayerMovementMessage _msg = new PlayerMovementMessage()
        {
            objectTransformName = _playerID,
            headPosition = _hPosition,
            leftHandPosition = _lhPosition,
            rightHandPosition = _rhPosition,
            headRotation = _hRotation,
            leftHandRotation = _lhRotation,
            rightHandRotation = _rhRotation,
            leftTriggerLocalRotationY = _ltRotationY,
            rightTriggerLocalRotationY = _rtRotationY,
            time = _timeToLerp
        };

        NetworkManager.singleton.client.Send(movement_msg, _msg);
    }

    private void RegisterNetworkMessages(){
        NetworkManager.singleton.client.RegisterHandler(movement_msg, OnReceiveMovementMessage);
    }

    public void ReceiveMovementMessage(Vector3 _hPosition, Vector3 _lhPosition, Vector3 _rhPosition, 
                                    Quaternion _hRotation, Quaternion _lhRotation, Quaternion _rhRotation, 
                                    float _ltRotationY, float _rtRotationY, 
                                    float _timeToLerp){
        lastRealHeadPosition = realHeadPosition;
        lastRealLeftHandPosition = realLeftHandPosition; 
        lastRealRightHandPosition = realRightHandPosition;
        lastRealHeadRotation = realHeadRotation;
        lastRealLeftHandRotation = realLeftHandRotation;
        lastRealRightHandRotation = realRightHandRotation;
        lastRealLeftTriggerLocalRotationY = realLeftTriggerLocalRotationY;
        lastRealRightTriggerLocalRotationY = realRightTriggerLocalRotationY;
        
        realHeadPosition = _hPosition;
        realLeftHandPosition = _lhPosition;
        realRightHandPosition = _rhPosition;
        realHeadRotation = _hRotation;
        realLeftHandRotation = _lhRotation;
        realRightHandRotation = _rhRotation;
        realLeftTriggerLocalRotationY = _ltRotationY;
        realRightTriggerLocalRotationY = _rtRotationY;
        
        timeToLerp = _timeToLerp;

        if(realHeadPosition != headTransform.position){
            isLerpingPosition = true;
        }

        if(realHeadRotation.eulerAngles != headTransform.rotation.eulerAngles){
            isLerpingRotation = true;
        }

        if(realLeftTriggerLocalRotationY != leftTriggerTransform.rotation.y ||
            realRightTriggerLocalRotationY != rightTriggerTransform.rotation.y){
            isLerpingWeaponTrigger = true;
        }

        timeStartedLerping = Time.time;
    }

#endregion

#region Player Functions
    private void UpdatePlayerBehaviour(){
        if (!canSendNetworkMovement){
            canSendNetworkMovement = true;
            StartCoroutine(StartNetworkSendCooldown());
        } 
    }

    /// <summary>
	/// From delegate HandgunController.BulletSpawnEventHandler
	/// </summary>
    public void Fire(GameObject hit, WeaponHandSide handSide){
        //CmdHit(hit);
        if(hit != null && hit.tag.Contains(Constants.DefaultZombieTag)){
            CmdFire(hit.transform.root.gameObject, handSide, hit.tag);
        } else {
            CmdFire(null, handSide, "");
        }
    }

    [Command] public void CmdHit(GameObject _hit){
        if(explosionTestProbe != null){
            var explosionHit = (GameObject)Instantiate(
                explosionTestProbe,
                _hit.transform.position,
                Quaternion.identity);

            // Spawn the explosion particle system on the Clients
            NetworkServer.Spawn(explosionTestProbe);

            // Destroy the bullet after 2 seconds
            Destroy(explosionTestProbe, 3.0f);
        }
    }
 
    [Command] public void CmdFire(GameObject _hit, WeaponHandSide _handSide, string _tag){
        if(_handSide == WeaponHandSide.Left)
            GetComponent<PlayerWeaponsManager>().ShootLeft();
        else 
            GetComponent<PlayerWeaponsManager>().ShootRight();

        if(_hit != null){
            var health = _hit.GetComponent<Zombie_Health>();
            if (health != null)
                health.TakeDamage(playerID, _tag, KillEnemy);
        }
    }
 
    private void NetworkLerp(){   

        if(isLerpingPosition){
            float lerpPercentage = (Time.time - timeStartedLerping) / timeToLerp;

            headTransform.position = Vector3.Lerp(lastRealHeadPosition, realHeadPosition, lerpPercentage);
            leftHandTransform.position = Vector3.Lerp(lastRealLeftHandPosition, realLeftHandPosition, lerpPercentage);
            rightHandTransform.position = Vector3.Lerp(lastRealRightHandPosition, realRightHandPosition, lerpPercentage);
        }

        if(isLerpingRotation){
            float lerpPercentage = (Time.time - timeStartedLerping) / timeToLerp;

            headTransform.rotation = Quaternion.Lerp(lastRealHeadRotation, realHeadRotation, lerpPercentage);
            leftHandTransform.rotation = Quaternion.Lerp(lastRealLeftHandRotation, realLeftHandRotation, lerpPercentage);
            rightHandTransform.rotation = Quaternion.Lerp(lastRealRightHandRotation, realRightHandRotation, lerpPercentage);
        }

        if(isLerpingWeaponTrigger){
            float lerpPercentage = (Time.time - timeStartedLerping) / timeToLerp;
            
            leftTriggerTransform.localRotation = Quaternion.Lerp(Quaternion.Euler(0,lastRealLeftTriggerLocalRotationY,0), Quaternion.Euler(0,realLeftTriggerLocalRotationY,0), lerpPercentage);
            rightTriggerTransform.localRotation = Quaternion.Lerp(Quaternion.Euler(0,lastRealRightTriggerLocalRotationY,0), Quaternion.Euler(0,realRightTriggerLocalRotationY,0), lerpPercentage);
        }

    }

#endregion 

#region Callbacks
    
    private void OnReceiveMovementMessage(NetworkMessage _message){

        PlayerMovementMessage _msg = _message.ReadMessage<PlayerMovementMessage>();

        if (_msg.objectTransformName != transform.name){
            Manager.Instance.ConnectedPlayers[_msg.objectTransformName]
                .GetComponent<NetworkPlayerVR>().ReceiveMovementMessage(_msg.headPosition, _msg.leftHandPosition, _msg.rightHandPosition
                    , _msg.headRotation, _msg.leftHandRotation, _msg.rightHandRotation 
                    , _msg.leftTriggerLocalRotationY, _msg.rightTriggerLocalRotationY, _msg.time);
        }
    }

    public void KillEnemy(){
        killsCount++;
    }

    public void OnKillCountUpdate(int _killsCount){
        killsText.text = "Kills\n" + _killsCount.ToString();
    }

#endregion

}