using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {
    public static CameraManager singleton;

    public Camera physicalCamera;
    public CameraController3D controlledCamera;
    public GameObject followCameraPrefab;
    public GameObject personalCameraPrefab;

    [HideInInspector]
    public CinemachineVirtualCamera activeFollowCamera;
    [HideInInspector]
    public CinemachineVirtualCamera activePersonalCamera;

    [HideInInspector]
    public float blendTime;
    Vector3 cameraOffset;

    private Stack<UnitController> encounterTargets;

    public static float CUTSCENE_TIME = 1.66f;

    private void Awake() {
        singleton = this;
        encounterTargets = new Stack<UnitController>();
    }

    private void Start() {
        
    }

    public void Initialise() {
        controlledCamera.Initialise();
        blendTime = GameObject.Find("Main Camera").GetComponent<CinemachineBrain>().m_DefaultBlend.m_Time;
    }

    // Update is called once per frame
    private void Update() {

    }

    public void AddEncounteredTarget(UnitController target) {
        if (target == null) {
            return;
        }

        encounterTargets.Push(target);

        StartCoroutine(EncounterCutscene());
    }

    public IEnumerator EncounterCutscene() {
        yield return TurnManager.singleton.WaitForWaitingForInput();

        if (encounterTargets.Count < 1) {
            yield break;
        } 

        TurnManager.singleton.StartingCutscene();
        UnitController target = encounterTargets.Pop();
        Transform targetTransform = target.transform.Find("Token");

        TurnOffCameras();
        CreatePersonalCamera(targetTransform);

        GUIController.singleton.HideUI();

        AudioManager.singleton.LowerMusic();

        yield return new WaitForSeconds(blendTime);

        GUIController.singleton.CreateFlyinText(target.myStats.className);

        AudioManager.singleton.Play(target.myStats.encounterSFX, targetTransform, AudioMixers.SFX);

        yield return new WaitForSeconds(CUTSCENE_TIME);

        AudioManager.singleton.RaiseMusic();

        GUIController.singleton.ShowUI();

        if (activePersonalCamera != null) {
            Destroy(activePersonalCamera.gameObject, blendTime + 1f);
        }

        TurnOffCameras();

        TurnManager.singleton.EndedCutscene();

        controlledCamera.TurnOn();
    }

    public void MoveToLocation(Vector2 location) {
        TurnOffCameras();
        controlledCamera.TurnOn();
        controlledCamera.MoveToTarget(location);
    }

    public void MoveToLocation(Node node) {
        MoveToLocation(TileMap.instance.getPositionOfNode(node));
    }

    public void JumpToLocation(Node node) {
        TurnOffCameras();
        controlledCamera.TurnOn();
        controlledCamera.JumpToLocation(TileMap.instance.getPositionOfNode(node) + cameraOffset);
    }

    public void FollowTarget(Transform target) {
        TurnOffCameras();
        GameObject followCamera = Instantiate(followCameraPrefab);
        CinemachineVirtualCamera newFollowCamera = followCamera.GetComponent<CinemachineVirtualCamera>();

        newFollowCamera.Follow = target;
        newFollowCamera.Priority = 10;

        if (activeFollowCamera != null) {
            Destroy(activeFollowCamera.gameObject, blendTime + 1f);
        }
        activeFollowCamera = newFollowCamera;
    }

    public void CreatePersonalCamera(Transform target) {
        TurnOffCameras();
        GameObject personalCamera = Instantiate(personalCameraPrefab);
        CinemachineVirtualCamera newPersonalCamera = personalCamera.GetComponent<CinemachineVirtualCamera>();

        newPersonalCamera.Follow = target;
        newPersonalCamera.LookAt = target;
        newPersonalCamera.Priority = 10;

        if (activePersonalCamera != null) {
            Destroy(activePersonalCamera.gameObject, blendTime + 1f);
        }
        activePersonalCamera = newPersonalCamera;
    }

    public void TurnOffCameras() {
        controlledCamera.TurnOff();

        if (activeFollowCamera != null) {
            activeFollowCamera.Priority = 0;
        }

        if (activePersonalCamera != null) {
            activePersonalCamera.Priority = 0;
        }
    }
}