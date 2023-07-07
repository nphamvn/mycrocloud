using System.Runtime.Serialization.Formatters.Binary;
/// <summary>
///     IEnumerable File Helper Class.
/// </summary>
public static class IEnumerableFileHelper
{
    /// <summary>
    ///     Save a IEnumerable to a file.
    /// </summary>
    [Obsolete("Obsolete")]
    public static void SaveToFileIEnum<T>(this IEnumerable<T> obj, string path, bool append = false)
    {
        var type = obj.GetType();
        if (!type.IsSerializable)
            throw new Exception("The object is not serializable.");
        using (Stream stream = File.Open(path, append ? FileMode.Append : FileMode.Create))
        {
            var bf = new BinaryFormatter();
            bf.Serialize(stream, obj);
        }
    }
    /// <summary>
    ///     Load a IEnumerable from a file.
    /// </summary>
    [Obsolete("Obsolete")]
    public static IEnumerable<T> LoadFromFileIEnum<T>(this IEnumerable<T> dump, string path)
    {
        var type = dump.GetType();
        if (!type.IsSerializable)
            throw new Exception("The object is not serializable.");
        using (Stream stream = File.Open(path, FileMode.Open))
        {
            var bf = new BinaryFormatter();
            var obj = (IEnumerable<T>)bf.Deserialize(stream);
            return obj;
        }
    }
    /// <summary>
    ///     Save an object (Primitive, class, structure) to a file.
    /// </summary>
    [Obsolete("Obsolete")]
    public static void SaveToFileObj<T>(this T obj, string path, bool append = false)
    {
        var type = obj.GetType();
        if (!type.IsSerializable)
            throw new Exception("The object is not serializable.");
        using (Stream stream = File.Open(path, append ? FileMode.Append : FileMode.Create))
        {
            var bf = new BinaryFormatter();
            bf.Serialize(stream, obj);
        }
    }
    /// <summary>
    ///     Load an object (Primitive, class, structure) from a file.
    /// </summary>
    [Obsolete("Obsolete")]
    public static T LoadFromFileObj<T>(this T dump, string path)
    {
        var type = dump.GetType();
        if (!type.IsSerializable)
            throw new Exception("The object is not serializable.");
        using (Stream stream = File.Open(path, FileMode.Open))
        {
            var bf = new BinaryFormatter();
            var obj = (T)bf.Deserialize(stream);
            return obj;
        }
    }
}