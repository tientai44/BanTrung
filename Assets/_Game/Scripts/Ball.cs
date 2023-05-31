using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BallColor
{
    Red, Green, Blue,Null
}
public enum BallState
{
    Idle,Moving,Fall
}
public class Ball : MonoBehaviour
{
    /// Static property
    public static Vector3 offset = new Vector3(-2f, 4, 5f);
    public static float BallRadius = 0.25f;
    /// 
    public string tagPool;
    [SerializeField] private BallColor color;
    private Rigidbody2D rb;
    public BallState state = BallState.Idle;
    private Transform tf;
    private int row, col;
    private float speed=10f;
    private CircleCollider2D circleCollider;
    public LayerMask sticker_Mask;
    List<Vector2> manyPosition = new List<Vector2>();
    public Transform TF
    {
        get
        {
            if (tf == null)
            {
                tf = transform;
            }
            return tf;
        }
    }

    public BallState State { get => state; set => state = value; }
    public BallColor Color { get => color; set => color = value; }
    public int Row { get => row; set => row = value; }
    public int Col { get => col; set => col = value; }
   
    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        gameObject.layer = 0;//Default Layer
        circleCollider = GetComponent<CircleCollider2D>();
    }
    public void OnInit()
    {
        row = -1; col = -1;
        state = BallState.Idle;
        rb.velocity = Vector2.zero;
    }
    private void Update()
    {
        //if(Input.GetKeyDown(KeyCode.J))
        //{
        //    rb.AddForce(new Vector2(100,100));
        //}
        
        if ( manyPosition.Count > 0)
        {
            if (Vector2.Distance(TF.position, manyPosition[0]) <= BallRadius)
            {
                if (manyPosition.Count == 1)
                {
                    circleCollider.radius = BallRadius;
                }
                manyPosition.RemoveAt(0);
            }
        }
        if(state is BallState.Moving && manyPosition.Count>0)
        {
            TF.position = Vector2.MoveTowards(TF.position,manyPosition[0],speed*Time.deltaTime);
        }

    }
    public void SetPos(int x, int y)
    {
        row = x; col = y;
        if (LevelManager.GetInstance().Row <= x)
        {
            LevelManager.GetInstance().AddLine();
        }
        if (LevelManager.GetInstance().Balls[x][y]!=null)
        {
            return;
        }
        LevelManager.GetInstance().Balls[x][y] = this;
        Vector3 offset1 = Vector3.zero;
        if (row % 2 == 1)
        {
            offset1 += Vector3.right / 4;
        }
        TF.position = new Vector3(col, -row, 0) / 2 + offset1 + Ball.offset;
        if (color is BallColor.Red)
        {
            LevelManager.GetInstance().Map[x][y] = 1;
        }
        if (color is BallColor.Green)
        {
            LevelManager.GetInstance().Map[x][y] = 2;
        }
        if (color is BallColor.Blue)
        {
            LevelManager.GetInstance().Map[x][y] = 3;
        }
        TF.SetParent(GameController.GetInstance().BallZone);
        gameObject.layer = 6;//WallLayer
    }
    public void StopMoving()
    {
        manyPosition.Clear();
        state = BallState.Idle;
        rb.velocity = Vector2.zero;
        circleCollider.radius = BallRadius;
    }
    public void AddForce(Vector2 force)
    {
        rb.AddForce(force);
    }
    public void Follow(List<Vector2> destinations)
    {
        this.manyPosition = destinations;
        state = BallState.Moving;
        circleCollider.radius = 0.01f;
    }
    public void PopBall(int point)
    {
        StopMoving();
        rb.gravityScale = 0;
        if (row >= 0 && col >= 0)
        {
            LevelManager.GetInstance().Balls[row][col] = null;
            LevelManager.GetInstance().Map[row][col] = 0;
        }
        gameObject.layer = 0;
        LevelManager.numBallColor[color] -= 1;

        BallPool.GetInstance().ReturnToPool(tagPool,gameObject);

        Constants.Score += point;
        CombatText cbt = BallPool.GetInstance().GetFromPool(Constants.CombatText_Point, TF.position).GetComponent<CombatText>();
        cbt.SetText(point.ToString());
        BallPool.GetInstance().ReturnToPool(Constants.CombatText_Point, cbt.gameObject,0.5f);
        UIManager.GetInstance().GetUI<UIGamePlay>().SetScoreText(Constants.Score);

    }
  
   
    IEnumerator ReadyCheckAround(float time)
    {
        yield return new WaitForSeconds(time);
        LevelManager.GetInstance().BFS_BallAround(row, col);
        
    }
    
    public void FallBall()
    {
        

        //Cho roi tu nhien hon

        rb.AddForce(new Vector2(Random.Range(-50, 50),Random.Range(0,50)));
        state = BallState.Fall;
        rb.gravityScale = 1f;
    }
    //public bool isStick()
    //{
    //    Collider2D[] hitColliders = Physics2D.OverlapCircleAll(new Vector2(TF.position.x,TF.position.y),0.25f,sticker_Mask);
        
    //    if(hitColliders.Length > 0) { return true; }
    //    return false;
    //}
    void CheckPos(Ball ball)
    {
        Vector3 direct = TF.position - ball.TF.position;
        //Debug.Log(direct);
        float angle = Vector2.Angle(Vector3.down, direct);
        //Debug.Log(angle);
        if (direct.x < 0)//left
        {
            if (angle <= 60)//leftbottom
            {
                if (ball.Row % 2 == 0 && ball.Col > 0)
                {
                    Debug.Log("LeftBottom Chan");
                    SetPos(ball.Row + 1, ball.Col - 1);
                }
                else
                {
                    Debug.Log("LeftBottom Le");
                    SetPos(ball.Row + 1, ball.Col);
                }
            }
            else if (angle <= 120)//left-side
            {
                Debug.Log("Left-Side");
                if (ball.Col > 0)// Con trong ben trai
                {
                    SetPos(ball.Row, ball.Col - 1);
                }
                else
                {
                    if (angle < 90)
                    {

                    }
                    else
                    {

                    }
                }
            }
            else//left-top
            {
                Debug.Log("Left-Top");
                if (ball.Row % 2 == 0 && ball.Col > 0)
                {
                    Debug.Log("LeftTop Chan");
                    SetPos(ball.Row - 1, ball.Col - 1);
                }
                else
                {
                    Debug.Log("LeftTop Le");
                    SetPos(ball.Row - 1, ball.Col);
                }
            }
        }
        if (direct.x > 0)//right
        {
            if (angle <= 60)//rightbottom
            {
                if (ball.Row % 2 == 0 || ball.Col >= LevelManager.GetInstance().Col - 1)
                {
                    Debug.Log("RightBottom Chan");
                    SetPos(ball.Row + 1, ball.Col);
                }
                else
                {
                    Debug.Log("RightBottom Le");
                    SetPos(ball.Row + 1, ball.Col + 1);
                }
            }
            else if (angle <= 120)//right-side
            {
                Debug.Log("Right-Side");
                if (ball.Col < LevelManager.GetInstance().Col - 1)// Con trong ben phai
                {
                    SetPos(ball.Row, ball.Col + 1);
                }
            }
            else//right-top
            {
                Debug.Log("Right-Top");
                if (ball.Row % 2 == 0 || ball.Col < LevelManager.GetInstance().Col - 1)
                {
                    Debug.Log("RightTop Chan");
                    SetPos(ball.Row - 1, ball.Col);
                }
                else
                {
                    Debug.Log("RightTop Le");
                    SetPos(ball.Row - 1, ball.Col + 1);
                }
            }
        }

        StopMoving();
        //Kiem tra an duoc khong
        StartCoroutine(ReadyCheckAround(1f));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Wall"))
        {
            rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);
        }
        if (collision.CompareTag("Top"))
        {
            if (state is not BallState.Fall)
            {
                StopMoving();
                float distance = TF.position.x - offset.x;
                distance *= 2;
                float gap = distance - (int)distance;
                if (gap > 0.5f)
                {
                    SetPos(0, (int)distance + 1);
                    //Kiem tra an duoc khong
                    StartCoroutine(ReadyCheckAround(1f));
                }
                else
                {
                    SetPos(0, (int)distance);
                    //Kiem tra an duoc khong
                    StartCoroutine(ReadyCheckAround(1f));
                }
            }
        }

        if (collision.CompareTag("Ball"))
        {
            if (state is BallState.Moving)
            {
                Debug.Log("Va Cham");
                Ball ball = collision.GetComponent<Ball>();
                CheckPos(ball);
            }
        }
        if (collision.CompareTag("Bottom") && state is BallState.Fall)
        {
            PopBall(Constants.BallFallPoint);
        }

    }

    public void ThrowUp()
    {
        state = BallState.Fall;
        AddForce(new Vector2(Random.Range(-100, 100), 300f));
        rb.gravityScale = 1f;
    }
}
