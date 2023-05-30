using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelManager : GOSingleton<LevelManager>
{

    private List<List<int>> map = new List<List<int>>();
    int row, col;
    List<List<Ball>> balls = new List<List<Ball>>();
    Vector3 firstBallPos = new Vector3(-2f, 4, 5f);

    private Queue<Ball> queue = new Queue<Ball>();
    private int rowDisplay=6;
   
    List<Ball> ballsListToPop = new List<Ball>();
    List<Ball> listSaveBalls = new List<Ball>();
    public int BallGrpCount = 1;
    public int Row { get => row; set => row = value; }
    public int Col { get => col; set => col = value; }
    public List<List<Ball>> Balls { get => balls; set => balls = value; }
    public List<List<int>> Map { get => map; set => map = value; }
    public Vector3 FirstBallPos { get => firstBallPos; set => firstBallPos = value; }

    private void Update()
    {
        
    }

    public void SetUp()
    {

        for (int i = 0; i < row; i++)
        {
            balls.Add(new List<Ball>());
        }
    }
    public void AddLine()
    {
        balls.Add(new List<Ball>());
        map.Add(new List<int>());
        row++;
        for (int i = 0; i < col; i++)
        {
            balls[row - 1].Add(null);
            map[row - 1].Add(0);
        }
        
    }
    public void RemoveLine()
    {
        balls.RemoveAt(row-1);
        map.RemoveAt(row-1);
        row--;
      
    }
    public void LoadLevel(int lv)
    {
        Ball.offset = FirstBallPos;
        string fileName = "Map/Level" + lv.ToString();
        ReadFile(fileName);

        SetUp();
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                Vector3 pos = new Vector3(j, -i, 0) / 2 + firstBallPos;
                if (i % 2 != 0)
                {
                    pos += Vector3.right / 4;
                }

                if (map[i][j] == 1)
                {
                    balls[i].Add(BallPool.GetInstance().GetFromPool(Constants.RedBall, pos).GetComponent<Ball>());
                }
                if (map[i][j] == 2)
                {
                    balls[i].Add(BallPool.GetInstance().GetFromPool(Constants.GreenBall, pos).GetComponent<Ball>());
                }
                if (map[i][j] == 3)
                {
                    balls[i].Add(BallPool.GetInstance().GetFromPool(Constants.BlueBall, pos).GetComponent<Ball>());
                }
                if (map[i][j] == 0)
                {
                    balls[i].Add(null);
                    continue;
                }
                balls[i][j].SetPos(i, j);
                
            }
        }
        ResetLine();
    }
    public void ResetLine()
    {
        for(int i=row-1; i>=0; i--)
        {
            bool flag = false;
            for(int j=col-1; j>=0; j--)
            {
                if (map[i][j] != 0)
                {
                    flag = true;
                    break;
                }
               
            }
            if (!flag)
            {
                RemoveLine();
            }
            else
            {
                break;
            }
        }
        Debug.Log("Reset");
        if (row >= rowDisplay)
        {
            Debug.Log(Ball.offset);
            Ball.offset = firstBallPos + new Vector3(0, 0.5f, 0) * (row - rowDisplay);
            Debug.Log(Ball.offset);
            GameController.GetInstance().SetLine(row - rowDisplay);
        }
        else
        {
            Ball.offset = firstBallPos;
            GameController.GetInstance().SetLine(0);
        }
    }
    //public void PopBallAround(int x, int y)
    //{
    //    BallColor color = balls[x][y].Color;
    //    if (balls[x + 1][y].Color == color)
    //    {
    //        balls[x + 1][y].PopBall();
    //    }
    //    if (balls[x - 1][y].Color == color)
    //    {
    //        balls[x - 1][y].PopBall();
    //    }
    //    if (balls[x][y + 1].Color == color)
    //    {
    //        balls[x][y + 1].PopBall();
    //    }
    //    if (balls[x][y - 1].Color == color)
    //    {
    //        balls[x][y - 1].PopBall();
    //    }
    //}
    public void ReadFile(string fileName)
    {
        var textFile = Resources.Load<TextAsset>(fileName);

        string text = textFile.text;
        string[] arrListStr = text.Split('\n');
        row = arrListStr.Length;
        //map = new int[arrListStr.Length][];

        for (int i = 0; i < row; i++)
        {
            map.Add(new List<int>());
        }

        for (int i = 0; i < arrListStr.Length; i++)
        {
            string[] temp = arrListStr[i].Split(' ');
            col = temp.Length;
            for (int j = 0; j < col; j++)
            {
                //map[i][j] = int.Parse(temp[j]);
                map[i].Add(int.Parse(temp[j]));
            }
        }
        
    }
    public void BFS_BallAround(int posX, int posY)
    {
        queue.Clear();
        queue.Enqueue(balls[posX][posY]);
        ballsListToPop.Add(balls[posX][posY]);

        while (queue.Count > 0)
        {
            Ball ball = queue.Dequeue();
            CheckAroundSameColor(ball.Row, ball.Col, ballsListToPop);
        }
       
         //Sau moi luot ban
         StartCoroutine(CheckAll(1f));
        
    }
    IEnumerator CheckAll(float time)
    {
        bool flag =PopAllBall();
        if (flag == true)
        {
            // Kiem tra cac qua bong lo lung
            yield return new WaitForSeconds(time);
            BFS_BallCheckAll();
        }
        yield return new WaitForSeconds(time);
        ResetLine();
    }
    public void BFS_BallCheckAll()
    {
        Debug.Log("Check All");
        queue.Clear();
        for (int j = 0; j < col; j++)
        {
            if (balls[0][j] != null)
            {
                AddToGroup(balls[0][j],listSaveBalls);
            }
        }
        while (queue.Count > 0)
        {
            Ball ball = queue.Dequeue();
            CheckAround(ball.Row, ball.Col, listSaveBalls);
        }
       
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (balls[i][j] != null)
                {
                    if (!listSaveBalls.Contains(balls[i][j]))
                    {
                        balls[i][j].FallBall();
                    }
                }
            }
        }
        listSaveBalls.Clear();
    }
    void AddToGroup(Ball b, List<Ball> listBall)
    {
        if (!listBall.Contains(b))
        {
            listBall.Add(b);
            queue.Enqueue(b);
        }
    }
    public bool PopAllBall()
    {
        bool res=false;
        if (ballsListToPop.Count >= 3)
        {
            res = true;
            foreach (Ball b in ballsListToPop)
            {
                b.PopBall(50);
            }
        }
        ballsListToPop.Clear();
        return res;


    }
    void AddToCheckList(Ball b)
    {
        if (!listSaveBalls.Contains(b))
        {
            listSaveBalls.Add(b);
        }
    }
    public void CheckAroundSameColor(int posX, int posY, List<Ball> listBall)
    {
        Ball temp = balls[posX][posY];
        Ball b;
        if (posX % 2 == 0)// Hang Chan
        {
            if (posX > 0)
            {
                b = balls[posX - 1][posY];

                if (b != null)
                {
                    if (temp.Color == b.Color)//right-top
                    {
                        AddToGroup(b, listBall);
                    }
                }
                if (posY > 0)
                {
                    b = balls[posX - 1][posY - 1];
                    if (b != null)
                    {
                        if (temp.Color == b.Color)//left-top
                        {
                            AddToGroup(b, listBall);
                        }
                    }
                }
            }
            if (posY > 0)
            {
                b = balls[posX][posY - 1];
                if (b != null)
                {
                    if (temp.Color == b.Color)//left-side
                    {
                        AddToGroup(b, listBall);
                    }
                }
                if (posX < Row - 1)
                {
                    b = balls[posX + 1][posY - 1];
                    if (b != null)
                    {
                        if (b.Color == temp.Color)//left-bottom
                        {
                            AddToGroup(b, listBall);
                        }
                    }
                }
            }
            if (posY < Col - 1)
            {
                b = balls[posX][posY + 1];
                if (b != null)
                {
                    if (b.Color == temp.Color)//right-side
                    {
                        AddToGroup(b, listBall);
                    }
                }
            }
            if (posX < Row - 1)
            {
                b = balls[posX + 1][posY];
                if (b != null)
                {
                    if (b.Color == temp.Color)//right-bottom
                    {
                        AddToGroup(b, listBall);

                    }
                }
            }
        }

        if (posX % 2 != 0)// Hang Le
        {
            if (posX > 0)
            {
                b = balls[posX - 1][posY + 1];
                if (posY < Col - 1)
                {
                    if (b != null)
                    {
                        if (temp.Color == b.Color)//right-top x
                        {
                            AddToGroup(b, listBall);
                        }
                    }
                }
                b = balls[posX - 1][posY];
                if (b != null)
                {
                    if (temp.Color == b.Color)//left-top x
                    {
                        AddToGroup(b, listBall);
                    }
                }

            }
            if (posY > 0)
            {
                b = balls[posX][posY - 1];
                if (b != null)
                {
                    if (temp.Color == b.Color)//left-side x
                    {
                        AddToGroup(b, listBall);

                    }
                }

            }
            if (posY < Col - 1)
            {
                b = balls[posX][posY + 1];
                if (b != null)
                {
                    if (b.Color == temp.Color)//right-side x
                    {
                        AddToGroup(b, listBall);
                    }
                }
            }
            if (posX < Row - 1)
            {
                if (posY < Col - 1)
                {
                    b = balls[posX + 1][posY + 1];
                    if (b != null)
                    {
                        if (b.Color == temp.Color)//right-bottom
                        {
                            AddToGroup(b, listBall);
                        }
                    }
                }
                b = balls[posX + 1][posY];
                if (balls[posX + 1][posY] != null)
                {
                    if (b.Color == temp.Color)//left-bottom x
                    {
                        AddToGroup(b, listBall);
                    }
                }
            }
        }
    }
    public void CheckAround(int posX, int posY, List<Ball> listBall)
    {
        Ball temp = balls[posX][posY];
        Ball b;
        if (posX % 2 == 0)// Hang Chan
        {
            if (posX > 0)
            {
                b = balls[posX - 1][posY];

                if (b != null)
                {
                    AddToGroup(b, listBall);
                }
                if (posY > 0)
                {
                    b = balls[posX - 1][posY - 1];
                    if (b != null)
                    {
                        AddToGroup(b, listBall);
                    }
                }
            }
            if (posY > 0)
            {
                b = balls[posX][posY - 1];
                if (b != null)
                {
                    AddToGroup(b, listBall);
                }
                if (posX < Row - 1)
                {
                    b = balls[posX + 1][posY - 1];
                    if (b != null)
                    {
                        AddToGroup(b, listBall);
                    }
                }
            }
            if (posY < Col - 1)
            {
                b = balls[posX][posY + 1];
                if (b != null)
                {
                    AddToGroup(b, listBall);
                }
            }
            if (posX < Row - 1)
            {
                b = balls[posX + 1][posY];
                if (b != null)
                {
                    AddToGroup(b, listBall);
                }
            }
        }

        if (posX % 2 != 0)// Hang Le
        {
            if (posX > 0)
            {
                b = balls[posX - 1][posY + 1];
                if (posY < Col - 1)
                {
                    if (b != null)
                    {
                        AddToGroup(b, listBall);
                    }
                }
                b = balls[posX - 1][posY];
                if (b != null)
                {
                    AddToGroup(b, listBall);
                }

            }
            if (posY > 0)
            {
                b = balls[posX][posY - 1];
                if (b != null)
                {
                    AddToGroup(b, listBall);
                }

            }
            if (posY < Col - 1)
            {
                b = balls[posX][posY + 1];
                if (b != null)
                {
                    AddToGroup(b, listBall);
                }
            }
            if (posX < Row - 1)
            {
                if (posY < Col - 1)
                {
                    b = balls[posX + 1][posY + 1];
                    if (b != null)
                    {
                        AddToGroup(b, listBall);
                    }
                }
                b = balls[posX + 1][posY];
                if (balls[posX + 1][posY] != null)
                {
                    AddToGroup(b, listBall);
                }
            }
        }
    }
}

