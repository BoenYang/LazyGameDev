using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class ObjectSerilizer<T> {
    protected IFormatter iformatter;

    public ObjectSerilizer() {
        this.iformatter = new BinaryFormatter();
    }

    public T GetSerializedObject(string fileName) {
        if(File.Exists(fileName)) {
            Stream inStream = new FileStream(
                fileName,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read
            );
            T obj = (T)this.iformatter.Deserialize(inStream);
            inStream.Close();
            return obj;
        } else {
            throw new System.Exception("Could't find " + fileName);
        }
    }

    public void SaveSerializedObject(T obj, string fileName) {
        Stream outStream = new FileStream(
            fileName,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None
        );
        this.iformatter.Serialize(outStream, obj);
        outStream.Close();
    }
}