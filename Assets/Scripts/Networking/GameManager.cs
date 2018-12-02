using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour {

    public static GameManager singleton;

    //ALL this is state on the server, not neccasarily up to date on clients!

    public PlayerConnectionObject localPlayer;
    public List<PlayerConnectionObject> players = new List<PlayerConnectionObject>();

    public PlayerSlot[] playerSlots = new PlayerSlot[2];

    //TEMP
    public Vector2[] spawnLocations = new Vector2[2];

    TileMap map;
    CameraManager cameraManager;

    public Button btnEndTurn;

    private void Awake() {
        singleton = this;
    }

    void Start() {
        map = GetComponentInChildren<TileMap>();
        map.Initialise();
        cameraManager = GetComponent<CameraManager>();
        cameraManager.Initialise();
        UnitManager.singleton.Initialise(map);
    }

    public void AddPlayer(PlayerConnectionObject player) {
        players.Add(player);

        if (!TurnManager.singleton.gameHasStarted) {
            if (players.Count == NetworkConstants.MAX_PLAYERS) {
                TurnManager.singleton.ServerStartGame();
            }
        } else {

            //Catch the player up on things that might have been set in the game via RPC
            players.ForEach(existingPlayer => {

                // dont want to do this for the new player
                // Or maybe add reconnect feature here
                if (player == existingPlayer) {
                    return;
                }

                existingPlayer.handCards.ForEach(card => {
                    //Send message about card
                    CardMessage msg = new CardMessage();
                    msg.playerId = existingPlayer.netId;
                    msg.cardId = card;
                    player.connectionToClient.Send(CardMessage.CardMsgId, msg);
                });

            });

        }

    }

    public void RemovePlayer(PlayerConnectionObject player) {
        players.Remove(player);
    }

    // UI METHODS - TODO maybe move to their own class
    //////////////////////////////////

    [Client]
    public void DisableClientButtons() {
        btnEndTurn.interactable = false;
    }

    [Client]
    public void EnableClientButtons() {
        btnEndTurn.interactable = true;
    }

    public void UiEndTurn() {
        localPlayer.CmdEndTurn();
    }

}
