using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SavePlayer (PlayerScript player)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(Path("player"), FileMode.Create);

        PlayerData data = new PlayerData(player);

        formatter.Serialize(stream, data);
        stream.Close();
    }
    public static PlayerData LoadPlayer()
    {
        if (File.Exists(Path("player")))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream steam = new FileStream(Path("player"), FileMode.Open);

            PlayerData data = formatter.Deserialize(steam) as PlayerData;
            steam.Close();
            return data;
        }
        else
        {
            Debug.LogError("Savefile not found " + Path("player"));
            return null;
        }
    }
    private static string Path(string filename)
    {
        string path = Application.persistentDataPath + "/" + filename + ".file";
        return path;
    }

}
