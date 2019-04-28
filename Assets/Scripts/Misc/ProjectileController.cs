using UnityEngine;

public class ProjectileController : MonoBehaviour {
    public const float HIT_DISTANCE = 50;
    public const float SCALE_SPEED = 0.07f;
    public const float MIN_SCALE_DISTANCE = 0.005f;

    [SerializeField]
    public GameObject onHitEffect;

    private UnitController myCaster;
    private Node myTarget;
    private Vector3 direction;
    private float speed;

    private bool hitTarget = false;

    private void Start() {
    }

    private void Update() {
        if (hitTarget) {
            ScaleDown();
        } else {
            MoveToTarget();
        }
    }

    private void ScaleDown() {
        float distanceToZeroScale = Vector3.Distance(transform.localScale, Vector3.zero);

        if (distanceToZeroScale >= MIN_SCALE_DISTANCE) {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, SCALE_SPEED);
        } else {
            Destroy(gameObject);
        }
    }

    private void MoveToTarget() {
        if (myTarget == null || speed <= 0) {
            return;
        }

        Vector3 targetPosition = myTarget.transform.position;
        targetPosition.y = transform.position.y;

        float distanceToNode = Vector3.Distance(targetPosition, transform.position);

        if (distanceToNode >= speed / 20) {
            transform.position = transform.position + (direction * speed * Time.deltaTime);
        } else {
            transform.position = targetPosition;
            ReachedTarget();
        }
    }

    public void SetTarget(UnitController caster, Node target, float movementSpeed) {
        myCaster = caster;
        myTarget = target;
        speed = movementSpeed;

        Vector3 startPosition = caster.transform.Find("Token").position;
        transform.position = startPosition;

        Vector3 targetPosition = target.myUnit != null ?
            target.myUnit.transform.Find("Token").position :
            target.transform.position;

        direction = targetPosition - transform.position;
        direction.y = 0;
        direction.Normalize();

        //Sprite mySprite = GetComponent<SpriteRenderer>().sprite;
        //halfWidth = mySprite.rect.width / (mySprite.pixelsPerUnit * 2);

        transform.position = startPosition;

        float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
        transform.rotation = rotation;
    }

    public void ReachedTarget() {
        if (onHitEffect != null) {
            EffectOptions options = new EffectOptions(onHitEffect, 0);
            options.location = myTarget;
            myCaster.CreateEffect(options);
        }

        myCaster.ProjectileHit(this);
        hitTarget = true;
    }
}