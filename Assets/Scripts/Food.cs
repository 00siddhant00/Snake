using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public GameObject food;

    public void RandomFoodSpawn(float leftPivot,float rightPivot, float topPivot, float botPivot, GameObject player)
    {
        var Food = Instantiate(food, new Vector3(Mathf.RoundToInt(Random.Range(rightPivot - 2, leftPivot + 2)), Mathf.RoundToInt(Random.Range(topPivot - 2, botPivot + 2)), 0), Quaternion.identity);

        for (int i = 0; i < player.GetComponent<Movement>().GetSnakePos().Count; i++)
        {
            if(Food.transform.position == player.GetComponent<Movement>().GetSnakePos()[i])
            {
                RandomFoodSpawn(leftPivot, rightPivot, topPivot, botPivot, player);
                Destroy(gameObject);
            }
        }
    }
}
