using NetMQ.Sockets;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ProcessB
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                DummyData dummyData = new DummyData(10);

                var sizeVal=dummyData.GetMemorySize();
                //var serializedData = SerializerDeserializerExtensions.Serializer(dummyData);
                //Console.WriteLine("Using Text.json " + Environment.NewLine + serializedData);
                Stopwatch stopwatch = Stopwatch.StartNew();
                stopwatch.Start();
                var serializedData = JsonConvert.SerializeObject(dummyData);
                //Console.WriteLine("Using Newtonsoft.json " + Environment.NewLine + serializedData);
                System.Diagnostics.Debug.WriteLine("serialization took " + stopwatch.ElapsedMilliseconds);

                var len = serializedData.Length * sizeof(Char);
                var leninKB = System.Text.ASCIIEncoding.ASCII.GetByteCount (serializedData);
                ////StartServer();
                //Task.Delay(1000).Wait();


                //Client
                var client = new NamedPipeClientStream("PipesOfPiece");
                client.Connect();
                StreamReader reader = new StreamReader(client);
                StreamWriter writer = new StreamWriter(client);
                int i = 0;
                while (true)
                {
                    string input = serializedData;
                    if (String.IsNullOrEmpty(input)) break;
                    Stopwatch s1 = new Stopwatch();

                    s1.Start();
                    writer.WriteLine(input);
                    s1.Stop();
                    System.Diagnostics.Debug.WriteLine("Writing took " + s1.ElapsedMilliseconds);

                    writer.Flush();
                    string yesOrNo = reader.ReadLine();
                    while (yesOrNo == "No")
                    {
                        yesOrNo = reader.ReadLine();
                    }
                    //string input = Console.ReadLine();
                    //if (String.IsNullOrEmpty(input)) break;
                    //writer.WriteLine(input);
                    //writer.Flush();
                    //var reply=reader.ReadLine();
                    Console.WriteLine("Counter " + i);

                    //}
                    ////using (MemoryMappedFile mmf = MemoryMappedFile.CreateNew("testmap",100))
                    ////{

                    ////    //Mutex mutex = Mutex.OpenExisting("testmapmutex");
                    ////    //mutex.WaitOne();

                    ////    using (MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor())
                    ////    {
                    ////        //BinaryWriter writer = new BinaryWriter(stream);
                    ////        accessor.Write(54, serializedData.Length);
                    ////        //accessor.Write(54+4,serializedData,0,serializedData.Length);
                    ////    }
                    ////    //mutex.ReleaseMutex();
                    ////}

                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Memory-mapped file does not exist. Run Process A first.");
            }
            catch (Exception ex)
            {

            }
        }

        //static void StartServer()
        //{
        //    Task.Factory.StartNew(() =>
        //    {
        //        var server = new NamedPipeServerStream("PipesOfPiece");
        //        server.WaitForConnection();
        //        StreamReader reader = new StreamReader(server);
        //        StreamWriter writer = new StreamWriter(server);
        //        while (true)
        //        {
        //            var line = reader.ReadLine();
        //            writer.WriteLine(String.Join("", line));
        //            writer.Flush();
        //        }
        //    });
        //}

    }

    public class DummyData
    {
        public int _movementCount = 0;
        public DummyData(int movementCount = 1000)
        {
            Movements = new List<Movement>();
            Name = nameof(DummyData);
            ID = Guid.NewGuid();
            DepartureTime = DateTime.Now;
            Origin = nameof(Origin);
            Destination = nameof(Destination);
            _movementCount = movementCount;
            FillMovements();
        }
        public DummyData()
        { }
        public string Name { get; set; }
        public Guid ID { get; set; }
        public DateTime DepartureTime { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public ICollection<Movement> Movements { get; set; }
        public void FillMovements()
        {
            for (int i = 0; i < _movementCount; i++)
            {
                Movement m = new Movement();
                Movements.Add(m);
            }
        }
    }
    public class Movement
    {
        public const float V = 1.0f; 
        public Movement()
        {
            TrackId = Guid.NewGuid();
            TrackName = nameof(TrackName);
            Time = DateTime.Now;
            Offset = V;
            Speed = (decimal)V;
            MovementType = nameof(MovementType);
            Columnn1 = nameof(Columnn1);
            Columnn2 = nameof(Columnn2);
            Columnn3 = nameof(Columnn3);
            Columnn4 = nameof(Columnn4);
            Columnn5 = nameof(Columnn5);
            Columnn6 = nameof(Columnn6);
            Columnn7 = nameof(Columnn7);
            Columnn8 = nameof(Columnn8);
            Columnn9 = nameof(Columnn9);
            Columnn10 = nameof(Columnn10);
        }
        public Guid TrackId { get; set; }
        public string TrackName { get; set; }
        public DateTime Time { get; set; }
        public float Offset { get; set; }
        public decimal Speed { get; set; }
        public string MovementType { get; set; }
        public string Columnn1 { get; set; }
        public string Columnn2 { get; set; }
        public string Columnn3 { get; set; }
        public string Columnn4 { get; set; }
        public string Columnn5 { get; set; }
        public string Columnn6 { get; set; }
        public string Columnn7 { get; set; }
        public string Columnn8 { get; set; }
        public string Columnn9 { get; set; }
        public string Columnn10 { get; set; }
    }
    public static class SerializerDeserializerExtensions
    {
        public static string Serializer(this object _object)
        {
            var bytes = System.Text.Json.JsonSerializer.Serialize(_object);
            return bytes;
        }

        public static T Deserializer<T>(this byte[] _byteArray)
        {
            T ReturnValue = System.Text.Json.JsonSerializer.Deserialize<T>(_byteArray);
            return ReturnValue;
        }
    }


    public static class ObjectMemorySizeCalculator
    {
        static int OBJECT_SIZE = IntPtr.Size == 8 ? 24 : 12;
        static int POINTER_SIZE = IntPtr.Size;
        public static long GetMemorySize(this object obj)
        {
            long memorySize = 0;
            Type objType = obj.GetType();

            if (objType.IsValueType)
            {
                memorySize = Marshal.SizeOf(obj);
            }
            else if (objType.Equals(typeof(string)))
            {
                var str = (string)obj;
                memorySize = str.Length * 2 + 6 + OBJECT_SIZE;
            }
            else if (objType.IsArray)
            {
                var arr = (Array)obj;
                var elementType = objType.GetElementType();
                if (elementType.IsValueType)
                {
                    long elementSize = Marshal.SizeOf(elementType);
                    long elementCount = arr.LongLength;
                    memorySize += elementSize * elementCount;
                }
                else
                {
                    foreach (var element in arr)
                    {
                        memorySize += element != null ? element.GetMemorySize() + POINTER_SIZE : POINTER_SIZE;
                    }
                }

                memorySize += OBJECT_SIZE;
            }
            else if (obj is IEnumerable)
            {
                var enumerable = (IEnumerable)obj;
                foreach (var item in enumerable)
                {
                    var itemType = item.GetType();
                    memorySize += item != null ? item.GetMemorySize() : 0;
                    if (itemType.IsClass)
                        memorySize += POINTER_SIZE;
                }
                memorySize += OBJECT_SIZE;
            }
            else if (objType.IsClass)
            {
                var properties = objType.GetProperties();
                foreach (var property in properties)
                {
                    var valueObject = property.GetValue(obj);
                    memorySize += valueObject != null ? valueObject.GetMemorySize() : 0;
                    if (property.GetType().IsClass)
                        memorySize += POINTER_SIZE;
                }
                memorySize += OBJECT_SIZE;
            }

            return memorySize;
        }
    }
}
