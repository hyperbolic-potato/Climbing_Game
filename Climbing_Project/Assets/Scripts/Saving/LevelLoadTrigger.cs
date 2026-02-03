using UnityEngine;

public class LevelLoadTrigger : MonoBehaviour
{
    public LevelLoader ll;
    public int nextLevel;

    private void Start()
    {
        ll = GameObject.FindWithTag("LevelLoader").GetComponent<LevelLoader>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("AAAAAAA");
        if (collision.CompareTag("Player"))
        {
            ll.currentSaveData.levelIndex = nextLevel;
            ll.SaveToFile(ll.currentSaveData);

            ll.LoadLevel(nextLevel);
        }
    }
}
