using System;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;

class FileReader
{
    private StreamReader _reader;
    public int Current { get; private set; }
    private bool _hasNext;

    public FileReader(string filePath)
    {
        _reader = new StreamReader(filePath);
        _hasNext = MoveNext();
    }

    public bool HasNext() => _hasNext;

    public bool MoveNext()
    {
        string line = _reader.ReadLine();
        if (line != null)
        {
            Current = int.Parse(line);
            return true;
        }
        else
        {
            _hasNext = false;
            return false;
        }
    }
    public void Close()
    {
        _reader.Close();
    }
}
class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        string filename = "inputFile.txt";
        string FolderForSolution = "Solution";

        CreateFile(filename, 10);
       

        Stopwatch sw = new Stopwatch();
        sw.Start();


        DivideIntoFiles(filename, 30, FolderForSolution);

      // ExternalSort(FolderForSolution);

      // MODSort(FolderForSolution);
      // MergeFiles(FolderForSolution, "output.txt");

        sw.Stop();

        Console.WriteLine(" {0} seconds", sw.Elapsed.TotalSeconds);
    }

    public static void CreateFile(string name, int size)
    {
        long fileSize = (1024 * 1024) * size;

        Random random = new Random();

        using (FileStream fs = new FileStream(name, FileMode.Create, FileAccess.Write))
        using (StreamWriter writer = new StreamWriter(fs))
        {
            long currentSize = 0;

            while (currentSize < fileSize)
            {
                int randomNumber = random.Next(10001);
                string numberString = randomNumber.ToString() + Environment.NewLine;

                int byteCount = System.Text.Encoding.UTF8.GetByteCount(numberString);

                if (currentSize + byteCount > fileSize)
                {
                    break;
                }

                writer.Write(numberString);
                currentSize += byteCount;
            }
        }

        Console.WriteLine("Input file name: " + name);
    }



    public static void DivideIntoFiles(string inputFile, int FilesCount, string outputDirectory)
    {
        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }

        StreamWriter[] writers = new StreamWriter[FilesCount];

        for (int i = 0; i < FilesCount; i++)
        {
            string fileName = Path.Combine(outputDirectory, $"B{i + 1}.txt");
            writers[i] = new StreamWriter(fileName);
        }

        using (StreamReader reader = new StreamReader(inputFile))
        {
            int index = 0;
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                writers[index++].WriteLine(line);

                if (index == FilesCount)
                    index = 0;
            }
        }

        for (int i = 0; i < FilesCount; i++)
        {
            writers[i].Close();
            writers[i].Dispose();
        }

    }

    static void ExternalSort(string FolderForSoutionPath)
    {
        string[] fileName = Directory.GetFiles(FolderForSoutionPath);

        int seria = 1;
        int mult = fileName.Length;

        while (fileName.Length != 1)
        {
            fileName = Directory.GetFiles(FolderForSoutionPath);
            List<StreamReader> readers = new List<StreamReader>();

            for (int i = 0; i < fileName.Length; i++)
            {
                readers.Add(new StreamReader(fileName[i]));
            }

            List<(int?, int)> values = new List<(int?, int)>();
            for (int i = 0; i < fileName.Length; i++)
            {
                values.Add((null, 0));
            }

            int index = 0;

            while (readers.Count != 0)
            {
                if (index == fileName.Length)
                    index = 0;

                StreamWriter writer = new StreamWriter(Path.Combine(FolderForSoutionPath, $"C{index + 1}.txt"), append: true);

                for (int i = 0; i < values.Count; i++)
                {
                    if (int.TryParse(readers[i].ReadLine(), out int value))
                    {
                        values[i] = (value, 1);
                    }
                    else
                    {
                        readers[i].Close();
                        readers.RemoveAt(i);
                        values.RemoveAt(i);
                    }
                }
                EndedSeria(readers, writer, values, seria);
                writer.Close();
                index++;

            }

            seria *= mult;


            for (int i = 0; i < fileName.Length; i++)
            {
                File.Delete(fileName[i]);
            }

            string[] filesToRename = Directory.GetFiles(FolderForSoutionPath, "C*");

            for (int i = 0; i < filesToRename.Length; i++)
            {
                string newFileName = "B" + Path.GetFileName(filesToRename[i]).Substring(1);
                string newFilePath = Path.Combine(FolderForSoutionPath, newFileName);

                File.Move(filesToRename[i], newFilePath);
            }
        }
    }

    public static bool EndedSeria(List<StreamReader> readers, StreamWriter writer, List<(int?, int)> values, int seria)
    {
        try
        {
            if (readers.Count == 0)
                return true;

            while (values.Count(x => x.Item1 == null) != values.Count)
            {
                int? min = values.Min(x => x.Item1);
                int index = values.FindIndex(x => x.Item1 == min);
                values[index] = (null, values[index].Item2 + 1);
                writer.WriteLine(min.ToString());

                if (values[index].Item2 <= seria)
                {
                    if (int.TryParse(readers[index].ReadLine(), out int value))
                    {
                        values[index] = (value, values[index].Item2);
                    }
                    else
                    {
                        readers[index].Close();
                        readers.RemoveAt(index);
                        values.RemoveAt(index);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        return false;
    }





    public static void MODSort(string FolderForSoutionPath)
    {
        string[] files = Directory.GetFiles(FolderForSoutionPath);

        foreach (string file in files)
        {
            List<int> values = new List<int>();
            using (StreamReader reader = new StreamReader(file))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    values.Add(int.Parse(line));
                }

                values.Sort();
            }
            WriteAllLines(file, values);
        }
    }

    public static void WriteAllLines(string file, List<int> values)
    {
        using (StreamWriter writer = new StreamWriter(file))
        {
            foreach (var value in values)
            {
                writer.WriteLine(value);
            }
        }
    }

    public static void MergeFiles(string FolderForSoutionPath, string outputFile)
    {
        string[] files = Directory.GetFiles(FolderForSoutionPath);
        using (StreamWriter writer = new StreamWriter(outputFile))
        {
            PriorityQueue<FileReader, int> PrioriryQ = new PriorityQueue<FileReader, int>();

            foreach (var file in files)
            {
                var reader = new FileReader(file);
                if (reader.HasNext())
                {
                    PrioriryQ.Enqueue(reader, reader.Current);
                }
            }

            while (PrioriryQ.Count > 0)
            {
                var minReader = PrioriryQ.Dequeue();
                writer.WriteLine(minReader.Current);

                if (minReader.MoveNext())
                {
                    PrioriryQ.Enqueue(minReader, minReader.Current);
                }
                else
                {
                    minReader.Close();
                }
            }
        }

        foreach (var file in files)
        {
            File.Delete(file);
        }
    }
}
