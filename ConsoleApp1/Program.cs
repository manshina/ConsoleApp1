//получить размер папки
static long GetDirSize(string path)
{
    long size = 0;
    string[] files = Directory.GetFiles(path);
    foreach (string file in files)
        size += (new FileInfo(file)).Length;
    string[] dirs = Directory.GetDirectories(path);
    foreach (string dir in dirs)
        size += GetDirSize(dir);
    return size;
}
//получить размер файла 
static long GetFileSize(string path)
{
    System.IO.FileInfo file = new System.IO.FileInfo(path);
    long size = file.Length;
    return size;
}
//рекурсивный обход фалов
List<string> GetRecursFiles(string start_path)
{
    List<string> ls = new List<string>();
       
    try
    {
        string[] folders = Directory.GetDirectories(start_path);
        foreach (string folder in folders)
        {           
            ls.Add(folder + "(" + GetDirSize(folder) + ")");
            //для папок выполняем новый обход
            ls.AddRange(GetRecursFiles(folder));
        }
        string[] files = Directory.GetFiles(start_path);
        foreach (string filename in files)
        {
            ls.Add(filename + "(" + GetFileSize(filename) + ")");
        }

        for (int i = 0; i < ls.Count; i++)
        {
            ls[i] = "-" + ls[i];
        }

    }
    catch (System.Exception e)
    {
        Console.WriteLine(e.Message);
    }
    return ls;
}
//преобразование в человекочитаемый формат делением на 1024
static string ToHumanread(string fname)
{
    //данные берутся из списка с файлами
    var x = fname.Split("(")[1].Remove(fname.Split("(")[1].Length - 1);
    var units = new[] { "B", "KB", "MB", "GB", "TB" };
    var index = 0;
    double size = Convert.ToDouble(x);
    while (size > 1024)
    {
        size /= 1024;
        index++;
    }
    var y = fname.Split("(")[0];
    string done = y + "(" + string.Format("{0:f2} {1}", size, units[index]) + ")";
    return done;
}
//список параметров командной строки
List<string> argslist = new List<string>();
List<string> correctargs = new List<string>()
{
    "-q",
    "--quite",
    "-p",
    "--path",
    "-o",
    "--output",
    "-h",
    "--humanred",
};
//добавление принятых аргументов и их проверка 
foreach (string arg in args)
{
    if (correctargs.Contains(arg))
    {
        argslist.Add(arg);
    }
    else
    {
        Console.WriteLine("Exepted argunets:");
        foreach(string item in correctargs)
        {
            Console.WriteLine(item);
        }
        return;
    }          
}
//папка вызова программы
string PathToFolder = AppContext.BaseDirectory;
//путь к папке для обхода
if (argslist.Contains("-p") || argslist.Contains("-path"))
{
    PathToFolder = argslist[(argslist.IndexOf("-p") + 1)];
}
//список файлов и папок
List<string> ls = GetRecursFiles(PathToFolder); 
try
{    
    ls.Insert(0, PathToFolder + "(" + GetDirSize(PathToFolder) + ")");
}
catch (System.Exception e)
{
    Console.WriteLine("wrong path");
    Console.WriteLine(e.Message);
}


//файл вывода в формате YYYY-MM-DD
string dt = DateTime.Now.ToString("yyyy-MM-dd");
string outname = $"sizes-{dt}.txt";
//файл вывода текстового фала в папке вызова
string outputpath = "";
//путь файла вывода 
if (argslist.Contains("-o") || argslist.Contains("-output"))
{
    outputpath = argslist[(argslist.IndexOf("-o") + 1)] +"\\";
}
StreamWriter? file = null;
try
{
    file = new StreamWriter(outputpath + outname);
    //два признака -q и -h
    if ((argslist.Contains("-q") || argslist.Contains("-quite")) && (argslist.Contains("-h") || argslist.Contains("-humanread")))
    {
        foreach (string fname in ls)
        {
            file.Write(ToHumanread(fname) + "\n");
        }
    }
    //не выводить лог в консоль
    else if (argslist.Contains("-q") || argslist.Contains("-quite"))
    {
        foreach (string fname in ls)
        {
            file.Write(fname + "\n");
        }
    }
    //файлы в человекочитаемой форме
    else if (argslist.Contains("-h") || argslist.Contains("-humanread"))
    {
        foreach (string fname in ls)
        {
            Console.WriteLine(ToHumanread(fname));
            file.Write(ToHumanread(fname) + "\n");
        }
    }
    //вывод в лог и запись в файл без параметров
    else
    {
        foreach (string fname in ls)
        {
            Console.WriteLine(fname);
            file.Write(fname + "\n");
        }
    }
}
catch (System.Exception e)
{
    Console.WriteLine("wrong output");
    Console.WriteLine(e.Message);
}
finally
{
    file?.Close();
}
Console.ReadLine();