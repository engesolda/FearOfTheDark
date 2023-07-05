using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowPathFinder : MonoBehaviour
{
    private int numberGoodPaths = 5;
    private float teste = 200;

    struct Dimensions
    {
        public float widith;
        public float height;

        public Dimensions(float w, float h) { widith = w; height = h; }
    }

    public class Node
    {
        public float x;
        public float y;
        public bool visited;
        public int cost;
        public Node before;
        public float DistanceToGoal;

        public Node(float _x, float _y, bool v, int c, Node b, float d) { x = _x; y = _y; visited = false; cost = c; before = b; DistanceToGoal = d; }
    }

    public class Rays
    {
        public Vector2 r1Ini;
        public Vector2 r1End;
        public Vector2 r2Ini;
        public Vector2 r2End;

        public Rays(Vector2 a, Vector2 b, Vector2 c, Vector2 d) { r1Ini = a; r1End = b; r2Ini = c; r2End = d; }
    }

    public GameObject start;
    public GameObject end;
    public SpriteRenderer white;
    public SpriteRenderer green;
    public bool prioritizeDistance;

    private List<Node> searchPath;
    private List<Vector2> choosenPath;
    private GameObject g2;
    private List<Rays> raysList;
    private bool drawPath = false;
    private bool drawSearch = false;

    private void Start()
    {
        searchPath = new List<Node>();
        raysList = new List<Rays>();
        choosenPath = new List<Vector2>();

        choosenPath = FindBestPathToDarkness(start.transform.position, end.GetComponent<BoxCollider2D>().size.x, end.GetComponent<BoxCollider2D>().size.y, 1.5f);
        drawSearch = true;
    }

    private void Update()
    {
        if (drawSearch)
        {
            //PaintSearch();
            drawSearch = false;
            drawPath = true;
        }

        if (drawPath)
        {
            DrawPath();
        }
    }

    void DrawPath()
    {
        foreach (Vector2 current in choosenPath)
        {
            GameObject g1 = new GameObject();
            g1.AddComponent<SpriteRenderer>();
            g1 = Instantiate(g1, transform);
            g1.transform.position = new Vector2(current.x, current.y);
            g1.GetComponent<SpriteRenderer>().sprite = green.sprite;
            g1.GetComponent<SpriteRenderer>().sortingLayerName = "Units";
            g1.transform.localScale = new Vector2(0.5f, 0.5f);
        }

        drawPath = false;
    }

    

    void PaintRays()
    {
        if (raysList.Count > 0)
        {
            Rays current = raysList[0];
            raysList.RemoveAt(0);

            Debug.DrawLine(current.r1Ini, current.r1End, Color.green, 10);
            Debug.DrawLine(current.r2Ini, current.r2End, Color.blue, 10);
        }
    }

    void PaintSearch()
    {
        if (g2 != null)
        {
            g2.GetComponent<SpriteRenderer>().sprite = white.sprite;
        }

        if (searchPath.Count > 0)
        {
            Node current = searchPath[0];
            searchPath.RemoveAt(0);

            GameObject g1 = new GameObject();
            g1.AddComponent<SpriteRenderer>();
            g1 = Instantiate(g1, transform);
            g1.transform.position = new Vector2(current.x, current.y);
            g1.GetComponent<SpriteRenderer>().sprite = green.sprite;
            g1.GetComponent<SpriteRenderer>().sortingLayerName = "Units";
            g1.transform.localScale = new Vector2(0.5f, 0.5f);
            g2 = g1;
        }
        else
        {
            drawSearch = false;
            drawPath = true;
        }
    }

    //FInd the N best paths from _iniPos to _endPos within the provided range.
    public List<Vector2> FindBestPath(Vector2 _iniPos, Vector2 _endPos, float _myWidith, float _myHeight, float _rangePursuit)
    {
        float maxDistanceSearch = _rangePursuit * 5;
        Dimensions mySize = new Dimensions(_myWidith * 1.1f, _myHeight * 1.1f);
        float distanceToTarget = Vector2.Distance(_iniPos, _endPos);

        List<Node> path = new List<Node>();
        List<Node> cost_so_far = new List<Node>();
        List<Node> finalPaths = new List<Node>();
        List<Vector2> positions = new List<Vector2>();
        Node nodeStart = new Node(_iniPos.x, _iniPos.y, true, 0, null, distanceToTarget);

        path.Add(nodeStart);

        while (path.Count > 0)
        {
            Node current = path[0];

            if (Vector2.Distance(new Vector2(current.x, current.y), _endPos) <= 1)
            {// We reached the player or the limit of paths needed
                finalPaths.Add(current);

                if (finalPaths.Count == numberGoodPaths)
                {
                    break;
                }
            }

            path.RemoveAt(0);

            List<Node> neighborsList = new List<Node>();

            // Get the neighbors in the following order: Right / Left / Up / Down
            neighborsList = GetNeighbors(current, _endPos, mySize, false);

            foreach (Node neighbor in neighborsList)
            {
                Vector2 currentPos = new Vector2(current.x, current.y);
                Vector2 neighborPos = new Vector2(neighbor.x, neighbor.y);

                if (!HasCollision(currentPos, neighborPos, mySize, false) && Vector2.Distance(_iniPos, neighborPos) <= maxDistanceSearch)
                {// we only work with this neighbor if there is no collision and it is within our limits
                    int pathCost = current.cost + 1;

                    Node costNode = cost_so_far.Find(it => it.x == neighbor.x && it.y == neighbor.y);
                    if (costNode == null || pathCost < costNode.cost)
                    {//if we dont know the cost of this node or the cost to it is lower than what we had 
                        neighbor.cost = pathCost;

                        if (costNode == null)
                        {
                            cost_so_far.Add(neighbor);
                        }
                        else
                        {
                            cost_so_far.Find(it => it.x == costNode.x && it.y == costNode.y).cost = pathCost;
                        }

                        if (path.Find(it => it.x == neighbor.x && it.y == neighbor.y) == null)
                        {// If the node was already visited we dont need to visit it again
                            path.Add(neighbor);

                            path.Sort((n1, n2) => (n1.DistanceToGoal).CompareTo(n2.DistanceToGoal));
                        }
                    }
                }
            }
        }

        //Lets build the paths based on the nodes that reached the target
        if (finalPaths.Count > 0)
        {
            Node currentNode = finalPaths[Random.Range(0, finalPaths.Count - 1)];

            while (currentNode != nodeStart)
            {
                positions.Add(new Vector2(currentNode.x, currentNode.y));
                currentNode = currentNode.before;
            }
        }
        else
        {
            Debug.Log("No Path Found from (" + _iniPos.x + "/" + _iniPos.y + ") to (" + _endPos.x + "/" + _endPos.y + ").");
        }

        return positions;
    }

    //Find all best paths back to darkness, get only the paths within 10% of the best cost path and them select on randomly to return
    public List<Vector2> FindBestPathToDarkness(Vector2 _iniPos, float _myWidith, float _myHeight, float _rangePursuit)
    {
        float maxDistanceSearch = _rangePursuit * 5;
        Dimensions mySize = new Dimensions(_myWidith * 1.1f, _myHeight * 1.1f);

        List<Node> path = new List<Node>();
        List<Node> cost_so_far = new List<Node>();
        List<Node> finalPaths = new List<Node>();
        List<Vector2> positions = new List<Vector2>();
        Node nodeStart = new Node(_iniPos.x, _iniPos.y, true, 0, null, 0);

        path.Add(nodeStart);

        while (path.Count > 0)
        {
            teste -= Time.deltaTime;
            if (teste <= 0)
            {
                Debug.Log("loop infinito");
                return null;
            }

            Node current = path[0];

            if (FoundDarkness(_iniPos, new Vector2(current.x, current.y)))
            {// We reached the player or the limit of paths needed
                finalPaths.Add(current);
            }

            path.RemoveAt(0);

            List<Node> neighborsList = new List<Node>();

            // Get the neighbors in the following order: Right / Left / Up / Down
            neighborsList = GetNeighbors(current, Vector2.zero, mySize, true);

            foreach (Node neighbor in neighborsList)
            {
                Vector2 currentPos = new Vector2(current.x, current.y);
                Vector2 neighborPos = new Vector2(neighbor.x, neighbor.y);

                if (!HasCollision(currentPos, neighborPos, mySize, true) && Vector2.Distance(_iniPos, neighborPos) <= maxDistanceSearch)
                {// we only work with this neighbor if there is no collision and it is within our limits
                    int pathCost = current.cost + 1;

                    Node costNode = cost_so_far.Find(it => it.x == neighbor.x && it.y == neighbor.y);
                    if (costNode == null || pathCost < costNode.cost)
                    {//if we dont know the cost of this node or the cost to it is lower than what we had 
                        neighbor.cost = pathCost;

                        if (costNode == null)
                        {
                            cost_so_far.Add(neighbor);
                        }
                        else
                        {
                            cost_so_far.Find(it => it.x == costNode.x && it.y == costNode.y).cost = pathCost;
                        }

                        if (path.Find(it => it.x == neighbor.x && it.y == neighbor.y) == null)
                        {// If the node was already visited we dont need to visit it again
                            path.Add(neighbor);
                            searchPath.Add(neighbor);

                            path.Sort((n1, n2) => (n1.cost).CompareTo(n2.cost));
                        }
                    }
                }
            }
        }

        //Lets build the paths based on the nodes that reached the target
        if (finalPaths.Count > 0)
        {
            //This is because i want only the best paths within 10% of the best possible path and i want them random.
            finalPaths.Sort((n1, n2) => (n1.cost).CompareTo(n2.cost));
            float range = finalPaths[0].cost * 1.1f;
            int indexMax = finalPaths.FindIndex(it => it.cost >= range);

            Node currentNode = finalPaths[Random.Range(0, indexMax)];

            while (currentNode != nodeStart)
            {
                positions.Add(new Vector2(currentNode.x, currentNode.y));
                choosenPath.Add(new Vector2(currentNode.x, currentNode.y));
                currentNode = currentNode.before;
            }
        }
        else
        {
            Debug.Log("No Path Found from (" + _iniPos.x + "/" + _iniPos.y + ") to darkness.");
        }

        return positions;
    }

    bool FoundDarkness(Vector2 _iniPos, Vector2 _currentPos)
    {
        Vector2 direction = GetDirection(_currentPos, _iniPos);

        RaycastHit2D ray = Physics2D.Raycast(_currentPos, direction, Vector2.Distance(_iniPos, _currentPos));

        if (ray.collider && ray.collider.CompareTag("Light"))
        {
            return true;
        }

        return false;
    }

    //Cast 2 rays from the bottom and top of the object to the next position and check if these rays collided with something
    bool HasCollision(Vector2 _iniPos, Vector2 _endPos, Dimensions mySize, bool _searchingDarkness)
    {
        Vector2 direction = GetDirection(_iniPos, _endPos);

        Vector2 ray1Ini = new Vector2();
        Vector2 ray2Ini = new Vector2();
        Vector2 ray1End = new Vector2();
        Vector2 ray2End = new Vector2();

        if (direction == Vector2.up)
        {
            ray1Ini = new Vector2(_iniPos.x - mySize.widith / 2, _iniPos.y);
            ray2Ini = new Vector2(_iniPos.x + mySize.widith / 2, _iniPos.y);

            ray1End = new Vector2(_endPos.x - mySize.widith / 2, _endPos.y + mySize.height / 2);
            ray2End = new Vector2(_endPos.x + mySize.widith / 2, _endPos.y + mySize.height / 2);
        }
        if (direction == Vector2.down)
        {
            ray1Ini = new Vector2(_iniPos.x - mySize.widith / 2, _iniPos.y);
            ray2Ini = new Vector2(_iniPos.x + mySize.widith / 2, _iniPos.y);

            ray1End = new Vector2(_endPos.x - mySize.widith / 2, _endPos.y - mySize.height / 2);
            ray2End = new Vector2(_endPos.x + mySize.widith / 2, _endPos.y - mySize.height / 2);
        }
        if (direction == Vector2.left)
        {
            ray1Ini = new Vector2(_iniPos.x, _iniPos.y + mySize.height / 2);
            ray2Ini = new Vector2(_iniPos.x, _iniPos.y - mySize.height / 2);

            ray1End = new Vector2(_endPos.x - mySize.widith / 2, _endPos.y + mySize.height / 2);
            ray2End = new Vector2(_endPos.x - mySize.widith / 2, _endPos.y - mySize.height / 2);
        }
        if (direction == Vector2.right)
        {
            ray1Ini = new Vector2(_iniPos.x, _iniPos.y + mySize.height / 2);
            ray2Ini = new Vector2(_iniPos.x, _iniPos.y - mySize.height / 2);

            ray1End = new Vector2(_endPos.x + mySize.widith / 2, _endPos.y + mySize.height / 2);
            ray2End = new Vector2(_endPos.x + mySize.widith / 2, _endPos.y - mySize.height / 2);
        }

        RaycastHit2D ray1 = Physics2D.Raycast(ray1Ini, direction, Vector2.Distance(ray1Ini, ray1End));
        RaycastHit2D ray2 = Physics2D.Raycast(ray2Ini, direction, Vector2.Distance(ray2Ini, ray2End));

        if (ray1.collider != null)
        {
            if (_searchingDarkness && ray1.collider.CompareTag("Light"))
            {
                return false;
            }

            if (!ray1.collider.CompareTag("Player") && !ray1.collider.CompareTag("Enemy") && !ray1.collider.CompareTag("Light"))
            {
                return true;
            }
        }

        if (ray2.collider != null)
        {
            if (_searchingDarkness && ray2.collider.CompareTag("Light"))
            {
                return false;
            }

            if (!ray2.collider.CompareTag("Player") && !ray2.collider.CompareTag("Enemy") && !ray2.collider.CompareTag("Light"))
            {
                return true;
            }
        }

        return false;
    }

    //Get the naighbors of the current position we are looking
    List<Node> GetNeighbors(Node _current, Vector2 _endPos, Dimensions mySize, bool _searchingForDark)
    {
        List<Node> neighbors = new List<Node>();

        Vector2 neighborPos;
        float distToGoal;

        neighborPos = new Vector2(_current.x + mySize.widith, _current.y);
        distToGoal = _searchingForDark ? 0 : Vector2.Distance(neighborPos, _endPos);
        neighbors.Add(new Node(neighborPos.x, neighborPos.y, false, 1, _current, distToGoal + _current.cost)); //Right


        neighborPos = new Vector2(_current.x - mySize.widith, _current.y);
        distToGoal = _searchingForDark ? 0 : Vector2.Distance(neighborPos, _endPos);
        neighbors.Add(new Node(neighborPos.x, neighborPos.y, false, 1, _current, distToGoal + _current.cost)); //Left            

        neighborPos = new Vector2(_current.x, _current.y + mySize.height);
        distToGoal = _searchingForDark ? 0 : Vector2.Distance(neighborPos, _endPos);
        neighbors.Add(new Node(neighborPos.x, neighborPos.y, false, 1, _current, distToGoal + _current.cost)); //Up

        neighborPos = new Vector2(_current.x, _current.y - mySize.height);
        distToGoal = _searchingForDark ? 0 : Vector2.Distance(neighborPos, _endPos);
        neighbors.Add(new Node(neighborPos.x, neighborPos.y, false, 1, _current, distToGoal + _current.cost)); //Down

        return neighbors;
    }

    Vector2 GetDirection(Vector2 _iniPos, Vector2 _endPos)
    {
        Vector2 direction = (_endPos - _iniPos).normalized;

        if (direction == (Vector2)Vector3.up)
        {
            return Vector2.up;
        }
        if (direction == -(Vector2)Vector3.up)
        {
            return Vector2.down;
        }
        if (direction == (Vector2)Vector3.right)
        {
            return Vector2.right;
        }
        if (direction == -(Vector2)Vector3.right)
        {
            return Vector2.left;
        }

        return Vector2.right;
    }
}
