﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TeamRoster : MonoBehaviour {

    public GameObject slotPrefab;

    List<GameObject> mySlots = new List<GameObject>();

    // Use this for initialization
    void Start() {
        for (int i = 0; i < MatchDetails.UnitLimit; i++) {
            GameObject newSlot = Instantiate(slotPrefab, transform);
            mySlots.Add(newSlot);
        }
    }

}
