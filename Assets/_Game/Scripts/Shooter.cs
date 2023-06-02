using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : GOSingleton<Shooter>
{
    [SerializeField] private int numBall;
    [SerializeField] private List<Ball> balls;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private List<Transform> shootPoints;
    public Ball currentBall;
    public Ball prevBall;
    public Ball nextBall;
    private float speedShoot=300f;
    private Transform tf;
    private Vector3 offset = new Vector3 (1.5f,-0.5f,0);
    private float ballRadius=0.25f;
    private int countBreakLine=20;
    private bool isSwitching = false;
    private float speedSwitching = 3f;
    public LineRenderer lineRenderer;


    public Transform TF { 
        get { 
            if (tf == null)
            {
                tf = transform;
            }
            return tf; 
        } 
    }

    public int NumBall { get => numBall; set => numBall = value; }
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }
    private void Update()
    {
        if(GameController.GetInstance().State != GameState.Playing ||isSwitching)
        {
            lineRenderer.enabled = false;
            return;
        }
        if (currentBall == null)
        {
            GetBall();
        }
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
            Vector2 direct = touchPosition - (Vector2)shootPoints[0].position;
            if (direct.y >0.5f)
            {
                direct /= Mathf.Max(Mathf.Abs(direct.x), Mathf.Abs(direct.y));
                List<Vector2> destinations = DrawVector(shootPoints[0].position, direct);

                if (touch.phase == TouchPhase.Ended /*|| touch.phase == TouchPhase.Canceled*/)
                {
                    // Xử lý sự kiện thả tay ở đây
                    //Shoot(direct);
                    ShootUpdate(destinations);
                }
            }
            else
            {
                lineRenderer.enabled = false;
            }
            
        }
        else
        {
            lineRenderer.enabled = false;
        }
        //if (Input.GetMouseButtonDown(0))
        //{
        //    Touch touch = Input.GetTouch(0);
        //    Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
        //    Vector2 direct = touchPosition - TF.position;
        //    direct /= Mathf.Max(Mathf.Abs(direct.x), Mathf.Abs(direct.y));
        //    Shoot( direct);
            
        //}
    }
    Ball RandomBall()
    {
        
        List<int> indexs = new List<int> { 1,2,3};
        if (LevelManager.numBallColor[BallColor.Red] == 0)
        {
            indexs.Remove(1);
        }
        if (LevelManager.numBallColor[BallColor.Green] == 0)
        {
            indexs.Remove(2);
        }
        if (LevelManager.numBallColor[BallColor.Blue] == 0)
        {
            indexs.Remove(3);
        }
        if(indexs.Count == 0)
        {
            indexs = new List<int> { 1, 2, 3 };
        }
        int index = indexs[Random.Range(0, indexs.Count)];
        Ball ball;
        if (index == 1)
        {
            ball = BallPool.GetInstance().GetFromPool(Constants.RedBall).GetComponent<Ball>();
        }
        else if (index == 2)
        {
            ball = BallPool.GetInstance().GetFromPool(Constants.GreenBall).GetComponent<Ball>();
        }
        else
        {
            ball = BallPool.GetInstance().GetFromPool(Constants.BlueBall).GetComponent<Ball>();
        }
        ball.OnInit();
        return ball;
    }
    public void OnInit(int numball)
    {
        currentBall = null;
        nextBall = null;
        this.numBall = numball;
       
        nextBall = RandomBall();
        nextBall.TF.position = shootPoints[1].position;
        GetBall();
        
    }
    public void GetBallDirectly()
    {
        GetBall(0);
    }
    public void GetBall(float angle=120)
    {
        if(numBall == 0) { return; }
        numBall -= 1;
        UIManager.GetInstance().GetUI<UIGamePlay>().SetNumBall(numBall);
        prevBall = currentBall;
        if (LevelManager.numBallColor[nextBall.Color] == 0)
        {
            nextBall.OnInit();
            BallPool.GetInstance().ReturnToPool(nextBall.tagPool, nextBall.gameObject);
            nextBall = RandomBall();
        }
        currentBall = nextBall;
        //currentBall.TF.position = shootPoints[0].position;
        StartCoroutine(IERotateBall(currentBall,angle, shootPoints[0].position));
        if (numBall <= 0)
        {
            return;
        }
        int index = Random.Range(0, balls.Count);
        nextBall = RandomBall();
        nextBall.TF.position = shootPoints[1].position;
       
    }
    public void Shoot(Vector2 direction)
    {
        if(prevBall!=null && prevBall.State is BallState.Moving)
        {
            return;
        }
        if (currentBall.State is not BallState.Moving)
        {
            currentBall.AddForce(direction * speedShoot);
            Invoke(nameof(GetBall), 2f);
        }
    }
    public void ShootUpdate(List<Vector2> destinations)
    {
       
        if (prevBall != null && prevBall.State is BallState.Moving)
        {
            return;
        }
        if (currentBall.State is not BallState.Moving)
        {
            LevelManager.numBallColor[currentBall.Color] += 1;
            currentBall.Follow(destinations);
            currentBall = null;
            GameController.GetInstance().ChangeState(GameState.Waiting);
            //Invoke(nameof(GetBall), 2f);
        }
    }
    List<Vector2> DrawVector(Vector3 startPos,Vector3 direction)
    {
        List<Vector2> positions = new List<Vector2>();
        lineRenderer.enabled = true;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPos);
        //lineRenderer.SetPosition(1, transform.position + direction * 100);
        int i;
        for( i=0;i<countBreakLine;i++) {
            RaycastHit2D hit = Physics2D.CircleCast(startPos, 0.02f, direction, 100f, wallLayer);
            if (hit.collider != null && i<countBreakLine-1)
            {       
                Vector2 hitpoint;
                if (hit.collider.CompareTag("Wall"))
                {
                    float angle = Vector2.Angle(direction, Vector2.up);
                    float distanceHeight = ballRadius / Mathf.Tan(angle * Mathf.Deg2Rad);
                    if (direction.x > 0)
                    {
                        hitpoint = hit.point + new Vector2(-ballRadius, -distanceHeight);
                    }
                    else
                    {
                        hitpoint = hit.point + new Vector2(ballRadius, -distanceHeight);
                    }
                    startPos = hitpoint;
                    lineRenderer.SetPosition(lineRenderer.positionCount - 1, hitpoint);
                    // Thực hiện thay đổi hướng tại điểm va chạm (hit.point)
                    // Ví dụ: direction = Vector3.Reflect(direction, hit.normal);
                    direction = new Vector3(-direction.x, direction.y, direction.z);
                    lineRenderer.positionCount = lineRenderer.positionCount + 1;
                    positions.Add(hitpoint);
                }
                else if (hit.collider.CompareTag("TopWall"))
                {
                    float angle = Vector2.Angle(direction, Vector2.right);
                    float distanceWidth = ballRadius / Mathf.Tan(angle * Mathf.Deg2Rad);
                    if (direction.x > 0)
                    {
                        hitpoint = hit.point + new Vector2(-distanceWidth, -ballRadius);
                    }
                    else
                    {
                        hitpoint = hit.point + new Vector2(distanceWidth, -ballRadius);
                    }
                    startPos = hitpoint;
                    lineRenderer.SetPosition(lineRenderer.positionCount - 1, hitpoint);
                    // Thực hiện thay đổi hướng tại điểm va chạm (hit.point)
                    // Ví dụ: direction = Vector3.Reflect(direction, hit.normal);
                    direction = new Vector3(direction.x, -direction.y, direction.z);
                    lineRenderer.positionCount = lineRenderer.positionCount + 1;
                    positions.Add(hitpoint);
                }
                else if (hit.collider.CompareTag("Ball")|| hit.collider.CompareTag("Top"))
                {
                    positions.Add(hit.point);
                    lineRenderer.SetPosition(lineRenderer.positionCount - 1, hit.point);
                    break;
                }


            }
            else
            {
                lineRenderer.SetPosition(lineRenderer.positionCount-1, transform.position + direction * 100);
                positions.Add(transform.position + direction * 100);
                break;
            }
        }
        return positions;
    }
   
    public void ClearBall()
    {
        StartCoroutine(IEClearBall(Time.deltaTime*20));
    }

    public IEnumerator IEClearBall(float time)
    {
        int num = numBall;
        for (int i = 0; i < num; i++)
        {
            GetBall(0);
            //GetBallDirectly();
            currentBall.ThrowUp();
            yield return new WaitForSeconds(time);
        }
        
    }

    public void SwitchBall()
    {
        if(isSwitching||nextBall ==null||currentBall==null)
        {
            return;
        }
        Ball temp=currentBall;
        currentBall = nextBall;
        StartCoroutine(IERotateBall(currentBall, 120, shootPoints[0].position));
        //currentBall.TF.RotateAround(TF.position,Vector3.forward,120);
        //currentBall.TF.position = shootPoints[0].position;
        nextBall = temp;
        //nextBall.TF.position= shootPoints[1].position;
        //nextBall.TF.RotateAround(TF.position, Vector3.forward, 120);
        StartCoroutine(IERotateBall(nextBall, 240, shootPoints[1].position));
    }

    IEnumerator IERotateBall(Ball b,float angle,Vector3 target)
    {
        isSwitching = true;
        float temp = 0;
        while(temp<angle)
        {
            float nextAngle = angle * Time.deltaTime * speedSwitching;
            b.TF.RotateAround(TF.position, Vector3.forward, nextAngle);
            temp += nextAngle;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        b.TF.position = target;
        isSwitching=false;
    }
    
}
