using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class G4_LevelManager : G4_GOSingleton<G4_LevelManager>
{
    //Static 
    public static int CurrentLevel;
    public static List<int> CheckPoints = new List<int> { 200,300,400};
    public static int  targetNum;
    //
    public List<G4_LevelScriptTableObject> levelInfors;
    private List<List<int>> map = new List<List<int>>();
    int row, col;
    List<List<G4_Ball>> balls = new List<List<G4_Ball>>();
    Vector3 firstBallPos = new Vector3(-2f, 4, 5f);
    
    private Queue<G4_Ball> queue = new Queue<G4_Ball>();
    private int rowDisplay=10;
   
    List<G4_Ball> ballsListToPop = new List<G4_Ball>();
    List<G4_Ball> listSaveBalls = new List<G4_Ball>();
    public int BallGrpCount = 1;
    public static Dictionary<BallColor, int> numBallColor = new Dictionary<BallColor, int>();

    public int Row { get => row; set => row = value; }
    public int Col { get => col; set => col = value; }
    public List<List<G4_Ball>> Balls { get => balls; set => balls = value; }
    public List<List<int>> Map { get => map; set => map = value; }
    public Vector3 FirstBallPos { get => firstBallPos; set => firstBallPos = value; }

   
    public void SetUp()
    {
        
        balls.Clear();
        for (int i = 0; i < row; i++)
        {
            balls.Add(new List<G4_Ball>());
        }
    }
    public void AddLine()
    {
        balls.Add(new List<G4_Ball>());
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
        firstBallPos = G4_GameController.GetInstance().FirstBall.position;
        numBallColor[BallColor.Red] = 0;
        numBallColor[BallColor.Green] = 0;
        numBallColor[BallColor.Blue] = 0;
        numBallColor[BallColor.Rabbit] = 0;
        numBallColor[BallColor.FullColor] = 0;
        numBallColor[BallColor.FireBall] = 0;
        numBallColor[BallColor.Bomb] = 0;
        //BallPool.GetInstance().ClearObjectActive(Constants.RedBall);
        //BallPool.GetInstance().ClearObjectActive(Constants.BlueBall);
        //BallPool.GetInstance().ClearObjectActive(Constants.GreenBall);
        //BallPool.GetInstance().ClearObjectActive(Constants.Rabbit);
        G4_BallPool.GetInstance().ClearAllObjectActive();
        G4_GameController.GetInstance().ResetScore();
        balls.Clear();
        map.Clear();

        CurrentLevel = lv;
        G4_Ball.offset = FirstBallPos;
        G4_LevelScriptTableObject levelInfor = levelInfors[lv - 1];
        string fileName = "Map/" + levelInfor.Filename;
        CheckPoints = levelInfor.CheckPoints;
        ReadFile(fileName);
        
        SetUp();
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                Vector3 pos = new Vector3(j, -i, 0) / 2 + firstBallPos;
                if (i % 2 != 0)
                {
                    pos += Vector3.right *G4_Ball.BallRadius;
                }
                balls[i].Add(null);
                G4_Ball ball =null;
                if (map[i][j] == 1)
                {
                    //balls[i].Add(BallPool.GetInstance().GetFromPool(Constants.RedBall, pos).GetComponent<Ball>());
                    ball = G4_BallPool.GetInstance().GetFromPool(G4_Constants.RedBall, pos).GetComponent<G4_Ball>();
                    numBallColor[BallColor.Red] += 1;
    }
                if (map[i][j] == 2)
                {
                    //balls[i].Add(BallPool.GetInstance().GetFromPool(Constants.GreenBall, pos).GetComponent<Ball>());
                    ball = G4_BallPool.GetInstance().GetFromPool(G4_Constants.GreenBall, pos).GetComponent<G4_Ball>();
                    numBallColor[BallColor.Green] += 1;
                }
                if (map[i][j] == 3)
                {
                    ball = G4_BallPool.GetInstance().GetFromPool(G4_Constants.BlueBall, pos).GetComponent<G4_Ball>();
                    numBallColor[BallColor.Blue] += 1;
                }
                if (map[i][j] == 4)
                {
                    ball = G4_BallPool.GetInstance().GetFromPool(G4_Constants.Rabbit, pos).GetComponent<G4_Ball>();
                    numBallColor[BallColor.Rabbit] += 1;
                }
                if (map[i][j] == 0)
                {
                    
                    continue;
                }
                ball.OnInit();
                ball.SetPos(i, j);
                
                
            }
        }
        
        if (levelInfor.LevelType is G4_LevelType.ClearBall)
        {
            int sum = numBallColor[BallColor.Red] + numBallColor[BallColor.Green] + numBallColor[BallColor.Blue];
            G4_UIManager.GetInstance().GetUI<G4_UIGamePlay>().SetMissionProcess(sum.ToString());
        }
        else if(levelInfor.LevelType is G4_LevelType.SaveRabbit)
        {
            targetNum = numBallColor[BallColor.Rabbit];
            G4_UIManager.GetInstance().GetUI<G4_UIGamePlay>().SetMissionProcess("0"+"/"+ targetNum.ToString());
        }
        else if (levelInfor.LevelType is G4_LevelType.CollectFlower)
        {
            SetUpLeaf();
            targetNum = 6;
            G4_UIManager.GetInstance().GetUI<G4_UIGamePlay>().SetMissionProcess(SearchFirstRow().ToString() + "/" + targetNum.ToString());
        }
        G4_UIManager.GetInstance().GetUI<G4_UIGamePlay>().SetShooterPosition();
        G4_UIManager.GetInstance().GetUI<G4_UIGamePlay>().SetMissionImg(levelInfor.LevelType);
        G4_Shooter.GetInstance().OnInit(levelInfor.NumBall);
        
        ResetLine();
    }
    public void SetUpLeaf()
    {
        for(int i = 0; i < col - 1; i++)
        {
            Vector3 pos = new Vector3(i, 0, 0) * G4_Ball.BallRadius * 2  + G4_Ball.offset; ;
            GameObject leaf = G4_BallPool.GetInstance().GetFromPool(G4_Constants.Leaf, pos);
            leaf.transform.SetParent(G4_GameController.GetInstance().BallZone);
        }
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
            G4_Ball.offset = firstBallPos + new Vector3(0, G4_Ball.BallRadius*2, 0) * (row - rowDisplay);
            G4_GameController.GetInstance().SetLine(row - rowDisplay);
        }
        else
        {
            G4_Ball.offset = firstBallPos;
            G4_GameController.GetInstance().SetLine(0);
        }
    }
   
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
            G4_Ball ball = queue.Dequeue();
            CheckAroundSameColor(ball.Row, ball.Col, ballsListToPop);
        }
       
         //Sau moi luot ban
         StartCoroutine(IECheckAll(1f));
        
    }
    public IEnumerator IECheckAll(float time)
    {
        //bool flag =PopAllBall();
        //if (flag == true)
        //{
        //    // Kiem tra cac qua bong lo lung
        //    yield return new WaitForSeconds(time);
        //    flag = BFS_BallCheckAll();
        //}
        //if (flag == true)
        //{
        //    yield return new WaitForSeconds(time * 2);
        //}
        ////Check Game
        //ResetLine();
        //yield return new WaitUntil(() => PopAllBall()); // Đợi cho đến khi PopAllBall() trả về true
        yield return StartCoroutine(PopAllBall());

        // Kiểm tra các quả bóng lơ lửng
        yield return new WaitUntil(() => BFS_BallCheckAll()); // Đợi cho đến khi BFS_BallCheckAll() trả về true

        yield return new WaitForSeconds(1f);
        if (levelInfors[CurrentLevel - 1].LevelType is G4_LevelType.ClearBall)
        {
            int sum = numBallColor[BallColor.Red] + numBallColor[BallColor.Green] + numBallColor[BallColor.Blue];
            G4_UIManager.GetInstance().GetUI<G4_UIGamePlay>().SetMissionProcess(sum.ToString());
        }
        else if (levelInfors[CurrentLevel - 1].LevelType is G4_LevelType.SaveRabbit)
        {
            G4_UIManager.GetInstance().GetUI<G4_UIGamePlay>().SetMissionProcess((targetNum - numBallColor[BallColor.Rabbit]).ToString() + "/" + targetNum.ToString());
        }else if (levelInfors[CurrentLevel - 1].LevelType is G4_LevelType.CollectFlower)
        {
            G4_UIManager.GetInstance().GetUI<G4_UIGamePlay>().SetMissionProcess(SearchFirstRow().ToString() + "/" + targetNum.ToString());
        }
        // Kiểm tra Game
        ResetLine();


    }
    bool  BFS_BallCheckAll()
    {
        queue.Clear();
        if (row <= 0)// Het Bong
        {
            return true;
        }
        for (int j = 0; j < col; j++)
        {
            if (balls[0][j] != null)
            {
                AddToGroup(balls[0][j],listSaveBalls);
            }
        }
        while (queue.Count > 0)
        {
            G4_Ball ball = queue.Dequeue();
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
        return true;
    }
    void AddToGroup(G4_Ball b, List<G4_Ball> listBall)
    {
        if (!listBall.Contains(b))
        {
            listBall.Add(b);
            queue.Enqueue(b);
        }
    }
    public IEnumerator PopAllBall()
    {
        if (ballsListToPop.Count >= 3 || G4_Shooter.GetInstance().Mode is ShooterMode.FullColor)
        {
            foreach (G4_Ball b in ballsListToPop)
            {
                b.PopBall(G4_Constants.BallPopPoint);
                yield return new WaitForSeconds(Time.deltaTime*2);
            }
        }
        ballsListToPop.Clear();
        
    }
    
    public IEnumerator PopListBall(List<G4_Ball> balls,float time)
    {
        yield return new WaitForSeconds(time);
        for(int i = 0;i < balls.Count; i++)
        {
            balls[i].PopBall(G4_Constants.BallPopPoint);
        }
        yield return StartCoroutine(IECheckAll(1f));
    }
    void AddToCheckList(G4_Ball b)
    {
        if (!listSaveBalls.Contains(b))
        {
            listSaveBalls.Add(b);
        }
    }
    public void CheckAroundSameColor(int posX, int posY, List<G4_Ball> listBall)
    {
        G4_Ball temp = balls[posX][posY];
        G4_Ball b;
        if (posX % 2 == 0)// Hang Chan
        {
            if (posX > 0)
            {
                b = balls[posX - 1][posY];

                if (b != null)
                {
                    if (temp.IsEqualColor(b))//right-top
                    {
                        AddToGroup(b, listBall);
                    }
                }
                if (posY > 0)
                {
                    b = balls[posX - 1][posY - 1];
                    if (b != null)
                    {
                        if (temp.IsEqualColor(b))//left-top
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
                    if (temp.IsEqualColor(b))//left-side
                    {
                        AddToGroup(b, listBall);
                    }
                }
                if (posX < Row - 1)
                {
                    b = balls[posX + 1][posY - 1];
                    if (b != null)
                    {
                        if (temp.IsEqualColor(b))//left-bottom
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
                    if (temp.IsEqualColor(b))//right-side
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
                    if (temp.IsEqualColor(b))//right-bottom
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
                        if (temp.IsEqualColor(b))//right-top x
                        {
                            AddToGroup(b, listBall);
                        }
                    }
                }
                b = balls[posX - 1][posY];
                if (b != null)
                {
                    if (temp.IsEqualColor(b))//left-top x
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
                    if (temp.IsEqualColor(b))//left-side x
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
                    if (temp.IsEqualColor(b))//right-side x
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
                        if (temp.IsEqualColor(b))//right-bottom
                        {
                            AddToGroup(b, listBall);
                        }
                    }
                }
                b = balls[posX + 1][posY];
                if (balls[posX + 1][posY] != null)
                {
                    if (temp.IsEqualColor(b))//left-bottom x
                    {
                        AddToGroup(b, listBall);
                    }
                }
            }
        }
    }
    public void CheckAround(int posX, int posY, List<G4_Ball> listBall)
    {
        G4_Ball temp = balls[posX][posY];
        G4_Ball b;
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

    public bool CheckWin()
    {
        G4_LevelScriptTableObject levelInfor = levelInfors[CurrentLevel-1];
       
        if (levelInfor.LevelType is G4_LevelType.ClearBall)
        {
            if(numBallColor[BallColor.Red] + numBallColor[BallColor.Green] + numBallColor[BallColor.Blue] == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (levelInfor.LevelType is G4_LevelType.SaveRabbit)
        {
            if (numBallColor[BallColor.Rabbit] == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if(levelInfor.LevelType is G4_LevelType.CollectFlower)
        {
            if (SearchFirstRow()>=targetNum)
            {
                return true;
            }
            else { return false; }
        }
        return false;
    }
    public IEnumerator FallAllBall(float time)
    {
        for(int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (balls[i][j] != null)
                {
                    balls[i][j].FallBall();
                    yield return new WaitForSeconds(time);
                }
            }
        }
    }
    public int SearchFirstRow()
    {
        int res = 0;
        if(row <= 0)
        {
            return col;
        }
        else
        {
            for( int i = 0; i < col-1; i++)
            {
                
                if (balls[0][i] == null)
                {
                    res++;
                }
            }
        }
        return res;
    }
}

