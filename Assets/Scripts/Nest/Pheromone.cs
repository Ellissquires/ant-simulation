using UnityEngine;

public class Pheromone {
    public float creationTime;
    public PheromoneType type;
    public Transform marker;

    public Pheromone (PheromoneType type, Transform marker) {
        this.type = type;
        this.marker = marker;
        this.creationTime = Time.time;
    }
}