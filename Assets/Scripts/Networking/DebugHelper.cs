using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Linq;

public class DebugHelper : MonoBehaviour {
    void Update() {
        string debug = "List of Ids:\n";
        int counter = 0;

        NetworkIdentity[] foundNetworkIds = FindObjectsOfType<NetworkIdentity>();
        string list = foundNetworkIds.Select(uv => uv.name).Aggregate((current, next) => current + ", " + next);
        Debug.Log("The list returned by FindObjectsOfType<NetworkIdentity>() is:\n" + list);

        foreach (NetworkIdentity uv in FindObjectsOfType<NetworkIdentity>()) {
            // if we had a [ConflictComponent] attribute that would be better than this check.
            // also there is no context about which scene this is in.
            counter++;
            Debug.Log(uv.gameObject.name + " netId (" + uv.netId + ")");

            //if (uv.GetComponent<NetworkManager>() != null) {
            //    Debug.LogError("NetworkManager has a NetworkIdentity component. This will cause the NetworkManager object to be disabled, so it is not recommended.");
            //}
            //if (uv.isClient || uv.isServer)
                //continue;

            //Original UNET code
            /*uv.gameObject.SetActive(false);
            uv.ForceSceneId(nextSceneId++);*/

            //debug += uv.gameObject.name + "\n";
        }

        Debug.Log(debug);
    }
}
