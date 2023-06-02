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
    public static float BallRadius = 0.2f;
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
    List<Vector2> destinations = new List<Vector2>();
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
        gameObject.layer = 0;//Default
    }
    private void Update()
    {
        if ( destinations.Count >0)
        {
            if (Vector2.Distance(TF.position, destinations[0]) <= BallRadius)
            {
                if (destinations.Count == 1)
                {
                    //circleCollider.enabled = true;
                    circleCollider.radius = BallRadius;
                }
            }
            if (Vector2.Distance(TF.position, destinations[0]) <= 0.001f)
            {
                destinations.RemoveAt(0);
            }
        }
        if(state is BallState.Moving && destinations.Count>0)
        {
            TF.position = Vector2.MoveTowards(TF.position,destinations[0],speed*Time.deltaTime);
        }

    }
    public bool SetPos(int x, int y)
    {
        row = x; col = y;
        if (LevelManager.GetInstance().Row <= x)
        {
            LevelManager.GetInstance().AddLine();
        }
        if (LevelManager.GetInstance().Balls[x][y]!=null)
        {
            return false;
        }
        LevelManager.GetInstance().Balls[x][y] = this;
        Vector3 offset1 = Vector3.zero;
        if (row % 2 == 1)
        {
            offset1 += Vector3.right *BallRadius;
        }
        TF.position = new Vector3(col, -row/Mathf.Tan(30*Mathf.Deg2Rad)/2, 0)*BallRadius*2 + offset1 + Ball.offset;
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
        return true;
    }
    public void StopMoving()
    {
        destinations.Clear();
        state = BallState.Idle;
        rb.velocity = Vector2.zero;
        //circleCollider.enabled = true;
        circleCollider.radius = BallRadius;
    }
    public void AddForce(Vector2 force)
    {
        rb.AddForce(force);
    }
    public void Follow(List<Vector2> destinations)
    {
        this.destinations = destinations;
        state = BallState.Moving;
        circleCollider.radius = 0f;
        Debug.Log(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position));
        Debug.Log("Shooted " + destinations.Count);
        //circleCollider.enabled = false;
    }
    public void PopBall(int point)
    {
        StopMoving();
        rb.gravityScale = 0;
        if (row < LevelManager.GetInstance().Row && col >= 0)
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
        if (row >= 0 && col >= 0)
        {
            LevelManager.GetInstance().Balls[row][col] = null;
            LevelManager.GetInstance().Map[row][col] = 0;
        }

        //Cho roi tu nhien hon
        rb.AddForce(new Vector2(Random.Range(-50, 50),Random.Range(0,50)));
        state = BallState.Fall;
        rb.gravityScale = 2f;
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
        Debug.Log(ball.Row);
        Debug.Log(ball.Col);
        if (direct.x < 0)//left
        {
            if (angle <= 60)//leftbottom
            {
                if (ball.Row % 2 == 0 && ball.Col > 0)
                {
                    Debug.Log("LeftBottom Chan");
                    if(!SetPos(ball.Row + 1, ball.Col - 1))
                    {
                        SetPos(ball.Row + 1, ball.Col);
                    }
                }
                else
                {
                    Debug.Log("LeftBottom Le");
                    if(!SetPos(ball.Row + 1, ball.Col))
                    {
                        SetPos(ball.Row+2, ball.Col);
                    }
                }
            }
            else if (angle <= 120)//left-side
            {
                Debug.Log("Left-Side");
                if (ball.Col > 0)// Con trong ben trai
                {
                    if(!SetPos(ball.Row, ball.Col - 1))
                    {
                        if (angle <= 90)
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
                        else
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
                    
                }
                
            }
            else//left-top
            {
                Debug.Log("Left-Top");
                if (ball.Row % 2 == 0 && ball.Col > 0)
                {
                    Debug.Log("LeftTop Chan");
                    if(!SetPos(ball.Row - 1, ball.Col - 1))
                    {
                        SetPos(ball.Row, ball.Col-1); //Left
                    }
                }
                else
                {
                    Debug.Log("LeftTop Le");
                    if(!SetPos(ball.Row - 1, ball.Col))
                    {
                        SetPos(ball.Row, ball.Col - 1);//Left
                    }
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
                    if(!SetPos(ball.Row + 1, ball.Col))
                    {
                        SetPos(ball.Row, ball.Col + 1);//Right
                    }
                }
                else
                {
                    Debug.Log("RightBottom Le");
                    if(!SetPos(ball.Row + 1, ball.Col + 1))
                    {
                        SetPos(ball.Row, ball.Col + 1);//Right
                    }
                }
            }
            else if (angle <= 120)//right-side
            {
                Debug.Log("Right-Side");
                if (ball.Col < LevelManager.GetInstance().Col - 1)// Con trong ben phai
                {
                    if(!SetPos(ball.Row, ball.Col + 1))
                    {
                        if (angle <= 90)
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
                        else
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
                }
            }
            else//right-top
            {
                Debug.Log("Right-Top");
                if (ball.Row % 2 == 0 || ball.Col < LevelManager.GetInstance().Col - 1)
                {
                    Debug.Log("RightTop Chan");
                    if(!SetPos(ball.Row - 1, ball.Col))
                    {
                        SetPos(ball.Row, ball.Col + 1);//Right
                    }
                }
                else
                {
                    Debug.Log("RightTop Le");
                    if(SetPos(ball.Row - 1, ball.Col + 1))
                    {
                        SetPos(ball.Row, ball.Col + 1);//Right
                    }
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
                distance /=Ball.BallRadius*2;
                float gap = distance - (int)distance;
                if (gap > 0.5f)//nghieng ve ben phai
                {
                    if(!SetPos(0, (int)distance + 1))
                    {
                        SetPos(0, (int)distance);
                    }
                    //Kiem tra an duoc khong
                    StartCoroutine(ReadyCheckAround(1f));
                }
                else
                {
                    if(!SetPos(0, (int)distance))
                    {
                        SetPos(0, (int)distance+1);
                    }
                    //Kiem tra an duoc khong
                    StartCoroutine(ReadyCheckAround(1f));
                }
            }
        }

        if (collision.CompareTag("Ball"))
        {
            Ball ball = collision.GetComponent<Ball>();
            //if (State is BallState.Idle)
            //{
            //    if (ball.State is BallState.Moving)
            //    {
            //        Debug.Log("Bound");
            //        Vector2 direct = ball.TF.position - TF.position;
            //        float max = Mathf.Max(Mathf.Abs(direct.x), Mathf.Abs(direct.y));
            //        ball.Bound(direct / max);
            //    }
            //}
            if (state is BallState.Moving)
            {
                CheckBallAround();
                Debug.Log("Va Cham");
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
        AddForce(new Vector2(Random.Range(-100, 100), 400f));
        state = BallState.Fall;
        rb.gravityScale = 2f;
    }
    IEnumerator Bound(Vector2 direction)
    {
        Vector3 oldPos = TF.position;
        Vector3 target = TF.position + (Vector3)direction * Ball.BallRadius / 4;
        while (Vector2.Distance(target,TF.position)>=0.001f)
        {
            //Debug.Log("Moving");
            TF.position = Vector3.Lerp(TF.position,target,0.5f);
            yield return new WaitForSeconds(Time.deltaTime * 5);
        }
        while (Vector2.Distance(oldPos, TF.position) >= 0.001f)
        {
            TF.position = Vector3.Lerp(TF.position, oldPos, 0.5f);
            yield return new WaitForSeconds(Time.deltaTime * 5);
        }

        //TF.position += (Vector3)direction*Ball.BallRadius/4;
        //yield return new WaitForSeconds(Time.deltaTime*10);
        //Debug.Log((Vector3)direction * Ball.BallRadius / 4);
        //TF.position = oldPos;
    }
    public void CheckBallAround()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, BallRadius);

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Ball"))
            {
                Ball ball = collider.GetComponent<Ball>();
                if(ball ==this)
                {
                    continue;
                }
                //Debug.Log("Bound");
                Vector2 direct = ball.TF.position - TF.position;
                float max = Mathf.Max(Mathf.Abs(direct.x), Mathf.Abs(direct.y));
                Debug.Log(max);
                Debug.Log(direct / max);
                ball.StartCoroutine(ball.Bound(direct / max));

            }
        }
    }
}
