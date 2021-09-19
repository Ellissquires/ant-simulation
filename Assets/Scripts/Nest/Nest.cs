using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nest : MonoBehaviour {
    [SerializeField]
    Transform antPrefab;
    [SerializeField]
    Transform homeMarkerPrefab;

    private Ant[] ants;
    private int antCount = 100;
    private LayerMask foodLayer;

    public float wanderStrength = 0.5f;
    public float steerStrength = 1.0f;
    public float maxSpeed = 3.0f;

    public float width = 2.0f;
    public float height = 2.0f;

    public float antViewRadius = 0.5f;
    public bool enableViewRadius = true;

    public float markerLifespan = 10.0f;

    private PheromoneMap pheromoneMap = new PheromoneMap();

    private void Awake() {
        foodLayer = LayerMask.GetMask("Food");
        ants = new Ant[antCount];
        for (int i = 0; i < ants.Length; i++) {
            Transform antBody  = Instantiate(antPrefab, Vector3.zero, Quaternion.identity);

            antBody.SetParent(transform, false);
            Ant newAnt = new Ant(antBody, foodLayer);
            ants[i] = newAnt;

            StartCoroutine(spawnMarker(newAnt));
        }

        StartCoroutine(updateTrailVisibility());
    }

    private void Update() {
        for (int i = 0; i < ants.Length; i++) {
            Ant ant = ants[i];
            ant.configuration = antConfiguration();
            ant.senseFood();
            ant.move(Time.deltaTime);
        }
    }

    private AntConfiguration antConfiguration() {
        return new AntConfiguration {
            viewRadius = antViewRadius,
            wanderStrength = wanderStrength,
            steerStrength = steerStrength,
            maxSpeed = maxSpeed,
            maxX = width,
            maxY = height,
            enableViewRadius = enableViewRadius
        };
    }

    private IEnumerator spawnMarker(Ant ant) {
        for(;;) {
            if (ant.searchingForFood) {
                Vector3 markerPosition = new Vector3(ant.previousPosition.x, ant.previousPosition.y);
                Transform homeMarker = Instantiate(homeMarkerPrefab, markerPosition, Quaternion.identity);
                homeMarker.gameObject.hideFlags = HideFlags.HideInHierarchy;

                pheromoneMap.add(ant.uid, new Pheromone(PheromoneType.Home, homeMarker));
            }

            yield return new WaitForSeconds(0.35f);
        }
    }

    private IEnumerator updateTrailVisibility() {
        Color tempColor;
        for(;;) {
            foreach(KeyValuePair<string, List<Pheromone>> entry in pheromoneMap.get()){
                List<Pheromone> pheromones = entry.Value;
                List<Pheromone> pheromoneCopies = new List<Pheromone>(pheromones);

                pheromoneCopies.ForEach(delegate(Pheromone pheromone) {
                    float markerAge = Time.time - pheromone.creationTime;
                    if (markerAge >= markerLifespan) {
                        pheromones.Remove(pheromone);
                        Destroy(pheromone.marker.gameObject);
                    } else {
                        tempColor = pheromone.marker.gameObject.GetComponent<Renderer>().material.color;
                        tempColor.a = 1.0f - markerAge / markerLifespan;
                        pheromone.marker.gameObject.GetComponent<Renderer>().material.color = tempColor;
                    }
                });
            }
            yield return new WaitForSeconds(1.0f);
        }
    }
}
