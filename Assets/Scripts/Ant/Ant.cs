using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ant {
    public Transform antBody;
    public Vector2 desiredDirection;
    public Vector2 velocity;
    public Vector2 position;

    public Transform targetFood;

    public LineRenderer lineRenderer;
    public float viewRadius;
    public LayerMask foodLayer;

    public Ant(Transform antBody, float viewRadius, LayerMask layerMask) {
        this.antBody = antBody;
        this.viewRadius = viewRadius;
        this.foodLayer = layerMask;
        lineRenderer = antBody.GetComponent<LineRenderer>();
    }

    public void senseFood(float viewRadius, LayerMask foodLayer) {
        if (targetFood == null) {
            Collider2D[] foodInRange = Physics2D.OverlapCircleAll(position, viewRadius, foodLayer);
            if (foodInRange.Length > 0) {
                Transform food = foodInRange[Random.Range(0, foodInRange.Length)].transform;
                Vector2 foodDirection = ((Vector2)food.position - position).normalized;

                food.gameObject.layer = LayerMask.NameToLayer("PickedUpFood");
                targetFood = food; 
            }
        } else {
            desiredDirection = ((Vector2)targetFood.position - position).normalized;
            const float foodPickupRadius = 0.3f;
            if (Vector2.Distance(targetFood.position, position) < foodPickupRadius) {
                targetFood.position = position;
                targetFood.parent = antBody;
                targetFood = null;
            }
        }
    }

    public void drawViewRange(float viewRadius) {
        if (!lineRenderer.enabled) return;
        lineRenderer.widthMultiplier = 0.02f;
 
        Vector3 pos;
        float deltaTheta = (2f * Mathf.PI) / 100;
        float theta = 0f;

        lineRenderer.positionCount = 100;
        for (int i = 0; i < lineRenderer.positionCount; i++) {

            float x = viewRadius * Mathf.Cos(theta);
            float y = viewRadius * Mathf.Sin(theta);          
            x += antBody.position.x;
            y += antBody.position.y;

            pos = new Vector3(x, y, 0);

            lineRenderer.SetPosition(i, pos);
            theta += deltaTheta;
        }
    }
}
