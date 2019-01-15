﻿using UnityEngine;

public class ProjectileController : MonoBehaviour {

    [SerializeField]
    public GameObject onHitEffect;

    private UnitController myCaster;
    private Node myTarget;
    private Vector3 direction;
    private float speed;
    private float halfWidth;

    private void Start() {
    }

    private void Update() {
        MoveToTarget();
    }

    private void MoveToTarget() {
        if (myTarget == null || speed <= 0) {
            return;
        }

        float distanceToNode = Vector3.Distance(myTarget.transform.position, transform.position);

        if (distanceToNode - (speed * Time.deltaTime) > halfWidth) {
            transform.position = transform.position + (direction * speed * Time.deltaTime);
        } else {
            ReachedTarget();
        }
    }

    public void SetTarget(UnitController caster, Node target, float movementSpeed) {
        myCaster = caster;
        myTarget = target;
        speed = movementSpeed;

        Vector3 startPosition = caster.transform.position;
        transform.position = startPosition;

        direction = target.transform.position - transform.position;
        direction.Normalize();

        Sprite mySprite = GetComponent<SpriteRenderer>().sprite;
        halfWidth = mySprite.rect.width / (mySprite.pixelsPerUnit * 2);

        transform.position = startPosition + (direction * halfWidth);

        //TODO FLIP THE PROJECTILE AND REVERSE / REDUCE ROTATION
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = rotation;
    }

    public void ReachedTarget() {
        if (onHitEffect != null) {
            myCaster.CreateEffectWithDelay(onHitEffect, 0, myTarget);
        }

        myCaster.ProjectileHit(this);
    }
}