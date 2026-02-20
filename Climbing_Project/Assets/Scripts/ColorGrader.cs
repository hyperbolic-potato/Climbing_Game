using UnityEngine;
using UnityEngine.Tilemaps;

public class ColorGrader : MonoBehaviour
{
    public Gradient colorGradient;
    public Color environmentColor;

    [HeaderAttribute("base colors:")]

    public Color baseForegroundColor;
    public Color baseBackgroundColor;

    public float foregroundWeight;
    public float backgroundWeight;

    Transform playerPos;
    Transform topPos;

    Tilemap foreground;
    Tilemap background;

    private void Start()
    {
        playerPos = GameObject.FindWithTag("Player").transform;
        topPos = GameObject.FindWithTag("Victory").transform;
        foreground = transform.GetChild(1).GetComponent<Tilemap>();
        background = transform.GetChild(0).GetComponent<Tilemap>();
    }

    private void Update()
    {
        float progress = topPos.position.y - playerPos.position.y;
        float progressRatio = 1 - (progress / topPos.position.y);

        environmentColor = colorGradient.Evaluate(Mathf.Clamp(progressRatio, 0 , 1));

        Camera.main.backgroundColor = environmentColor;

        background.color = AverageColors(environmentColor, baseBackgroundColor, backgroundWeight);
        foreground.color = AverageColors(environmentColor, baseForegroundColor, foregroundWeight);


        
    }
    //a very ad-hoc weighted average function that returns a color along the line intersecting two other colors
    Color AverageColors(Color a, Color b, float weight)
    {
        Color avg;

        Color weightedA;
        Color weightedB;

        weightedA = a * weight;
        weightedB = (b * (1 - weight));

        avg = (weightedA + weightedB);

        return avg;
    }
}
