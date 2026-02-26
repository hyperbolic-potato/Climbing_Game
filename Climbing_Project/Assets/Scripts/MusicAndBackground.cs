using System.Collections;
using UnityEngine;

public class MusicAndBackground : MonoBehaviour
{
    public int level;
    public int deltaLevel = -1;
    public float level1Threshold;
    public float level2Threshold;
    Transform player;

    public SpriteRenderer sr;
    public Sprite[] levelBackgrounds;

    float levelProgressRatio;
    Transform victory;

    public float textureHeight;

    public AudioSource ASource;

    public AudioClip[] songs;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        sr = GetComponent<SpriteRenderer>();
        victory = GameObject.FindGameObjectWithTag("Victory").transform;
        ASource = GetComponent<AudioSource>();

        if (player.position.y > level2Threshold) level = 2;
        else if (player.position.y > level1Threshold) level = 1;
        else level = 0;

        deltaLevel = level;

        if (level == 0)
        {
            StartCoroutine(SongTransition());
        }
    }

    private void Update()
    {
        //determining level progress
        if (player.position.y > level2Threshold)      level = 2;
        else if (player.position.y > level1Threshold) level = 1;
        else                                                    level = 0;

        //for background texture
    
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

        //for music

        if(level > deltaLevel)
        {
            StartCoroutine(SongTransition());
        }

        deltaLevel = level;
    }  
    
    IEnumerator SongTransition()
    {
        while (ASource.volume  > 0)
        {
            ASource.volume -= Time.deltaTime;
            yield return new WaitForEndOfFrame();

        }
        ASource.clip = songs[level];
        ASource.Play();
        while (ASource.volume < 1)
        {
            ASource.volume += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

    }
}
