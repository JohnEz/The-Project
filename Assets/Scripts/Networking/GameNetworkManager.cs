using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class GameNetworkManager : NetworkManager {

    PlayerConnectionObject[] playerSlots = new PlayerConnectionObject[NetworkConstants.MAX_PLAYERS];

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId) {
        // find empty player slot
        for (int slot = 0; slot < NetworkConstants.MAX_PLAYERS; slot++) {
            if (playerSlots[slot] == null) {
                GameObject playerObj = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
                PlayerConnectionObject player = playerObj.GetComponent<PlayerConnectionObject>();

                player.playerId = slot;
                playerSlots[slot] = player;

                Debug.Log("Adding player in slot " + slot);
                NetworkServer.AddPlayerForConnection(conn, playerObj, playerControllerId);
                return;
            }
        }

        //TODO: graceful  disconnect
        conn.Disconnect();
    }

    public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController playerController) {
        // remove players from slots
        PlayerConnectionObject player = playerController.gameObject.GetComponent<PlayerConnectionObject>();
        playerSlots[player.playerId] = null;
        GameManager.singleton.RemovePlayer(player);

        base.OnServerRemovePlayer(conn, playerController);
    }

    public override void OnServerDisconnect(NetworkConnection conn) {
        foreach (var playerController in conn.playerControllers) {
            PlayerConnectionObject player = playerController.gameObject.GetComponent<PlayerConnectionObject>();
            playerSlots[player.playerId] = null;
            GameManager.singleton.RemovePlayer(player);
        }

        base.OnServerDisconnect(conn);
    }

    //MESSAGE LISTENERS
    //////////////////////////////////

    public override void OnStartClient(NetworkClient client) {
        client.RegisterHandler(CardMessage.CardMsgId, OnCardMsg);
    }

    void OnCardMsg(NetworkMessage netMsg) {
        CardMessage msg = netMsg.ReadMessage<CardMessage>();

        GameObject other = ClientScene.FindLocalObject(msg.playerId);
        PlayerConnectionObject player = other.GetComponent<PlayerConnectionObject>();
        player.MsgAddCard(msg.cardId);
    }

}
