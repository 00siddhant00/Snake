using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class Movement : MonoBehaviour
{
    //[SerializeField] private Food food;
    private Vector2 direction;
    private Camera cam;
    private Vector3 pos;
    [SerializeField] private GameObject snakeBodyPrefab;
    [SerializeField] private TextMeshProUGUI scoreText;
    int score;

    [SerializeField] private float leftPivot;
    [SerializeField] private float rightPivot;
    [SerializeField] private float topPivot;
    [SerializeField] private float botPivot;

    private List<Transform> bodyParts = new List<Transform>();
    private int lengthOfSnake = 0;
    private Vector2 previousPos;
    private bool hasBody;
    public List<SpriteRenderer> spriteList;
    public Color baseColor = Color.red;
    public float startSaturation = 1.0f;
    public float endSaturation = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        score= 0;
        pos = transform.position;
        previousPos = pos;
        cam = Camera.main;
        direction = Vector2.right;
        CalculateCameraPivot();
    }

    void CalculateCameraPivot()
    {
        botPivot = cam.ScreenToWorldPoint(new Vector3(0, 0, 0)).y;
        topPivot = cam.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y;
        leftPivot = cam.ScreenToWorldPoint(new Vector3(0, 0, 0)).x;
        rightPivot = cam.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = score.ToString();
        pos = transform.position;

        if (Input.GetKeyDown(KeyCode.W))
        {
            if (hasBody && direction == Vector2.down) return;
            direction = Vector2.up;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            if (hasBody && direction == Vector2.up) return;
            direction = Vector2.down;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            if (hasBody && direction == Vector2.right) return;
            direction = Vector2.left;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            if (hasBody && direction == Vector2.left) return;
            direction = Vector2.right;
        }
    }

    void FixedUpdate()
    {
        previousPos = transform.position;
        if (pos.x <= leftPivot || pos.x >= rightPivot)
        {
            this.transform.position = new Vector3(
                pos.x >= rightPivot ? leftPivot + 1 : rightPivot -1,
                transform.position.y,
                0
            );
        }
        else if (pos.y <= botPivot || pos.y >= topPivot)
        {
            this.transform.position = new Vector3(
                transform.position.x,
                pos.y >= topPivot ? botPivot + 1 : topPivot -1,
                0
            );
        }
        else
        {
            transform.position = new Vector3(
                Mathf.RoundToInt(transform.position.x + direction.x),
                Mathf.RoundToInt(transform.position.y + direction.y),
                0
            );
        }
        if (bodyParts.Count < lengthOfSnake)
        {
            GameObject newPart = Instantiate(snakeBodyPrefab, previousPos, Quaternion.identity);
            bodyParts.Insert(0, newPart.transform);
            
        }
        else if (bodyParts.Count > 0)
        {
            hasBody = true;
            bodyParts.Last().position = previousPos;
            bodyParts.Insert(0, bodyParts.Last());
            bodyParts.RemoveAt(bodyParts.Count - 1);
            Saturate(bodyParts);
        }

    }

    public List<Vector3> GetSnakePos()
    {
        List<Vector3> result = new List<Vector3> ();
        for (int i = 0; i < bodyParts.Count; i++)
        {
            result.Add(bodyParts[i].transform.position);
        }
        return result;
    }

    void Saturate(List<Transform> tail)
    {
        spriteList.Clear();
        for (int i = 0; i < tail.Count; i++)
        {
            spriteList.Add(tail[i].gameObject.GetComponent<SpriteRenderer>());
        }
        for (int i = 0; i < spriteList.Count; i++)
        {
            float saturation = Mathf.Lerp(startSaturation, endSaturation, (float)i / (spriteList.Count - 1));
            float hue, saturation2, value;
            Color.RGBToHSV(baseColor, out hue, out saturation2, out value);
            Color color = Color.HSVToRGB(hue, saturation, value);
            spriteList[i].color = color;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Food")
        {
            collision.gameObject.GetComponent<Food>().RandomFoodSpawn(leftPivot, rightPivot, topPivot, botPivot, gameObject);
            Destroy(collision.gameObject);
            lengthOfSnake++;
            score++;
        }
        if(collision.gameObject.tag == "Body")
        {
            Time.timeScale = 0.0f;
            print("GameOver");
        }
    }

}
