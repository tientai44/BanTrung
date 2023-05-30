using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : GOSingleton<Shooter>
{
    [SerializeField] private int numBall;
    [SerializeField] private List<Ball> balls;
    [SerializeField] private LayerMask wallLayer;
    public Ball currentBall;
    public Ball prevBall;
    public Ball nextBall;
    private float speedShoot=300f;
    private Transform tf;
    private Vector3 offset = new Vector3 (1.5f,-0.5f,0);
    private float ballRadius=0.25f;
    private int countBreakLine=5;
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
  
    private void Update()
    {
       
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
            Vector2 direct = touchPosition - TF.position;
            direct /= Mathf.Max(Mathf.Abs(direct.x), Mathf.Abs(direct.y));
            DrawVector(TF.position,direct);
            if (touch.phase == TouchPhase.Ended /*|| touch.phase == TouchPhase.Canceled*/)
            {
                // Xử lý sự kiện thả tay ở đây
                Shoot(direct);
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
        int index = Random.Range(1, 4);
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
        return ball;
    }
    public void OnInit()
    {
        int index = Random.Range(0, balls.Count);
        nextBall = RandomBall();
        nextBall.TF.position = TF.position + offset;
        GetBall();
        lineRenderer = GetComponent<LineRenderer>();    
    }
    public void GetBall()
    {
       
        numBall -= 1;
        prevBall = currentBall;
        currentBall = nextBall;
        currentBall.TF.position = TF.position;
        if (numBall <= 0)
        {
            return;
        }
        int index = Random.Range(0, balls.Count);
        nextBall = RandomBall();
        nextBall.TF.position = TF.position + offset;

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
    void DrawVector(Vector3 startPos,Vector3 direction)
    {
        lineRenderer.enabled = true;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPos);
        //lineRenderer.SetPosition(1, transform.position + direction * 100);
        int i;
        for( i=0;i<countBreakLine;i++) {
            RaycastHit2D hit = Physics2D.Raycast(startPos, direction, 100f, wallLayer);
            if (hit.collider != null && i<countBreakLine-1)
            {
                float angle = Vector2.Angle(direction, Vector2.up);
                float distanceHeight =ballRadius/Mathf.Tan(angle*Mathf.Deg2Rad);
                Vector2 hitpoint;
                if (direction.x > 0)
                {
                    hitpoint = hit.point + new Vector2(-ballRadius,-distanceHeight);
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
            }
            else
            {
                lineRenderer.SetPosition(lineRenderer.positionCount-1, transform.position + direction * 100);
                break;
            }
        }
       
    }
    
}
