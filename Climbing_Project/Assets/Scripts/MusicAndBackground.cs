using UnityEngine;

public class MusicAndBackground : MonoBehaviour
{
    public int level;
    public float level1Threshold;
    public float level2Threshold;
    Transform player;

    public SpriteRenderer sr;
    public Sprite[] levelBackgrounds;

    float levelProgressRatio;
    Transform victory;

    public float textureHeight;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        sr = GetComponent<SpriteRenderer>();
        victory = GameObject.FindGameObjectWithTag("Victory").transform;
    }

    private void Update()
    {
        if (player.position.y > level2Threshold)      level = 2;
        else if (player.position.y > level1Threshold) level = 1;
        else                                                    level = 0;
    
        sr.sprite = levelBackgrounds[level];

        switch (level)
        {
            case 0:
                levelProgressRatio = player.position.y / level1Threshold;
                break;
            case 1:
                levelProgressRatio = (player.position.y - level1Threshold)/(level2Threshold - level1Threshold);
                break;
            case 2:
                levelProgressRatio = (player.position.y - level2Threshold) / (victory.position.y - level2Threshold);
                break;

        }

        transform.localPosition = new Vector2(0, -(textureHeight * levelProgressRatio - textureHeight / 2));
    }   
}
