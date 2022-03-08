using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pikachu_coreGame : MonoBehaviour
{
    private List<GameObject> ListPikachuSelected = new List<GameObject>();
    private List<GameObject> ListWait = new List<GameObject>();
    private List<Node> ListNodeSelected = new List<Node>();

    private LineRenderer lineR;
    private int x, y;

    public Node[,] grid;
    public int width = 14;
    public int height = 8;
    public Text text;

    public Sprite[] PikachuImage;
    public Sprite[] PikachuBackground;
    public GameObject PikachuPrefab;
    public GameObject Board;

    void Start()
    {
        lineR = GetComponent<LineRenderer>();
        InitializeBoard();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            GetHint();
            Vector3 screenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

            int x = (int)Mathf.Round(worldPos.x);
            int y = (int)Mathf.Round(worldPos.y);
            text.text = " Pos: " + x + " " + y;
            //Debug.Log(x+ " "+ y);
            //Debug.Log(" grid: " + grid[x, y].x + " " + grid[x, y].y + " " + grid[x, y].value);
            if (x > 0 && x <= width && y > 0 && y <= height)
            {
                GameObject select = GameObject.Find("Node " + x + " " + y);

                if (select != null) { NodeSelected(select); }
            }
        }
        

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchPos = new Vector3();
            if (touch.phase == TouchPhase.Began)
            {
                touchPos = Camera.main.ScreenToWorldPoint(touch.position);
                x = (int)Mathf.Round(touchPos.x);
                y = (int)Mathf.Round(touchPos.y);
                text.text = " Pos: " + x + " " + y;
                if (x > 0 && x <= width && y > 0 && y <= height)
                {
                    GameObject select = GameObject.Find("Node " + x + " " + y);

                    if (select != null) { NodeSelected(select); }
                }
            }
        }
    }
    void InitializeBoard()
    {
        grid = new Node[width + 2, height + 2];
        List<Node> closeNode = new List<Node>();
        for (int y = 1; y < height + 1; y++)
        {
            for (int x = 1; x < width + 1; x++)
            {
                closeNode.Add(new Node(x, y, 0));

            }
        }

        Vector2 a = new Vector2(closeNode[1].x, closeNode[1].y);
        for (int i = 0; i < ((width * height) / 2); i++)
        {
            int val = Random.Range(1, PikachuImage.Length - 1);

            //instance pikachu 1
            int index1 = Random.Range(0, closeNode.Count - 1);
            closeNode[index1].value = val;
            GameObject p1 = Instantiate(PikachuPrefab, new Vector3(closeNode[index1].x, closeNode[index1].y, 0), Quaternion.identity, Board.transform);
            p1.name = "Node " + closeNode[index1].x + " " + closeNode[index1].y;
            Pikachu np1 = p1.GetComponent<Pikachu>();
            np1.SetIconPikachu(PikachuImage[val]);
            np1.nodePikachu = closeNode[index1];
            grid[closeNode[index1].x, closeNode[index1].y] = closeNode[index1];
            closeNode.Remove(closeNode[index1]);

            //instance pikachu 2

            int index2 = Random.Range(0, closeNode.Count - 1);
            closeNode[index2].value = val;
            GameObject p2 = Instantiate(PikachuPrefab, new Vector3(closeNode[index2].x, closeNode[index2].y, 0), Quaternion.identity, Board.transform);
            p2.name = "Node " + closeNode[index2].x + " " + closeNode[index2].y;
            Pikachu np2 = p2.GetComponent<Pikachu>();
            np2.SetIconPikachu(PikachuImage[val]);
            np2.nodePikachu = closeNode[index2];
            grid[closeNode[index2].x, closeNode[index2].y] = closeNode[index2];
            closeNode.Remove(closeNode[index2]);

        }

        for (int y = 0; y < height + 2; y++)
        {
            for (int x = 0; x < width + 2; x++)
            {
                if (grid[x, y] == null)
                {
                    grid[x, y] = new Node(x, y, 0);
                }
            }
        }
    }
    public void NodeSelected(GameObject pikachu)
    {
        Pikachu np = pikachu.GetComponent<Pikachu>();
        np.SetBackgroundIsPick();

        if (ListWait.Count == 0)
        {
            ListPikachuSelected.Add(pikachu);
            ListNodeSelected.Add(np.nodePikachu);
        }

        if (ListNodeSelected.Count == 2)
        {
            if (ListNodeSelected[0] != ListNodeSelected[1] &&
                ListNodeSelected[0].value == ListNodeSelected[1].value)
            {
                Node[] check = CheckThreeLine(ListNodeSelected[0], ListNodeSelected[1]);

                if (check[1] != null && check[2] != null)
                {
                    DrawLine(check);
                    grid[ListNodeSelected[0].x, ListNodeSelected[0].y].value = 0;
                    grid[ListNodeSelected[1].x, ListNodeSelected[1].y].value = 0;
                    Destroy(ListPikachuSelected[0]);
                    Destroy(ListPikachuSelected[1]);
                }
                else
                {
                    ListWait.Add(ListPikachuSelected[0]);
                    ListWait.Add(ListPikachuSelected[1]);
                    StartCoroutine(RSBackground());
                }
            }
            else if (ListPikachuSelected[0] == ListPikachuSelected[1])
            {
                np.resetBackground();
            }
            else
            {
                ListWait.Add(ListPikachuSelected[0]);
                ListWait.Add(ListPikachuSelected[1]);
                StartCoroutine(RSBackground());
            }
            ListPikachuSelected.Clear();
            ListNodeSelected.Clear();
        }
    }

    private bool CheckLineX(Node a, Node b)
    {
        if (a == b && a.value == 0) { return true; }

        int minY = Mathf.Min(a.y, b.y);
        int maxY = Mathf.Max(a.y, b.y);

        for (int i = minY; i <= maxY; i++)
        {
            if (grid[a.x, i].value != 0)
            {
                return false;
            }
        }
        return true;
    }
    private bool CheckLineY(Node a, Node b)
    {
        if (a == b && a.value == 0) { return true; }

        int minX = Mathf.Min(a.x, b.x);
        int maxX = Mathf.Max(a.x, b.x);

        for (int i = minX; i <= maxX; i++)
        {
            if (grid[i, a.y].value != 0)
            {
                return false;
            }
        }
        return true;
    }
    private Node[] CheckThreeLine(Node a, Node b)
    {
        Node[] FourPoint = new Node[4];
        Node tempA, tempB;
        FourPoint[0] = a;
        FourPoint[3] = b;

        int minX = Mathf.Min(a.x, b.x);
        int maxX = Mathf.Max(a.x, b.x);

        int minY = Mathf.Min(a.y, b.y);
        int maxY = Mathf.Max(a.y, b.y);

        if (minX == maxX)
        {
            if ((grid[minX, minY + 1] == grid[minX, maxY]) || (CheckLineX(grid[minX, minY + 1], grid[minX, maxY - 1])))
            {
                FourPoint[1] = a;
                FourPoint[2] = b;
                return FourPoint;
            }
        }

        if (minY == maxY)
        {
            if ((grid[minX + 1, minY] == grid[maxX, minY]) || (CheckLineY(grid[minX + 1, minY], grid[maxX - 1, minY])))
            {
                FourPoint[1] = a;
                FourPoint[2] = b;
                return FourPoint;
            }
        }

        // check rectangle
        if ((a.x == minX && a.y == minY) || (b.x == minX && b.y == minY))
        {
            tempA = grid[minX, maxY];
            tempB = grid[maxX, minY];

            if (CheckLineX(grid[minX, minY + 1], tempA) && CheckLineY(grid[maxX - 1, maxY], tempA))
            {
                FourPoint[1] = tempA;
                FourPoint[2] = tempA;
                return FourPoint;
            }
            if (CheckLineX(grid[maxX, maxY - 1], tempB) && CheckLineY(grid[minX + 1, minY], tempB))
            {
                FourPoint[1] = tempB;
                FourPoint[2] = tempB;
                return FourPoint;
            }
        }
        else
        {
            tempA = grid[minX, minY];
            tempB = grid[maxX, maxY];

            if (CheckLineX(grid[minX, maxY - 1], tempA) && CheckLineY(grid[maxX - 1, minY], tempA))
            {
                FourPoint[1] = tempA;
                FourPoint[2] = tempA;
                return FourPoint;
            }
            if (CheckLineX(grid[maxX, minY + 1], tempB) && CheckLineY(grid[minX + 1, maxY], tempB))
            {
                FourPoint[1] = tempB;
                FourPoint[2] = tempB;
                return FourPoint;
            }
        }

        // right
        for (int i = minX + 1; i < maxX; i++)
        {
            tempA = grid[i, minY];
            tempB = grid[i, maxY];
            if ((a.x == minX && a.y == minY) || (b.x == minX && b.y == minY))
            {
                if (CheckLineX(tempA, tempB) && CheckLineY(grid[minX + 1, minY], tempA) && CheckLineY(tempB, grid[maxX - 1, maxY]))
                {
                    FourPoint[1] = tempA;
                    FourPoint[2] = tempB;
                    return FourPoint;
                }
            }
            else
            {
                if (CheckLineX(tempA, tempB) && CheckLineY(grid[maxX - 1, minY], tempA) && CheckLineY(tempB, grid[minX + 1, maxY]))
                {
                    FourPoint[1] = tempA;
                    FourPoint[2] = tempB;
                    return FourPoint;
                }
            }
        }

        // down
        for (int i = minY + 1; i < maxY; i++)
        {
            tempA = grid[minX, i];
            tempB = grid[maxX, i];
            if ((a.x == minX && a.y == minY) || (b.x == minX && b.y == minY))
            {
                if (CheckLineY(tempA, tempB) && CheckLineX(grid[minX, minY + 1], tempA) && CheckLineX(tempB, grid[maxX, maxY - 1]))
                {
                    FourPoint[1] = tempA;
                    FourPoint[2] = tempB;
                    return FourPoint;
                }
            }
            else
            {
                if (CheckLineY(tempA, tempB) && CheckLineX(grid[minX, maxY - 1], tempA) && CheckLineX(tempB, grid[maxX, minY + 1]))
                {
                    FourPoint[1] = tempA;
                    FourPoint[2] = tempB;
                    return FourPoint;
                }
            }
        }


        // check more
        for (int i = 1; i <= Mathf.Max(width, height); i++)
        {
            if ((a.x == minX && a.y == minY) || (b.x == minX && b.y == minY))
            {
                // more right
                if ((maxX + i) <= (width + 1))
                {
                    tempA = grid[maxX + i, minY];
                    tempB = grid[maxX + i, maxY];

                    if (CheckLineY(grid[minX + 1, minY], tempA) && CheckLineX(tempA, tempB) && CheckLineY(tempB, grid[maxX + 1, maxY]))
                    {
                        FourPoint[1] = tempA;
                        FourPoint[2] = tempB;
                        return FourPoint;
                    }
                }
                // more left
                if ((minX - i) >= 0)
                {
                    tempA = grid[minX - i, minY];
                    tempB = grid[minX - i, maxY];

                    if (CheckLineY(grid[minX - 1, minY], tempA) && CheckLineX(tempA, tempB) && CheckLineY(tempB, grid[maxX - 1, maxY]))
                    {
                        FourPoint[1] = tempA;
                        FourPoint[2] = tempB;
                        return FourPoint;
                    }
                }
                // more down
                if ((minY - i) >= 0)
                {
                    tempA = grid[minX, minY - i];
                    tempB = grid[maxX, minY - i];

                    if (CheckLineX(grid[minX, minY - 1], tempA) && CheckLineY(tempA, tempB) && CheckLineX(tempB, grid[maxX, maxY - 1]))
                    {
                        FourPoint[1] = tempA;
                        FourPoint[2] = tempB;
                        return FourPoint;
                    }
                }
                // more down
                if ((maxY + i) <= (height + 1))
                {
                    tempA = grid[minX, maxY + i];
                    tempB = grid[maxX, maxY + i];

                    if (CheckLineX(grid[minX, minY + 1], tempA) && CheckLineY(tempA, tempB) && CheckLineX(tempB, grid[maxX, maxY + 1]))
                    {
                        FourPoint[1] = tempA;
                        FourPoint[2] = tempB;
                        return FourPoint;
                    }
                }
            }
            else
            {
                // more right
                if ((maxX + i) <= (width + 1))
                {
                    tempA = grid[maxX + i, minY];
                    tempB = grid[maxX + i, maxY];

                    if (CheckLineY(grid[maxX + 1, minY], tempA) && CheckLineX(tempA, tempB) && CheckLineY(tempB, grid[minX + 1, maxY]))
                    {
                        FourPoint[1] = tempA;
                        FourPoint[2] = tempB;
                        return FourPoint;
                    }
                }
                // more left
                if ((minX - i) >= 0)
                {
                    tempA = grid[minX - i, minY];
                    tempB = grid[minX - i, maxY];

                    if (CheckLineY(grid[maxX - 1, minY], tempA) && CheckLineX(tempA, tempB) && CheckLineY(tempB, grid[minX - 1, maxY]))
                    {
                        FourPoint[1] = tempA;
                        FourPoint[2] = tempB;
                        return FourPoint;
                    }
                }
                // more up
                if ((minY - i) >= 0)
                {
                    tempA = grid[minX, minY - i];
                    tempB = grid[maxX, minY - i];

                    if (CheckLineX(grid[minX, maxY - 1], tempA) && CheckLineY(tempA, tempB) && CheckLineX(tempB, grid[maxX, minY - 1]))
                    {
                        FourPoint[1] = tempA;
                        FourPoint[2] = tempB;
                        return FourPoint;
                    }
                }
                // more down
                if ((maxY + i) <= (height + 1))
                {
                    tempA = grid[minX, maxY + i];
                    tempB = grid[maxX, maxY + i];

                    if (CheckLineX(grid[minX, maxY + 1], tempA) && CheckLineY(tempA, tempB) && CheckLineX(tempB, grid[maxX, minY + 1]))
                    {
                        FourPoint[1] = tempA;
                        FourPoint[2] = tempB;
                        return FourPoint;
                    }
                }
            }
        }
        return FourPoint;
    }
    public void GetHint()
    {
        Node current;
        Node[] listCheck;
        bool finded = false;
        for (int x = 1; x <= width;x++)
        {
            for(int y = 1; y <= height; y++)
            {
                if (grid[x, y].value > 0 ) 
                {
                    current = grid[x, y];

                    for (int i = 1; i <= width; i++)
                    {
                        for (int j = 1; j <= height; j++)
                        {
                            if (current != grid[i, j] && current.value == grid[i, j].value && !finded)
                            {
                                listCheck = CheckThreeLine(current, grid[i, j]);
                                if (listCheck[1] != null)
                                {
                                    GameObject p1 = GameObject.Find("Node " + x + " " + y);
                                    p1.GetComponent<Pikachu>().SetBackgroundIsPick();
                                    p1 = GameObject.Find("Node " + i + " " + j);
                                    p1.GetComponent<Pikachu>().SetBackgroundIsPick();
                                    finded = true;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    private void DrawLine(Node[] listNode)
    {
        StartCoroutine(FlashLine(listNode));
    }
    IEnumerator RSBackground()
    {
        yield return new WaitForSeconds(0.5f);
        ListWait[0].GetComponent<Pikachu>().resetBackground();
        ListWait[1].GetComponent<Pikachu>().resetBackground();
        ListWait.Clear();
    }
    IEnumerator FlashLine(Node[] listNode)
    {
        if (listNode[1].x != listNode[0].x && listNode[1].y != listNode[0].y)
        {
            Node temp = listNode[1];
            listNode[1] = listNode[2];
            listNode[2] = temp;
        }
        if (listNode[1] == listNode[2])
        {
            lineR.SetPosition(0, new Vector3(listNode[0].x, listNode[0].y, -1));
            lineR.SetPosition(1, new Vector3(listNode[1].x, listNode[1].y, -1));
            lineR.SetPosition(2, new Vector3(listNode[3].x, listNode[3].y, -1));
            lineR.SetPosition(3, new Vector3(listNode[3].x, listNode[3].y, -1));
        }
        else
        {
            for (int i = 0; i <= 3; i++)
            {
                lineR.SetPosition(i, new Vector3(listNode[i].x, listNode[i].y, -1));
            }
        }

        yield return new WaitForSeconds(0.3f);

        lineR.SetPosition(0, new Vector3(0, 0, 0));
        lineR.SetPosition(1, new Vector3(0, 0, 0));
        lineR.SetPosition(2, new Vector3(0, 0, 0));
        lineR.SetPosition(3, new Vector3(0, 0, 0));
    }
}

public class Node
{
    public int x;
    public int y;
    public int value;// 0: khong co gi,  2 3 ... la cac loai pikachu, 1 la vat can
    public Node(int newX, int newY, int value)
    {
        this.x = newX;
        this.y = newY;
        this.value = value;
    }
}