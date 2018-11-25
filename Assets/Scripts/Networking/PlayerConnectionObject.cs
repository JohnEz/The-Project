﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public struct PlayerData {
    public int id;
    public string playerName;
    public bool ai;
    public int faction;

    public UnitController myCharacter;

    public Queue<AbilityCardBase> deck;
    List<AbilityCardBase> hand;
    public Stack<AbilityCardBase> discard;

    public Deck myDeck;
}

public class PlayerConnectionObject : NetworkBehaviour {
    [SyncVar]
    public int playerId;

    // TODO i feel i shouldn't need this but its used for client validation
    public bool isMyTurn = false;

    public string playerName;

    public PlayerSlot playerSlot;

    public List<CardId> handCards = new List<CardId>();

    private void Start() {
        if (!isLocalPlayer) {
            return;
        }

        CmdCreateDeck();
        UnitManager.singleton.CmdSpawnUnit(0, 2, 3);
    }

    public override void OnStartServer() {
        GameManager.singleton.AddPlayer(this);
    }

    public override void OnStartClient() {
        playerSlot = GameManager.singleton.playerSlots[playerId];
        playerSlot.myPlayer = this;
        playerSlot.gameObject.SetActive(true);
    }

    public override void OnNetworkDestroy() {
        if (playerSlot != null) {
            playerSlot.gameObject.SetActive(false);
        }
    }

    public override void OnStartLocalPlayer() {
        GameManager.singleton.localPlayer = this;
    }

    // COMMANDs
    ////////////////////

    [Command]
    public void CmdCreateDeck() {
        // Validation here

        CardManager.singleton.ServerSetupDeck(playerId);
    }

    [Command]
    public void CmdDrawCard() {
        // Validation here
        //Check its my turn, this shouldnt be needed
        if (TurnManager.singleton.currentTurnPlayer != this) {
            Debug.LogError("not your turn");
            return;
        }
        ServerAddCard(CardManager.singleton.GetRandomCard(playerId));
    }

    [Command]
    public void CmdPlayCard(CardId cardId, int cardSlotIndex) {
        // Validation here
        //Check its my turn, this shouldnt be needed
        if (TurnManager.singleton.currentTurnPlayer != this) {
            Debug.LogError("not your turn");
            return;
        }
        if(!handCards.Contains(cardId)) {
            Debug.LogError("You do not have that card in your hand?!");
            return;
        }
        ServerPlayCard(cardId, cardSlotIndex);
    }

    [Command]
    public void CmdEndTurn() {
        if (TurnManager.singleton.currentTurnPlayer != this) {
            Debug.LogError("not your turn");
            return;
        }
        ServerEndTurn();
    }

    // SERVERs
    ////////////////////

    [Server]
    public void ServerAddCard(CardId drawnCard) {
        handCards.Add(drawnCard);
        RpcAddCard(drawnCard);
    }

    [Server]
    public void ServerPlayCard(CardId playedCard, int cardSlotIndex) {
        handCards.Remove(playedCard);
        RpcPlayCard(playedCard, cardSlotIndex);
    }

    [Server]
    public void ServerEndTurn() {
        TurnManager.singleton.ServerEndTurn();
    }

    // CLIENT RPCs
    ////////////////////

    [ClientRpc]
    void RpcAddCard(CardId drawnCard) {
        if (!isServer) {
            // this was already done for host player
            handCards.Add(drawnCard);
        }
        playerSlot.AddCard(drawnCard);
    }

    [ClientRpc]
    void RpcPlayCard(CardId playedCard, int cardSlotIndex) {
        if (!isServer) {
            // this was already done for host player
            handCards.Remove(playedCard);
        }
        Debug.Log("Player " + playerId + " played " + playedCard.name + " from slot " + cardSlotIndex);
        playerSlot.PlayCard(playedCard, cardSlotIndex);
    }

    [ClientRpc]
    public void RpcYourTurn(bool isYourTurn) {
        isMyTurn = isYourTurn;

        if (isYourTurn && isLocalPlayer) {
            GameManager.singleton.EnableClientButtons();

        } else {
            GameManager.singleton.DisableClientButtons();
        }
    }

    // MESSAGES
    ////////////////////

    public void MsgAddCard(CardId cardId) {
        handCards.Add(cardId);
        playerSlot.AddCard(cardId);
    }

}
