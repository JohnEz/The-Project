using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteShadow : MonoBehaviour {
    public UnityEngine.Rendering.ShadowCastingMode castShadows;
    public bool receiveShadows;

    // Start is called before the first frame update
    private void Start() {
        Renderer renderer = GetComponent<Renderer>();
        renderer.shadowCastingMode = castShadows;
        renderer.receiveShadows = receiveShadows;
    }

    // Update is called once per frame
    private void Update() {
    }
}