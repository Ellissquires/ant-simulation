using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nest : MonoBehaviour {
    [SerializeField]
    Transform antPrefab;

    Ant[] ants;
    int antCount = 10;

    public float wanderStrength = 0.5f;
    public float steerStrength = 2.0f;
    public float maxSpeed = 3.0f;

    public float width = 2.0f;
    public float height = 2.0f;

    public float antViewRadius = 3.0f;

    LayerMask foodLayer;

    private void Awake() {
        foodLayer = LayerMask.GetMask("Food");
        ants = new Ant[antCount];
        for (int i = 0; i < ants.Length; i++) {
            Transform antBody  = Instantiate(antPrefab, Vector3.zero, Quaternion.identity);

            antBody.SetParent(transform, false);
            ants[i] = new Ant(antBody);
        }
    }

    private void Update() {
        for (int i = 0; i < ants.Length; i++) {
            Ant ant = ants[i];
            // Wander aimlessly in random directions
            ant.senseFood(antViewRadius, foodLayer);
            
            Vector2 desiredDirection = ant.targetFood != null ? ant.desiredDirection 
                : (ant.desiredDirection + Random.insideUnitCircle * wanderStrength).normalized;
            moveAnt(ant, desiredDirection);
        }
    }

    private void moveAnt(Ant ant, Vector2 desiredDirection) {
        ant.desiredDirection = desiredDirection;
        Vector2 desiredVelocity = ant.desiredDirection * maxSpeed;
        // Calculate the amount to steer towards the desired direction (acceleration)
        Vector2 desiredSteeringForce = (desiredVelocity - ant.velocity) * steerStrength;
        // Clamp acceleration to set steerStrength
        Vector2 acceleration = Vector2.ClampMagnitude(desiredSteeringForce, steerStrength);
        // vnew = vold + (a * t), clamped to maximum speed
        Vector2 newVelocity = Vector2.ClampMagnitude(ant.velocity + acceleration * Time.deltaTime, maxSpeed);
        // Calculate new position based on velocity and dt
        Vector2 newPosition = ant.position +  newVelocity * Time.deltaTime;

        // Dodgy collision detection - can probably be handled by the engine
        if (newPosition.x >= width || newPosition.x <= -width) newVelocity.x = -newVelocity.x;
        if (newPosition.y >= height || newPosition.y <= -height) newVelocity.y = -newVelocity.y;

        ant.velocity = newVelocity;
        ant.position += newVelocity * Time.deltaTime;
        
        // Calculate angle to rotate sprite towards target
        float angle = Mathf.Atan2(ant.velocity.y, ant.velocity.x) + Mathf.Rad2Deg;
        ant.antBody.SetPositionAndRotation(ant.position, Quaternion.Euler(0, 0, angle));
    }
}
