using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public enum BallColor
{
    Red, Green, Blue,Null,Rabbit,FullColor,Bomb,FireBall
}
public enum BallState
{
    Idle,Moving,Fall
}
public class G4_Ball : MonoBehaviour
{
    /// Static property
    public static Vector3 offset = new Vector3(-2f, 4, 5f);
    public static float BallRadius = 0.2f;
    /// 
    public string tagPool;
    [SerializeField] private BallColor color;
    [SerializeField] ParticleSystem effect;
    [SerializeField] private GameObject elementEffect;

    private Rigidbody2D rb;
    public BallState state = BallState.Idle;
    private Transform tf;
    private int row, col;
    private float speed=10f;
    private CircleCollider2D circleCollider;
    List<Vector2> destinations = new List<Vector2>();
    private Collider2D targetCollider;
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
        targetCollider = null;
        if (elementEffect != null)
        {
            elementEffect.SetActive(true);
        }
    }
    private void Update()
    {
        if ( destinations.Count >0)
        {
            Vector3 direction =  destinations[0] - (Vector2)TF.position;

            // Tính toán góc quay từ vector hướng
            float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;

            // Áp dụng góc quay cho trục Z của transform
            TF.eulerAngles = new Vector3(0, 0,-angle);

            if (Vector2.Distance(TF.position, destinations[0]) <= BallRadius)
            {
                if (destinations.Count == 1)
                {
                    if (targetCollider != null)
                    {
                        if (targetCollider.CompareTag("Ball"))
                        {
                            StopMoving();
                            BoundBallAround();
                            G4_Ball b = targetCollider.GetComponent<G4_Ball>();
                            CheckPos(b);
                        }
                        if (targetCollider.CompareTag("Top"))
                        {
                            StopMoving();
                            float distance = TF.position.x - offset.x;
                            distance /= G4_Ball.BallRadius * 2;
                            float gap = distance - (int)distance;
                            if (gap > 0.5f)//nghieng ve ben phai
                            {
                                if (!SetPos(0, (int)distance + 1))
                                {
                                    SetPos(0, (int)distance);
                                }
                                ////Kiem tra an duoc khong
                                //StartCoroutine(IEReadyCheckAround(Time.deltaTime*20));
                                
                            }
                            else
                            {
                                if (!SetPos(0, (int)distance))
                                {
                                    SetPos(0, (int)distance + 1);
                                }
                                ////Kiem tra an duoc khong

                                //StartCoroutine(IEReadyCheckAround(Time.deltaTime * 20));
                            }
                            //Kiem tra an duoc khong
                            StartCoroutine(IEReadyCheckAround(Time.deltaTime * 20));
                            if(Color is BallColor.Bomb)
                            {
                                G4_LevelManager.GetInstance().StartCoroutine(G4_LevelManager.GetInstance().PopListBall(BombBallAround(), Time.deltaTime * 20));
                                PopBall(G4_Constants.BallPopPoint);
                            }
                        }
                    }
                }
                
            }
            
        }
        if(state is BallState.Moving && destinations.Count>0)
        {
            if (Vector2.Distance(TF.position, destinations[0]) <= 0.001f)
            {
                TF.position = destinations[0];
                destinations.RemoveAt(0);

            }
            TF.position = Vector2.MoveTowards(TF.position,destinations[0],speed*Time.deltaTime);
        }

    }
    public bool SetPos(int x, int y)
    {
        StopMoving();
        row = x; col = y;
        if (G4_LevelManager.GetInstance().Row <= x)
        {
            G4_LevelManager.GetInstance().AddLine();
        }
        if (G4_LevelManager.GetInstance().Balls[x][y]!=null)
        {
            return false;
        }
        G4_LevelManager.GetInstance().Balls[x][y] = this;
        Vector3 offset1 = Vector3.zero;
        if (row % 2 == 1)
        {
            offset1 += Vector3.right *BallRadius;
        }
        TF.position = new Vector3(col, -row/Mathf.Tan(30*Mathf.Deg2Rad)/2, 0)*BallRadius*2 + offset1 + G4_Ball.offset;
        if (color is BallColor.Red)
        {
            G4_LevelManager.GetInstance().Map[x][y] = 1;
        }
        if (color is BallColor.Green)
        {
            G4_LevelManager.GetInstance().Map[x][y] = 2;
        }
        if (color is BallColor.Blue)
        {
            G4_LevelManager.GetInstance().Map[x][y] = 3;
        }
        TF.SetParent(G4_GameController.GetInstance().BallZone);
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
        rb.gravityScale = 0;
        effect.Stop();
    }
    public void AddForce(Vector2 force)
    {
        rb.AddForce(force);
    }
    public void Follow(List<Vector2> destinations,Collider2D collider)
    {
        this.destinations = destinations;
        state = BallState.Moving;
        circleCollider.radius = G4_Shooter.lazeWidth;
        targetCollider = collider;
        effect.Play();
        if(elementEffect != null)
        {
            elementEffect.SetActive(false);
        }
        //circleCollider.enabled = false;
    }
    public void PopBall(int point)
    {
        StopMoving();
        rb.gravityScale = 0;
        if (row < G4_LevelManager.GetInstance().Row && col >= 0)
        {
            G4_LevelManager.GetInstance().Balls[row][col] = null;
            G4_LevelManager.GetInstance().Map[row][col] = 0;
        }
        gameObject.layer = 0;
        G4_LevelManager.numBallColor[color] -= 1;
        ParticleSystem effect = G4_BallPool.GetInstance().GetFromPool(G4_Constants.BallonPopEffect, TF.position).GetComponent<ParticleSystem>();
        effect.Play();
        G4_BallPool.GetInstance().ReturnToPool(G4_Constants.BallonPopEffect, effect.gameObject,0.5f);
        G4_BallPool.GetInstance().ReturnToPool(tagPool,gameObject);

        G4_CombatText cbt = G4_BallPool.GetInstance().GetFromPool(G4_Constants.CombatText_Point, TF.position).GetComponent<G4_CombatText>();
        cbt.SetText(point.ToString());
        G4_BallPool.GetInstance().ReturnToPool(G4_Constants.CombatText_Point, cbt.gameObject,0.5f);
        //G4_Constants.Score += point;

        //G4_UIManager.GetInstance().GetUI<G4_UIGamePlay>().SetScoreText(G4_Constants.Score);
        G4_GameController.GetInstance().UpScore(point);

    }
  
   
    IEnumerator IEReadyCheckAround(float time)
    {
        yield return new WaitForSeconds(time);
        G4_LevelManager.GetInstance().BFS_BallAround(row, col);
        
    }
    
    public void FallBall()
    {
        if (row >= 0 && col >= 0)
        {
            G4_LevelManager.GetInstance().Balls[row][col] = null;
            G4_LevelManager.GetInstance().Map[row][col] = 0;
        }

        //Cho roi tu nhien hon
        rb.AddForce(new Vector2(Random.Range(-50, 50),Random.Range(0,50)));
        state = BallState.Fall;
        rb.gravityScale = 2f;
    }
    
    void CheckPos(G4_Ball ball)
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
                else
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
                if (ball.Row % 2 == 0 || ball.Col >= G4_LevelManager.GetInstance().Col - 1)
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
                if (ball.Col < G4_LevelManager.GetInstance().Col - 1)// Con trong ben phai
                {
                    if(!SetPos(ball.Row, ball.Col + 1))
                    {
                        if (angle <= 90)
                        {
                            if (ball.Row % 2 == 0 || ball.Col >= G4_LevelManager.GetInstance().Col - 1)
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
                            if (ball.Row % 2 == 0 || ball.Col < G4_LevelManager.GetInstance().Col - 1)
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
                if (ball.Row % 2 == 0 || ball.Col < G4_LevelManager.GetInstance().Col - 1)
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
        if (Color is not BallColor.Bomb)
        {
            StartCoroutine(IEReadyCheckAround(Time.deltaTime * 20));
        }
        else
        {
            G4_LevelManager.GetInstance().StartCoroutine(G4_LevelManager.GetInstance().PopListBall(BombBallAround(), Time.deltaTime * 20));
            PopBall(G4_Constants.BallPopPoint);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Wall"))
        {
            if (Color is not BallColor.FireBall)
            {
                rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);
            }
            else
            {
                Fire();
                G4_LevelManager.GetInstance().StartCoroutine(G4_LevelManager.GetInstance().IECheckAll(1f));
            }
        }
        if (collision.CompareTag("Top"))
        {
            if (state is BallState.Moving)
            {
                if (Color is BallColor.FireBall)
                {
                    G4_LevelManager.GetInstance().StartCoroutine(G4_LevelManager.GetInstance().IECheckAll(1f));
                    Fire();
                }
                
            }
            
        }

        if (collision.CompareTag("Ball"))
        {
            
            if (Color is  BallColor.FireBall)
            {
                G4_Ball ball = collision.GetComponent<G4_Ball>();
                ball.Fire();
            }
            
            
        }
        if (collision.CompareTag("Bottom"))
        {
            if (state is BallState.Fall)
            {
                PopBall(G4_Constants.BallFallPoint);
            }
            if(state is BallState.Moving)
            {
                PopBall(0);
                G4_GameController.GetInstance().State = G4_GameState.Playing;
            }
        }

    }
    public void Fire()
    {
        if (Color is BallColor.Rabbit)
        {
            return;
        }
        GameObject fire = G4_BallPool.GetInstance().GetFromPool(G4_Constants.FireEffect, TF.position);
        G4_BallPool.GetInstance().ReturnToPool(G4_Constants.FireEffect, fire,0.5f);
        PopBall(G4_Constants.BallPopPoint);
        
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
        Vector3 target = TF.position + (Vector3)direction * G4_Ball.BallRadius / 4;
        while (Vector2.Distance(target,TF.position)>=0.001f)
        {
            //Debug.Log("Moving");
            TF.position = Vector3.Lerp(TF.position,target,0.5f);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        while (Vector2.Distance(oldPos, TF.position) >= 0.001f)
        {
            TF.position = Vector3.Lerp(TF.position, oldPos, 0.5f);
            yield return new WaitForSeconds(Time.deltaTime);
        }

        //TF.position += (Vector3)direction*Ball.BallRadius/4;
        //yield return new WaitForSeconds(Time.deltaTime*10);
        //Debug.Log((Vector3)direction * Ball.BallRadius / 4);
        //TF.position = oldPos;
    }
    public void BoundBallAround()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, BallRadius);

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Ball"))
            {
                G4_Ball ball = collider.GetComponent<G4_Ball>();
                if(ball ==this)
                {
                    continue;
                }
                //Debug.Log("Bound");
                Vector2 direct = ball.TF.position - TF.position;
                float max = Mathf.Max(Mathf.Abs(direct.x), Mathf.Abs(direct.y));
          
                ball.StartCoroutine(ball.Bound(direct / max));
            }
        }
    }
    public List<G4_Ball> BombBallAround()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, BallRadius*4);
        GameObject effect= G4_BallPool.GetInstance().GetFromPool(G4_Constants.ExplosionEffect, TF.position);
        G4_BallPool.GetInstance().ReturnToPool(G4_Constants.ExplosionEffect, effect,0.5f);
        List<G4_Ball> list = new List<G4_Ball>();
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Ball"))
            {
                G4_Ball ball = collider.GetComponent<G4_Ball>();
                if (ball == this || ball.Color is BallColor.Rabbit)
                {
                    continue;
                }
              
                list.Add(ball);
            }
        }
        return list;
    }

    public bool IsEqualColor(G4_Ball ball)
    {
        if(ball.Color is BallColor.Rabbit || Color is BallColor.Rabbit)
        {
            return false;
        }
        return ball.Color is BallColor.FullColor || this.Color == BallColor.FullColor || ball.Color == this.Color;

    }


}
