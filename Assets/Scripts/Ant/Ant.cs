using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ant {
    public Transform antBody;
    public Vector2 desiredDirection;
    public Vector2 velocity;
    public Vector2 position;

    public Transform targetFood;

    public Ant(Transform antBody) {
        this.antBody = antBody;
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
            const float foodPickupRadius = 0.05f;
            if (Vector2.Distance(targetFood.position, position) < foodPickupRadius) {
                targetFood.position = position;
                targetFood.parent = antBody;
                targetFood = null;
            }
        }
    }
}
