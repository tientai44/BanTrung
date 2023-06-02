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
    public Ball secondBall;
    public Ball thirdBall;
    private Transform tf;
    private Vector3 offset = new Vector3 (1.5f,-0.5f,0);
    private float ballRadius=0.25f;
    private int countBreakLine=20;
    private bool isSwitching = false;
    private float speedSwitching = 6f;
    private bool modeTripleBallActive=false;
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
        if(GameController.GetInstance().State != GameState.Playing || isSwitching)
        {
            lineRenderer.enabled = false;
            return;
        }
        if (currentBall == null)
        {
            StartCoroutine(GetBall());
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
        secondBall = null;
        thirdBall = null;
        this.numBall = numball;
        modeTripleBallActive = false;
        secondBall = RandomBall();
        secondBall.TF.position = shootPoints[1].position;
        StartCoroutine(GetBall());
        UIManager.GetInstance().GetUI<UIGamePlay>().SetNumBall(numBall);
    }
    public void GetBallDirectly()
    {
        StartCoroutine(GetBall(0));
    }
    public IEnumerator GetBall(float angle=120)
    {
        if(numBall == 0) { yield break; }
        if (!modeTripleBallActive)
        {
            //numBall -= 1;
            //UIManager.GetInstance().GetUI<UIGamePlay>().SetNumBall(numBall);
            prevBall = currentBall;
            if (secondBall !=null &&LevelManager.numBallColor[secondBall.Color] == 0)
            {
                secondBall.OnInit();
                BallPool.GetInstance().ReturnToPool(secondBall.tagPool, secondBall.gameObject);
                secondBall = RandomBall();
                secondBall.TF.position = shootPoints[1].position;
            }
            currentBall = secondBall;
            //currentBall.TF.position = shootPoints[0].position;
            yield return StartCoroutine(IERotateBall(currentBall, angle, shootPoints[0].position));
            if (numBall <= 1)
            {
                secondBall = null;
                yield break;
            }
            secondBall = RandomBall();
            secondBall.TF.position = shootPoints[1].position;
        }
        else//Che do 3 vien
        {
            //numBall -= 1;
            //UIManager.GetInstance().GetUI<UIGamePlay>().SetNumBall(numBall);
            prevBall = currentBall;
            if (secondBall != null && LevelManager.numBallColor[secondBall.Color] == 0)
            {
                secondBall.OnInit();
                BallPool.GetInstance().ReturnToPool(secondBall.tagPool, secondBall.gameObject);
                secondBall = RandomBall();
                secondBall.TF.position = shootPoints[1].position;
            }
            if (thirdBall!=null && LevelManager.numBallColor[thirdBall.Color] == 0)
            {
                thirdBall.OnInit();
                BallPool.GetInstance().ReturnToPool(thirdBall.tagPool, thirdBall.gameObject);
                thirdBall = RandomBall();
                thirdBall.TF.position = shootPoints[2].position;
            }
            currentBall = secondBall;
            
            StartCoroutine(IERotateBall(currentBall, angle, shootPoints[0].position));
            if(numBall <= 1)
            {
                secondBall = null;
                yield break;
            }
            secondBall = thirdBall;
            yield return StartCoroutine(IERotateBall(secondBall, angle, shootPoints[1].position));
            if (numBall <= 2)
            {
                thirdBall = null;
                yield break;
            }
            thirdBall = RandomBall();
            thirdBall.TF.position = shootPoints[2].position;
        }
       
    }
    //public void Shoot(Vector2 direction)
    //{
    //    if(prevBall!=null && prevBall.State is BallState.Moving)
    //    {
    //        return;
    //    }
    //    if (currentBall.State is not BallState.Moving)
    //    {
    //        currentBall.AddForce(direction * speedShoot);
    //        Invoke(nameof(GetBall), 2f);
    //    }
    //}
    public void ShootUpdate(List<Vector2> destinations)
    {
       
        if (prevBall != null && prevBall.State is BallState.Moving)
        {
            return;
        }
        if (currentBall.State is not BallState.Moving)//Shoot
        {
            numBall -= 1;
            UIManager.GetInstance().GetUI<UIGamePlay>().SetNumBall(numBall);
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
            yield return StartCoroutine(GetBall(0));
            //GetBallDirectly();
            currentBall.ThrowUp();
            numBall -= 1;
            UIManager.GetInstance().GetUI<UIGamePlay>().SetNumBall(numBall);
            yield return new WaitForSeconds(time);
        }
        
    }

    public void SwitchBall()
    {
        if(isSwitching||secondBall ==null||currentBall==null)
        {
            return;
        }
        if (!modeTripleBallActive)
        {
            Ball temp = currentBall;
            currentBall = secondBall;
            StartCoroutine(IERotateBall(currentBall, 120, shootPoints[0].position));
            secondBall = temp;
            StartCoroutine(IERotateBall(secondBall, 240, shootPoints[1].position));
        }
        else
        {
            Ball temp = currentBall;
            currentBall = secondBall;
            StartCoroutine(IERotateBall(currentBall, 120, shootPoints[0].position));
            secondBall = thirdBall;
            StartCoroutine(IERotateBall(secondBall, 120, shootPoints[1].position));
            thirdBall = temp;
            StartCoroutine(IERotateBall(thirdBall, 120, shootPoints[2].position));
        }
    }

    IEnumerator IERotateBall(Ball b,float angle,Vector3 target)
    {
        isSwitching = true;
        float temp = 0;
        while(temp<angle)
        {
            float nextAngle = angle * Time.deltaTime * speedSwitching;
            temp += nextAngle;
            if (temp > angle)
            {
                nextAngle -= temp - angle;
            }
            b.TF.RotateAround(TF.position, Vector3.forward, nextAngle);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        b.TF.position = target;
        isSwitching=false;
    }
    
    public void EnableTripleMode()
    {
        if (modeTripleBallActive)
        {
            return;
        }
        modeTripleBallActive = true;
        thirdBall = RandomBall();
        thirdBall.TF.position = shootPoints[2].position;
    }
}
