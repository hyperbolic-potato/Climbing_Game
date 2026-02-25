using UnityEngine;

public class MusicAndBackground : MonoBehaviour
{
    public int level;
    public float level1Threshold;
    public float level2Threshold;
    Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (player.transform.position.y > level2Threshold)      level = 2;
        else if (player.transform.position.y > level1Threshold) level = 1;
        else                                                    level = 0;
    }   
}
