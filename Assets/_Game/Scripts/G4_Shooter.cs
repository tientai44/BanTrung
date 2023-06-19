using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShooterMode
{
    Normal,FullColor,Bomb,FireBall
}
public class G4_Shooter : G4_GOSingleton<G4_Shooter>
{
    public static float lazeWidth = 0.05f;
    private static G4_Ball tempBall;
    private Collider2D targetCollider;
    [SerializeField] private int numBall;
    [SerializeField] private List<G4_Ball> balls;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private List<Transform> shootPoints;
    public G4_Ball currentBall;
    public G4_Ball prevBall;
    public G4_Ball secondBall;
    public G4_Ball thirdBall;
    private Transform tf;
    private Vector3 offset = new Vector3 (1.5f,-0.5f,0);
    private float ballRadius=0.25f;
    private int countBreakLine=20;
    private bool isSwitching = false;
    private float speedSwitching = 6f;
    private bool modeTripleBallActive=false;
    private ShooterMode mode = ShooterMode.Normal;
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
    public ShooterMode Mode { get => mode; set => mode = value; }

    private void Awake()
    {
        //lineRenderer = GetComponent<LineRenderer>();
    }
    private void Update()
    {
        if(G4_GameController.GetInstance().State != G4_GameState.Playing || isSwitching)
        {
            lineRenderer.enabled = false;
            return;
        }
        if (currentBall == null)
        {
            if(mode is not ShooterMode.Normal)
            {
                UnEnableAnyMode();
            }
            else StartCoroutine(GetBall());
        }
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
            Vector2 direct = touchPosition - (Vector2)shootPoints[0].position;
            if (direct.y >0.5f &&direct.y<5.8f)
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
    G4_Ball RandomBall()
    {
        
        List<int> indexs = new List<int> { 1,2,3};
        if (G4_LevelManager.numBallColor[BallColor.Red] == 0)
        {
            indexs.Remove(1);
        }
        if (G4_LevelManager.numBallColor[BallColor.Green] == 0)
        {
            indexs.Remove(2);
        }
        if (G4_LevelManager.numBallColor[BallColor.Blue] == 0)
        {
            indexs.Remove(3);
        }
        if(indexs.Count == 0)
        {
            indexs = new List<int> { 1, 2, 3 };
        }
        int index = indexs[Random.Range(0, indexs.Count)];
        G4_Ball ball;
        if (index == 1)
        {
            ball = G4_BallPool.GetInstance().GetFromPool(G4_Constants.RedBall).GetComponent<G4_Ball>();
        }
        else if (index == 2)
        {
            ball = G4_BallPool.GetInstance().GetFromPool(G4_Constants.GreenBall).GetComponent<G4_Ball>();
        }
        else
        {
            ball = G4_BallPool.GetInstance().GetFromPool(G4_Constants.BlueBall).GetComponent<G4_Ball>();
        }
        ball.OnInit();
        return ball;
    }
    public void OnInit(int numball)
    {
        
        StopAllCoroutines();
        currentBall = null;
        secondBall = null;
        thirdBall = null;
        this.numBall = numball;
        modeTripleBallActive = false;
        Mode = ShooterMode.Normal;
        secondBall = RandomBall();
        secondBall.TF.position = shootPoints[1].position;
        StartCoroutine(GetBall());
        G4_UIManager.GetInstance().GetUI<G4_UIGamePlay>().SetNumBall(numBall);
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
            if (secondBall !=null &&G4_LevelManager.numBallColor[secondBall.Color] == 0)
            {
                secondBall.OnInit();
                G4_BallPool.GetInstance().ReturnToPool(secondBall.tagPool, secondBall.gameObject);
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
            if (secondBall != null && G4_LevelManager.numBallColor[secondBall.Color] == 0)
            {
                secondBall.OnInit();
                G4_BallPool.GetInstance().ReturnToPool(secondBall.tagPool, secondBall.gameObject);
                secondBall = RandomBall();
                secondBall.TF.position = shootPoints[1].position;
            }
            if (thirdBall!=null && G4_LevelManager.numBallColor[thirdBall.Color] == 0)
            {
                thirdBall.OnInit();
                G4_BallPool.GetInstance().ReturnToPool(thirdBall.tagPool, thirdBall.gameObject);
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
            if (Mode is ShooterMode.Normal) {
                numBall -= 1;
            }
            G4_UIManager.GetInstance().GetUI<G4_UIGamePlay>().SetNumBall(numBall);
            G4_LevelManager.numBallColor[currentBall.Color] += 1;
            currentBall.Follow(destinations, targetCollider);
            currentBall = null;
            G4_GameController.GetInstance().ChangeState(G4_GameState.Waiting);
            //Invoke(nameof(GetBall), 2f);
        }
    }
    List<Vector2> DrawVector(Vector3 startPos,Vector3 direction)
    {
        List<Vector2> positions = new List<Vector2>();
        lineRenderer.enabled = true;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPos);
        targetCollider = null;
        //lineRenderer.SetPosition(1, transform.position + direction * 100);
        int i;
        for( i=0;i<countBreakLine;i++) {
            RaycastHit2D hit = Physics2D.CircleCast(startPos, lazeWidth, direction, 100f, wallLayer);
            if (hit.collider != null && i<countBreakLine-1)
            {
                if (mode is not ShooterMode.FireBall)
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
                    else if (hit.collider.CompareTag("Ball") || hit.collider.CompareTag("Top"))
                    {
                        positions.Add(hit.point);
                        lineRenderer.SetPosition(lineRenderer.positionCount - 1, hit.point);
                        targetCollider = hit.collider;
                        break;
                    }
                }
                else
                {
                    lineRenderer.SetPosition(lineRenderer.positionCount - 1, transform.position + direction * 100);
                    positions.Add(transform.position + direction * 100);
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
            G4_UIManager.GetInstance().GetUI<G4_UIGamePlay>().SetNumBall(numBall);
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
            G4_Ball temp = currentBall;
            currentBall = secondBall;
            StartCoroutine(IERotateBall(currentBall, 120, shootPoints[0].position));
            secondBall = temp;
            StartCoroutine(IERotateBall(secondBall, 240, shootPoints[1].position));
        }
        else
        {
            G4_Ball temp = currentBall;
            currentBall = secondBall;
            StartCoroutine(IERotateBall(currentBall, 120, shootPoints[0].position));
            secondBall = thirdBall;
            StartCoroutine(IERotateBall(secondBall, 120, shootPoints[1].position));
            thirdBall = temp;
            StartCoroutine(IERotateBall(thirdBall, 120, shootPoints[2].position));
        }
    }

    IEnumerator IERotateBall(G4_Ball b,float angle,Vector3 target)
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
 
    public void EnableMode(ShooterMode shooterMode)
    {
        if (mode is not ShooterMode.Normal || currentBall == null)
        {
            return;
        }
        mode = shooterMode;
        tempBall = currentBall;
        tempBall.gameObject.SetActive(false);
        secondBall.gameObject.SetActive(false);
        if (thirdBall != null)
        {
            thirdBall.gameObject.SetActive(false);
        }
        if(mode is ShooterMode.FullColor)
            currentBall = G4_BallPool.GetInstance().GetFromPool(G4_Constants.FullColorBall, shootPoints[0].position).GetComponent<G4_Ball>();
        if (mode is ShooterMode.Bomb)
            currentBall = G4_BallPool.GetInstance().GetFromPool(G4_Constants.Bomb, shootPoints[0].position).GetComponent<G4_Ball>();
        if (mode is ShooterMode.FireBall)
            currentBall = G4_BallPool.GetInstance().GetFromPool(G4_Constants.FireBall, shootPoints[0].position).GetComponent<G4_Ball>();
        currentBall.OnInit();
    }
    public void UnEnableAnyMode()
    {
        if(mode is ShooterMode.Normal)
        {
            return;
        }
        Debug.Log("unable");
        mode = ShooterMode.Normal;
        currentBall = tempBall; 
        currentBall.gameObject.SetActive(true);
        secondBall.gameObject.SetActive(true);
        if (thirdBall != null)
        {
            thirdBall.gameObject.SetActive(true);
        }
    }
    
}
