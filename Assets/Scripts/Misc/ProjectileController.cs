using UnityEngine;

public class ProjectileController : MonoBehaviour {
    public const float MAX_LIFE_SECONDS = 1.5f;
    public const float SCALE_SPEED = 1f;
    public const float MIN_SCALE_DISTANCE = 0.005f;

    [SerializeField]
    public GameObject onHitEffect;

    private UnitController myCaster;
    private Node myTarget;
    private Vector3 direction;
    private float speed;

    private bool hitTarget = false;

    private float timeAlive = 0;
    private float previousDistance = Mathf.Infinity;

    private void Start() {
    }

    private void Update() {
        timeAlive += Time.deltaTime;

        if (hitTarget) {
            ScaleDown();
        } else {
            MoveToTarget();

            if (timeAlive >= MAX_LIFE_SECONDS) {
                ReachedTarget();
            }
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

        if (distanceToNode > speed / 200 && previousDistance > distanceToNode) {
            transform.position = transform.position + (direction * speed * Time.deltaTime);
        } else {
            //transform.position = targetPosition;
            ReachedTarget();
        }

        previousDistance = distanceToNode;
    }

    public void SetTarget(UnitController caster, Node targetedNode, float movementSpeed) {
        myCaster = caster;
        myTarget = targetedNode;
        speed = movementSpeed;

        Vector3 startPosition = caster.transform.Find("Token").position;
        transform.position = startPosition;

        Vector3 targetPosition = targetedNode.MyUnit != null ?
            targetedNode.MyUnit.transform.Find("Token").position :
            targetedNode.transform.position;

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