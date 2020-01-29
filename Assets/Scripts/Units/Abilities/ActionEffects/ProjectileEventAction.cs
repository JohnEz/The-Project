﻿using UnityEngine;

[CreateAssetMenu(fileName = "New Projectile Effect", menuName = "Ability/EventFX/Projectile Effect")]
public class ProjectileEventAction : EventAction {
    public GameObject projectileObject = null;
    public float speed = 1f;
    public float delay = 0;

    public ProjectileEventAction() : base() {
        eventTarget = EventTarget.CASTER;
        action = (UnitController caster, Node targetedTile) => {
            caster.CreateProjectileWithDelay(projectileObject, targetedTile, speed, delay);
        };
    }
}