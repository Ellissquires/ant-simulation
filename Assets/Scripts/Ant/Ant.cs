using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ant {
    public string uid;

    private Transform antBody;
    private Transform targetFood;
    private LayerMask foodLayer;

    private LineRenderer lineRenderer;
    private float viewRadius;

    private Vector2 desiredDirection;
    private Vector2 velocity;
    
    private Vector2 position;
    public Vector2 previousPosition;

    public bool searchingForFood = true;
    public AntConfiguration configuration { get; set; }

    public Ant(Transform antBody, LayerMask foodLayer) {
        this.antBody = antBody;
        this.foodLayer = foodLayer;
        this.uid = System.Guid.NewGuid().ToString();
        lineRenderer = antBody.GetComponent<LineRenderer>();
    }

    public void senseFood() {
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

    public void move(float dt) {
        desiredDirection = calculateDesiredDirection();
        Vector2 desiredVelocity = desiredDirection * configuration.maxSpeed;
        // Calculate the amount to steer towards the desired direction (acceleration)
        Vector2 desiredSteeringForce = (desiredVelocity - velocity) * configuration.steerStrength;
        // Clamp acceleration to set steerStrength
        Vector2 acceleration = Vector2.ClampMagnitude(desiredSteeringForce, configuration.steerStrength);
        // vnew = vold + (a * t), clamped to maximum speed
        Vector2 newVelocity = Vector2.ClampMagnitude(velocity + acceleration * dt, configuration.maxSpeed);
        // Calculate new position based on velocity and dt
        Vector2 newPosition = position + newVelocity * dt;

        // Dodgy collision detection - can probably be handled by the engine
        if (newPosition.x >= configuration.maxX || newPosition.x <= -configuration.maxX) newVelocity.x = -newVelocity.x;
        if (newPosition.y >= configuration.maxY || newPosition.y <= -configuration.maxY) newVelocity.y = -newVelocity.y;

        velocity = newVelocity;
        previousPosition = position;
        position += newVelocity * Time.deltaTime;
        
        // Calculate angle to rotate sprite towards target
        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        antBody.SetPositionAndRotation(position, Quaternion.AngleAxis(angle - 90, Vector3.forward));
    }

    private Vector2 calculateDesiredDirection() {
        Vector2 randomDirection = Random.insideUnitCircle * configuration.wanderStrength;
        return targetFood != null ? desiredDirection
            : (desiredDirection + randomDirection).normalized;
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
