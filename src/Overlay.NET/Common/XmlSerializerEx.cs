using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Overlay.NET.Common {
    /// <summary>
    ///     Class to serialize/deserialize XML objects to and from strings, and to and from files.
    /// </summary>
    /// <seealso cref="ISerializer" />
    public class XmlSerializerEx : ISerializer {
        /// <summary>
        ///     Serializes the specified object and returns the document.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>
        ///     XML document of the serialized object.
        /// </returns>
        public string ExportToString<T>(T obj) {
            var serializer = new XmlSerializer(typeof(T));
            using (var stringWriter = new StringWriter()) {
                serializer.Serialize(stringWriter, obj);
                return stringWriter.ToString();
            }
        }

        /// <summary>
        ///     Deserializes the document to the specified object.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize.</typeparam>
        /// <param name="serializedObj">The string representing the serialized object.</param>
        /// <returns>
        ///     The deserialized object.
        /// </returns>
        public T ImportFromString<T>(string serializedObj) {
            var serializer = new XmlSerializer(typeof(T));
            using (var stringWriter = new StringReader(serializedObj)) {
                return (T) serializer.Deserialize(stringWriter);
            }
        }

        /// <summary>
        ///     Serializes the specified object and writes the document to the specified path.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="file">The path where the file is saved.</param>
        /// <param name="encoding">The encoding to generate.</param>
        public void ExportToFile<T>(T obj, string file, Encoding encoding) {
            using (var fileWriter = new StreamWriter(file, false, encoding)) {
                fileWriter.Write(ExportToString(obj));
            }
        }

        /// <summary>
        ///     Deserializes the specified file into an object.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize.</typeparam>
        /// <param name="path">The path where the object is read.</param>
        /// <param name="encoding">The character encoding to use. </param>
        /// <returns>The deserialized object.</returns>
        public T ImportFromFile<T>(string path, Encoding encoding) {
            using (var fileReader = new StreamReader(path, encoding)) {
                return ImportFromString<T>(fileReader.ReadToEnd());
            }
        }
    }
}