using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcceleratedMoveTowards : MonoBehaviour {

    public float timeToComplete = 1.0f;
    public AnimationCurve distanceTravelled;

    private Vector3 startingPos;
    private Vector3 endingPos;
    private float elapsedTime = 0.0f;
    private bool destroyOnComplete;

    public void StartMove(Vector3 destination, bool destroyOnComplete = false) {
        startingPos = transform.position;
        endingPos = destination;
        this.destroyOnComplete = destroyOnComplete;
        StartCoroutine(MoveThis());
    }

    IEnumerator MoveThis() {
        while (elapsedTime < timeToComplete) {
            transform.position = Vector3.Lerp(startingPos, endingPos, distanceTravelled.Evaluate(elapsedTime / timeToComplete));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        if (destroyOnComplete) {
            Destroy(gameObject);
        }
    }
}
