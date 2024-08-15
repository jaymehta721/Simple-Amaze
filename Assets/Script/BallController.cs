using UnityEngine;

public class BallController : MonoBehaviour
{
    private enum HitDirection
    {
        None,
        Forward,
        Back,
        Left,
        Right
    }

    [SerializeField] private Color ballTileColor;

    private string collisionSide;
    private Rigidbody rigidbody;
    private TouchController touchController;

    public int MoveCounter { get; set; }

    public int ColoredWallCounter { get; set; }

    private void Start()
    {
        touchController = GetComponent<TouchController>();
        rigidbody = GetComponent<Rigidbody>();
        StopBall();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("floor"))
        {
            return;
        }

        if (collision.gameObject.GetComponent<Renderer>().material.color != ballTileColor)
        {
            ColoredWallCounter++;
            collision.gameObject.GetComponent<Renderer>().material.color = ballTileColor;
        }
    }

    private void StopBall()
    {
        touchController.IsBallMoving = false;
        rigidbody.velocity = Vector3.zero;
        transform.Translate(Vector3.zero);
        rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
    }

    private void MoveBall(Vector3 direction, float deltaTime)
    {
        Ray ballRay = new Ray(transform.position, direction);
        Debug.DrawRay(transform.position, direction, Color.black);
        if (Physics.Raycast(ballRay, out var ballHit, 1))
        {
            if (ballHit.collider == null)
            {
                return;
            }

            MoveCounter++;
            StopBall();
            collisionSide = GetCollisionDirection(ballHit);
        }
        else
        {
            touchController.IsBallMoving = true;
            transform.Translate(direction * deltaTime * 50);
            transform.localScale = new Vector3(1, 1, 0.7f);
        }
    }

    private string GetCollisionDirection(RaycastHit hit)
    {
        HitDirection hitDirection = HitDirection.None;
        Vector3 normal = hit.normal;
        normal = hit.transform.TransformDirection(normal);

        if (normal == hit.transform.forward)
        {
            hitDirection = HitDirection.Forward;
        }
        else if (normal == -hit.transform.forward)
        {
            hitDirection = HitDirection.Back;
        }
        else if (normal == hit.transform.right)
        {
            hitDirection = HitDirection.Right;
        }
        else if (normal == -hit.transform.right)
        {
            hitDirection = HitDirection.Left;
        }

        return hitDirection.ToString();
    }

    private void FixedUpdate()
    {
        if (touchController.Direction == Vector2.up && collisionSide != HitDirection.Back.ToString())
        {
            MoveBall(Vector3.forward, Time.deltaTime);
        }
        else if (touchController.Direction == Vector2.down && collisionSide !=  HitDirection.Forward.ToString())
        {
            MoveBall(-Vector3.forward, Time.deltaTime);
        }
        else if (touchController.Direction == Vector2.right && collisionSide != HitDirection.Left.ToString())
        {
            MoveBall(Vector3.right, Time.deltaTime);
        }
        else if (touchController.Direction == Vector2.left && collisionSide !=  HitDirection.Right.ToString())
        {
            MoveBall(-Vector3.right, Time.deltaTime);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
}