using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ChildNetworkTransform : NetworkBehaviour {

    [SyncVar]
    public NetworkInstanceId parentNetId;

    bool hasSetParent = false;

    public override void OnStartClient() {
        // When we are spawned on the client,
        SetParent();
    }

    void Update() {
        // TODO this shouldnt need to exist but it seems it cant find the hand on client start, maybe because its disabled?
        if (!hasSetParent) {
            SetParent();
        }

    }

    private void SetParent() {
        // find the parent object using its ID,
        // and set it to be our transform's parent.
        GameObject parentObject = ClientScene.FindLocalObject(parentNetId);

        if (parentObject != null && hasSetParent) {
            transform.SetParent(parentObject.transform);
            hasSetParent = true;
        } else {
            //Debug.Log("Error finding parent with id " + parentNetId);
        }
    }
}
