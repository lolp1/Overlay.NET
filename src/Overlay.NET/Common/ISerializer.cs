using System.Text;

namespace Overlay.NET.Common {
    /// <summary>
    ///     Interface that defines common ways to serialize/deserialize objects to and from strings.
    /// </summary>
    public interface ISerializer {
        /// <summary>
        ///     Serializes the specified object and returns the document.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>XML document of the serialized object.</returns>
        string ExportToString<T>(T obj);

        /// <summary>
        ///     Deserializes the document to the specified object.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize.</typeparam>
        /// <param name="serializedObj">The string representing the serialized object.</param>
        /// <returns>The deserialized object.</returns>
        T ImportFromString<T>(string serializedObj);

        /// <summary>
        ///     Serializes the specified object and writes the document to the specified path.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="file">The path where the file is saved.</param>
        /// <param name="encoding">The encoding to generate.</param>
        void ExportToFile<T>(T obj, string file, Encoding encoding);

        /// <summary>
        ///     Deserializes the specified file into an object.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize.</typeparam>
        /// <param name="path">The path where the object is read.</param>
        /// <param name="encoding">The character encoding to use. </param>
        /// <returns>The deserialized object.</returns>
        T ImportFromFile<T>(string path, Encoding encoding);
    }
}