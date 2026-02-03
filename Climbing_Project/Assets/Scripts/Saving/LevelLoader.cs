using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public string savePath;
    public SaveData defaultSaveData;
    public SaveData currentSaveData;
    public static LevelLoader instance;


    private void Start()
    {
        savePath = Application.persistentDataPath + "/save.dat";
        
        //Ideally, we want the player to be able to save in scenes other than than the main menu
        DontDestroyOnLoad(gameObject);

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        //BUT THERE CAN ONLY BE ONE LEVELLOADER


    }

    public void New()
    {
        currentSaveData = defaultSaveData;
        SaveToFile(currentSaveData);
        LoadLevel(currentSaveData.levelIndex);
    }

    public void Load()
    {
        currentSaveData = LoadFromFile();
        LoadLevel(currentSaveData.levelIndex);
    }

    public void Menu()
    {
        LoadLevel(0);
    }

    public void Quit()//I'll give you one guess as to what this does
    {
        Application.Quit();
    }

    public void SaveToFile(SaveData save)
    {
        //verify/create file
        if (!File.Exists(savePath)) File.Create(savePath);

        //acquire reference to said file
        FileStream file = File.OpenWrite(savePath);

        //this thingy turns readable text into ones and zeros (i think)
        BinaryFormatter bf = new BinaryFormatter();

        //Still a little fuzzy on what serialization actually does but the result is saving the second thing into the first thing
        bf.Serialize(file, save);

        file.Close();
        //dont forget to close the door behind you
    }

    public SaveData LoadFromFile()
    {
        //dont load a file that doesnt exist you melon.
        if (!File.Exists(savePath)) return null;

        // just doing the save thing but backwards
        BinaryFormatter bf = new BinaryFormatter();
        SaveData data = (SaveData)bf.Deserialize(File.OpenRead(savePath));

        return data;
    }

    

    public void LoadLevel(int index)
    {
        SceneManager.LoadScene(index);
    }
}
