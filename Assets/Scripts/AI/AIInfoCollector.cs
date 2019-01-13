using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class HeatMap {
    public UnitController unit;
    public Dictionary<Vector2, int> heatMap;

    public HeatMap(UnitController _unit) {
        unit = _unit;
        heatMap = new Dictionary<Vector2, int>();
    }

    public void Generate() {
        heatMap = new Dictionary<Vector2, int>();

        unit.myStats.Attacks.ForEach(attack => {
            int damagePotential = attack.GetDamageEstimate();
            if (damagePotential > 0) {
                // close attack nodes
                List<Node> closeAttackableTiles = TileMap.instance.pathfinder.FindAttackableTiles(unit.myNode, attack);

                closeAttackableTiles.ForEach(node => {
                    SetHeatForNode(node, damagePotential * 2);
                });

                // Move and attack nodes
                List<Node> moveAttackNodes = TileMap.instance.pathfinder.findReachableTiles(unit.myNode, unit.myStats.Speed, Walkable.Walkable, unit.myPlayer.faction).basic.Keys.ToList();
                moveAttackNodes.ForEach(baseNode => {
                    TileMap.instance.pathfinder.FindAttackableTiles(baseNode, attack).ForEach(attackNode => {
                        SetHeatForNode(attackNode, damagePotential);
                    });
                });
            }
        });
        
    }

    private void SetHeatForNode(Node n, int heat) {
        Vector2 nodeVector = new Vector2(n.x, n.y);
        if (!heatMap.ContainsKey(nodeVector) || heatMap[nodeVector] < heat) {
            heatMap[nodeVector] = heat;
        }
    }

    public int GetHeat(int x, int y) {
        Vector2 location = new Vector2(x, y);
        if (!heatMap.ContainsKey(location)) {
            return 0;
        }

        return heatMap[location];
    }
}

public class TileHostility {
    public int heat = 0;
    public int numberOfAttacks = 0;
}

public class AIInfoCollector {

    private static AIInfoCollector instance = null;

    public Dictionary<int, List<HeatMap>> factionHeatMaps;

    private AIInfoCollector() {
        factionHeatMaps = new Dictionary<int, List<HeatMap>>();
    }

    public static AIInfoCollector Instance {
        get {
            if (instance == null) {
                instance = new AIInfoCollector();
            }
            return instance;
        }
    }


    public void GenerateHostilityMap(int faction) {
        if (factionHeatMaps.ContainsKey(faction)) {
            factionHeatMaps.Remove(faction);
        }

        List<HeatMap> heatmaps = new List<HeatMap>();
        List<UnitController> units = UnitManager.singleton.Units.FindAll(unit => unit.myPlayer.faction != faction && !unit.IsStealthed());

        units.ForEach(unit => {
            HeatMap heatmap = new HeatMap(unit);
            heatmap.Generate();
            heatmaps.Add(heatmap);
        });

        factionHeatMaps.Add(faction, heatmaps);
    }

    public void UnitDied() {

    }

    public void UnitSpawned() {

    }

    public TileHostility GetHostilityOfTile(int faction, Node node) {
        return GetHostilityOfTile(faction, new Vector2(node.x, node.y));
    }

    public TileHostility GetHostilityOfTile(int faction, Vector2 nodePos) {
        TileHostility hostility = new TileHostility();
        if (!factionHeatMaps.ContainsKey(faction)) {
            Debug.Log("No heat for faction: " + faction);
            return hostility;
        }

        //for each unit
        List<HeatMap> heatMaps = factionHeatMaps[faction];

        heatMaps.ForEach(heatmap => {
            hostility.heat += heatmap.GetHeat((int)nodePos.x, (int)nodePos.y);
            hostility.numberOfAttacks++;
        });

        return hostility;
    }

    public List<Vector2> GetHotNodes(int faction) {
        List<Vector2> hotNodes = new List<Vector2>();
        if (!factionHeatMaps.ContainsKey(faction)) {
            Debug.Log("Couldnt find heatmap for faction: " + faction);
            return hotNodes;
        }

        List<HeatMap> heatMaps = factionHeatMaps[faction];

        heatMaps.ForEach(heatmap => {
            hotNodes = hotNodes.Union(heatmap.heatMap.Keys.ToList()).ToList();
        });

        return hotNodes;
    }

}
