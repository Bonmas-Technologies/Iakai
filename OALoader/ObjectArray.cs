using System;
using System.IO;
using System.Text;
using System.Xml.Linq;

// todo: Добавить описание хранимых данных и используемые аттачменты в gl

namespace ObjectArray
{
    public static class ObjectArrayLoader
    {
        public const string Header   = "OAff";

        public static ObjectArrayContainer LoadObject(string path)
        {
            BinaryReader dataReader = new BinaryReader(File.OpenRead(path));
            
            CheckHeader(dataReader);

            ObjectArrayContainer container = new ObjectArrayContainer();

            int indicesLength = dataReader.ReadInt32();
            int elementsLength = dataReader.ReadInt32();

            container.indices = new uint[indicesLength];
            container.elements = new float[elementsLength];

            container.indices = GetArray(dataReader, sizeof(uint), indicesLength, container.indices) as uint[];
            container.elements = GetArray(dataReader, sizeof(float), elementsLength, container.elements) as float[];

            dataReader.Close();
            return container;
        }

        private static void CheckHeader(BinaryReader dataReader)
        {
            var header = new string(dataReader.ReadChars(4));

            if (header != Header)
                throw new ArgumentException("File isn't an ObjectArray type");
        }

        private static Array GetArray(BinaryReader reader, int dataSize,  int indicesLength, Array outputArray)
        {
            byte[] buffer = reader.ReadBytes(indicesLength * dataSize);

            Buffer.BlockCopy(buffer, 0, outputArray, 0, buffer.Length);
            
            return outputArray;
        }

        public static void WriteAsObject(string path, float[] elements, uint[] indices)
        {
            BinaryWriter dataWriter = new BinaryWriter(File.OpenWrite(path));
            
            dataWriter.Write(Header.ToCharArray());
            dataWriter.Write(indices.Length);
            dataWriter.Write(elements.Length);

            WriteArray(sizeof(uint), indices, dataWriter);
            WriteArray(sizeof(float), elements, dataWriter);

            dataWriter.Close();
        }

        private static void WriteArray(int dataSize, Array data, BinaryWriter writer)
        {
            byte[] buffer = new byte[data.Length * dataSize];
            Buffer.BlockCopy(data, 0, buffer, 0, buffer.Length);
            writer.Write(buffer);
        }
    }

    public struct ObjectArrayContainer
    {
        public float[] elements;
        public uint[]  indices;
    }
}
