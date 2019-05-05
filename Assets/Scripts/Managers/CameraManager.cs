﻿using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {
    public static CameraManager instance;

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

    private Stack<UnitController> encounterTargets;

    public static float CUTSCENE_TIME = 1.66f;

    private void Awake() {
        instance = this;
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
        yield return TurnManager.instance.WaitForWaitingForInput();

        if (encounterTargets.Count < 1) {
            yield break;
        }

        TurnManager.instance.StartingCutscene();
        UnitController target = encounterTargets.Pop();
        Transform targetTransform = target.transform.Find("Token");

        TurnOffCameras();
        CreatePersonalCamera(targetTransform);

        GUIController.instance.HideUI();

        AudioManager.instance.LowerMusic();

        yield return new WaitForSeconds(blendTime);

        GUIController.instance.CreateFlyinText(target.myStats.className);

        PlayOptions encounterSFX = new PlayOptions(target.myStats.encounterSFX, targetTransform);
        encounterSFX.audioMixer = AudioMixers.SFX;
        AudioManager.instance.Play(encounterSFX);

        yield return new WaitForSeconds(CUTSCENE_TIME);

        AudioManager.instance.RaiseMusic();

        GUIController.instance.ShowUI();

        if (activePersonalCamera != null) {
            Destroy(activePersonalCamera.gameObject, blendTime + 1f);
        }

        TurnOffCameras();

        TurnManager.instance.EndedCutscene();

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
        controlledCamera.JumpToLocation(TileMap.instance.getPositionOfNode(node));
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

    public void ShakeCamera() {
        StartCoroutine(_ProcessShake(1f, 0.125f));
    }

    private IEnumerator _ProcessShake(float shakeIntensity = 1f, float shakeTiming = 0.5f) {
        Noise(1, shakeIntensity);
        yield return new WaitForSeconds(shakeTiming);
        Noise(0, 0);
    }

    public void Noise(float amplitudeGain, float frequencyGain) {
        if (controlledCamera != null) {
            CameraNoise(controlledCamera.cam, amplitudeGain, frequencyGain);
        }

        if (activeFollowCamera != null) {
            CameraNoise(activeFollowCamera, amplitudeGain, frequencyGain);
        }

        if (activePersonalCamera != null) {
            CameraNoise(activePersonalCamera, amplitudeGain, frequencyGain);
        }
    }

    public void CameraNoise(CinemachineVirtualCamera camera, float amplitudeGain, float frequencyGain) {
        if (camera == null) {
            return;
        }

        CinemachineBasicMultiChannelPerlin noise = camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        noise.m_AmplitudeGain = amplitudeGain;
        noise.m_FrequencyGain = frequencyGain;
    }
}