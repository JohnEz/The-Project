using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour {

	[SerializeField]
	public GameObject onHitEffect;

	UnitController myCaster;
	Node myTarget;
	Vector3 direction;
	float speed;
	float halfWidth;

	void Start () {
		
	}

	void Update () {
		MoveToTarget ();
	}

	void MoveToTarget() {
		if (myTarget == null || speed <= 0) {
			return;
		}

		float distanceToNode = Vector3.Distance (myTarget.transform.position, transform.position);

		if (distanceToNode - (speed * Time.deltaTime) > halfWidth) {
			transform.position = transform.position + (direction * speed * Time.deltaTime);
		} else {
			ReachedTarget ();
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

		Sprite mySprite = GetComponent<SpriteRenderer> ().sprite;
		halfWidth = mySprite.rect.width / (mySprite.pixelsPerUnit * 2);

		transform.position = startPosition + (direction * halfWidth);

		//TODO FLIP THE PROJECTILE AND REVERSE / REDUCE ROTATION
		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		transform.rotation = rotation;
	}

	public void ReachedTarget() {
		if (onHitEffect != null) {
			myTarget.myUnit.CreateEffectWithDelay (onHitEffect, 0);
		}

		myCaster.ProjectileHit (this);
	}
}
