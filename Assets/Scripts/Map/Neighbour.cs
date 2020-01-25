using UnityEngine;

public class Neighbour {
    public Tile n1;
    public Tile n2;
    public GameObject myDoor;

    public Neighbour(Tile _n1, Tile _n2) {
        n1 = _n1;
        n2 = _n2;
        myDoor = null;
    }

    public Tile GetOppositeTile(Tile startTile) {
        return startTile.Equals(n2) ? n1 : n2;
    }

    public Vector2 GetDirectionFrom(Tile startTile) {
        Tile endTile = GetOppositeTile(startTile);

        // we have to inverse the y axis as the map renders from the top left rather than bottom left
        return new Vector2(startTile.x - endTile.x, endTile.y - startTile.y);
    }

    public bool HasDoor() {
        return myDoor != null;
    }

    public bool AddDoor(GameObject doorPrefab) {
        if (HasDoor()) {
            Debug.LogError("Tried to add door when it already existed: " + ToString());
            return false;
        }

        Vector3 positionDifference = (n2.transform.position - n1.transform.position) / 2;
        Vector3 position = new Vector3(n1.transform.position.x + positionDifference.x, doorPrefab.transform.position.y, n1.transform.position.z + positionDifference.z);
        Quaternion rotation = Quaternion.Euler(0, 0, 0);

        if (positionDifference.z == 0) {
            rotation = Quaternion.Euler(0, 90, 0);
        }

        myDoor = GameObject.Instantiate(doorPrefab, position, rotation);
        return true;
    }

    public override string ToString() {
        return string.Format("[ n1: {0}, n2: {1}, HasDoor: {2} ]", n1.ToString(), n2.ToString(), HasDoor());
    }
}