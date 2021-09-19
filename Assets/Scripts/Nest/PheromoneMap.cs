using System.Collections.Generic;

public class PheromoneMap {
    private Dictionary<string, List<Pheromone>> pheromoneMap = new Dictionary<string, List<Pheromone>>();

    public void add(string antId, Pheromone pheromone) {
        if (pheromoneMap.TryGetValue(antId, out var list))
            list.Add(pheromone);
        else
            pheromoneMap.Add(antId, new List<Pheromone>() {pheromone});
    }

    public Dictionary<string, List<Pheromone>> get() {
        return pheromoneMap;
    }   
}