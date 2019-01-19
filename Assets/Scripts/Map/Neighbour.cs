using UnityEngine;

public class Neighbour {

    //public Vector2 direction;
    public Node n1;

    public Node n2;
    public GameObject myDoor;

    public Neighbour(Node _n1, Node _n2) {
        n1 = _n1;
        n2 = _n2;
        myDoor = null;
    }

    public Node GetOppositeNode(Node startNode) {
        // TODO this isnt safe, what if its neither of the nodes?
        return startNode == n2 ? n1 : n2;
    }

    public Vector2 GetDirectionFrom(Node startNode) {
        Node endNode = GetOppositeNode(startNode);

        // we have to inverse the y axis as the map renders from the top left rather than bottom left
        return new Vector2(startNode.x - endNode.x, endNode.y - startNode.y);
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

    public bool OpenDoor() {
        if (!HasDoor()) {
            Debug.LogError("Tried to open a door that didnt exist: " + ToString());
            return false;
        }

        if (!TileMap.instance.IsRoomActive(n1.room)) {
            TileMap.instance.ActivateRoom(n1.room);
        }

        if (!TileMap.instance.IsRoomActive(n2.room)) {
            TileMap.instance.ActivateRoom(n2.room);
        }

        GameObject.Destroy(myDoor);
        myDoor = null;
        return true;
    }

    public override string ToString() {
        return string.Format("[ n1: {0}, n2: {1}, HasDoor: {2} ]", n1.ToString(), n2.ToString(), HasDoor());
    }
}