using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using M66FileIO;
namespace M66FileTest1
{

    class Program
    {
        static SerialPort port;
        private static CancellationTokenSource cancelationToken;
        private static FileOp fileOp;
        private static Configuration configuration;
        public static Task ReadKey { get; private set; }

        static void FileTest(FileOp Fileop)
        {
            string fileName = "testFile" + new Random().Next();
            Fileop.OpenFile(fileName).Wait();
            if (Fileop.prefix == null) throw new Exception("no prefix ");
            //FileOp.WriteFile("testing file write").Wait();
            //if (FileOp.resultOP == false) throw new Exception("can't write ");
            Fileop.AppendFile("appending file 23123123").Wait();
            if (Fileop.resultOP == false) throw new Exception("can't append ");
            Fileop.CloseFile().Wait();
            if (Fileop.resultOP == false) throw new Exception("can't close file ");
            Console.WriteLine("file writen");

            Fileop.ListFiles().Wait();

            foreach (var item in Fileop.files)
            {
                if (item.name == fileName) Fileop.GetContent(item.name).Wait();

            }
        }
        static void DeleteAll(FileOp Fileop)
        {
            Console.WriteLine("press any key to delete files");
            Console.ReadKey();
            foreach (var item in Fileop.files)
            {
                Fileop.DeleteFile(item.name).Wait();

            }

        }

        static void Main(string[] args)
        {
            try
            {
                // string portName = "COM9";
                string portFromArgs = "";
                int baudRateFromArgs = 0;
                if (args.Length > 0) portFromArgs = args[0];
                if (args.Length > 1) int.TryParse(args[1], out baudRateFromArgs);

                string filename = System.AppContext.BaseDirectory + @"\\Resources\\Configuration.json";
                var content = new TextReadWrite.SysReadWrite(filename);
                content.ReadText().Wait();

                if (content.Result)
                {
                    configuration = Newtonsoft.Json.JsonConvert.DeserializeObject<Configuration>(content.content);
                    Console.WriteLine("configuration load success");
                }
                else
                {
                    Console.WriteLine("error reading configuration");
                    Console.ReadKey();
                    return;
                }
                string portName = portFromArgs.Length > 0 ? portFromArgs : configuration.ComPort;
                int baudRate = baudRateFromArgs != 0 ? baudRateFromArgs : configuration.BaudRate;
                try
                {
                    port = new SerialPort(portName, baudRate);
                    port.Open();
                }
                catch (Exception)
                {

                    Console.WriteLine($"can't open port {portName} {baudRate}");
                    Console.ReadKey();
                    return;
                }
                fileOp = new FileOp(port);
                fileOp.debug = true;
                fileOp.ListFiles().Wait();
                //   fileOp.Close().Wait();
                cancelationToken = new CancellationTokenSource();
                ReadKey = new Task(ReadKeys, cancelationToken.Token);
                ReadKey.Start();
                if (fileOp.files.Count > 0)
                {
                    //fileOp.OpenFile(fileOp.files[0].name).Wait();
                    //fileOp.ReadFile(0, 10).Wait();
                }
                else
                {
                    Console.WriteLine("no files to read");

                }




                Console.ReadKey();

                while (true)
                {
                    Thread.Sleep(1000);
                }
            }
            catch (Exception Exp)
            {

                Console.WriteLine($"Error  :   {Exp.Message}");
                Console.ReadKey();
            }
        }

        private static void ReadKeys()
        {
            List<ConsoleKeyInfo> keys = new List<ConsoleKeyInfo>();
            int cursor = 0;
            //      ReadStopped = false;
            while (true)
            {
                ConsoleKeyInfo k = Console.ReadKey();
                //Console.Write(k.KeyChar);
                if ((int)k.Modifiers == ((int)ConsoleModifiers.Control | (int)ConsoleModifiers.Shift))
                {
                    switch (k.Key)
                    {
                        case ConsoleKey.P://load from pc
                            configuration.ConsoleList();
                            if (configuration.files.Count > 0)
                            {
                                Console.WriteLine($"choose file to read 1-{configuration.files.Count}");
                                fileOp.operationState = OperationStates.LOADFROMPC;
                            }
                            break;
                        case ConsoleKey.T://load to pc
                            fileOp.ConsoleList();
                            if (fileOp.files.Count > 0)
                            {
                                fileOp.operationState = OperationStates.LOADTOPC;
                                Console.WriteLine($"choose file to save 1-{fileOp.files.Count}  to pc folder {configuration.defaultdownloadfolder}");
                            }
                            break;
                        case ConsoleKey.R://read file

                            fileOp.ConsoleList();
                            if (fileOp.files.Count > 0)
                            {
                                fileOp.operationState = OperationStates.READ;
                                Console.WriteLine($"choose file to read 1-{fileOp.files.Count}");
                            }
                            break;
                        case ConsoleKey.L://list files
                            fileOp.operationState = OperationStates.LIST;
                            fileOp.ListFiles().Wait();
                            fileOp.ConsoleList();
                            break;
                        case ConsoleKey.D://delete file(s)

                            fileOp.ConsoleList();
                            if (fileOp.files.Count > 0)
                            {
                                fileOp.operationState = OperationStates.DELETE;
                                Console.WriteLine($"choose file to read 1-{fileOp.files.Count} or 'A' for deleting all");
                            }
                            break;
                        case ConsoleKey.A://append file
                            fileOp.operationState = OperationStates.APPEND;
                            fileOp.CloseFile().Wait();
                            fileOp.ConsoleList();
                            Console.WriteLine($"choose file to append 1-{fileOp.files.Count}");
                            break;
                        case ConsoleKey.W://create file
                            Console.WriteLine("please enter file name +enter");
                            fileOp.operationState = OperationStates.CREATE;
                            break;
                        case ConsoleKey.Z:
                            //end the file creating process
                            var chararray = keys.Select(x => x.KeyChar).ToArray();
                            if ((fileOp.operationState == OperationStates.CREATE || fileOp.operationState == OperationStates.APPEND) && fileOp.FilenameOK)
                            {
                                if (chararray.Length > 3)
                                {
                                    string filenameandcontent = new string(chararray);
                                    //var parts = filenameandcontent.Split(' ');
                                    //string file = parts[0];
                                    //string content = parts[1];

                                    fileOp.CloseFile().Wait();
                                    fileOp.OpenFile(fileOp.fileForWrite).Wait();
                                    if (fileOp.operationState == OperationStates.CREATE)
                                    {
                                        fileOp.WriteFile(filenameandcontent).Wait();
                                        if (fileOp.resultOP)
                                        {
                                            Console.WriteLine($"file: {fileOp.fileForWrite} content: {filenameandcontent} \n\r write OK");
                                        }
                                    }
                                    else
                                    {
                                        fileOp.AppendFile(filenameandcontent).Wait();
                                        if (fileOp.resultOP)
                                        {
                                            Console.WriteLine($"file: {fileOp.fileForWrite} content: {filenameandcontent} \n\r append OK");
                                        }
                                    }
                                    fileOp.FilenameOK = false;
                                    fileOp.fileForWrite = "";
                                    keys.Clear();
                                }

                            }
                            break;
                    }
                }
                else
                {
                    int num;
                    bool clearBuff = true;
                    switch (k.Key)
                    {
                        case ConsoleKey.Enter:
                            switch (fileOp.operationState)
                            {
                                case OperationStates.LOADFROMPC:
                                    num = CheckKey(keys);
                                    if (num > 0)
                                    {

                                        fileOp.CloseFile().Wait();
                                        num--;
                                        var list = configuration.files.ToList();

                                        Console.WriteLine($"Reading file from pc {list[num].Value}");
                                        var fileLoad = new TextReadWrite.SysReadWrite(list[num].Value);
                                        fileLoad.ReadText().Wait();
                                        if (fileLoad.Result)
                                        {
                                            string content = fileLoad.content;
                                            fileOp.OpenFile(list[num].Key).Wait();
                                            fileOp.WriteFile(fileLoad.content).Wait();

                                        }


                                    }
                                    break;

                                case OperationStates.LOADTOPC:
                                    num = CheckKey(keys);
                                    if (num > 0)
                                    {

                                        fileOp.CloseFile().Wait();
                                        num--;
                                        var list = configuration.files.ToList();

                                        Console.WriteLine($"Reading file from module {fileOp.files[num]}");
                                        fileOp.OpenFile(fileOp.files[num].name).Wait();
                                        if (fileOp.resultOP) fileOp.ReadFileContent(0, fileOp.files[num].lenghtInt).Wait();
                                        else
                                        {
                                            Console.WriteLine("can't open for reading");
                                        }
                                        if (fileOp.resultOP)
                                        {
                                            var Writer = new TextReadWrite.SysReadWrite(configuration.defaultdownloadfolder + fileOp.files[num].name);
                                            Writer.WriteText(fileOp.lastcontent).Wait();
                                        }



                                    }
                                    break;

                                case OperationStates.IDLE://TODO:
                                    //find a way to send the commands to module transparently
                                    break;
                                case OperationStates.LIST:

                                    break;
                                case OperationStates.READ:
                                    num = CheckKey(keys);
                                    if (num > 0)
                                    {

                                        fileOp.CloseFile().Wait();
                                        num--;
                                        Console.WriteLine($"Reading file {fileOp.files[num].name}");
                                        fileOp.OpenFile(fileOp.files[num].name).Wait();
                                        if (fileOp.resultOP) fileOp.ReadFile(0, fileOp.files[num].lenghtInt).Wait();
                                        else
                                        {
                                            Console.WriteLine("can't open for reading");
                                        }

                                    }
                                    break;
                                case OperationStates.CREATE:
                                    if (fileOp.FilenameOK)
                                    {
                                        clearBuff = false;//do not clear buffer if an enter key is pressed
                                        keys.Add(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));
                                        keys.Add(new ConsoleKeyInfo('\n', ConsoleKey.Enter, false, false, false));
                                        Console.WriteLine("");
                                    }
                                    else
                                    {
                                        var chararray = keys.Select(x => x.KeyChar).ToArray();
                                        fileOp.fileForWrite = new string(chararray).Trim();
                                        Console.WriteLine($"file name {fileOp.fileForWrite}");
                                        Console.WriteLine($"please enter content + Ctrl-Z");
                                        fileOp.FilenameOK = true;

                                    }
                                    break;
                                case OperationStates.APPEND:
                                    if (!fileOp.FilenameOK)
                                    {
                                        num = CheckKey(keys);
                                        if (num > 0)
                                        {

                                            fileOp.CloseFile().Wait();
                                            num--;
                                            Console.WriteLine($"ready to append file {fileOp.files[num].name}");
                                            Console.WriteLine($"please enter append content + Ctrl-Z");
                                            fileOp.fileForWrite = fileOp.files[num].name;
                                            fileOp.FilenameOK = true;
                                        }
                                    }
                                    else
                                    {
                                        clearBuff = false;//do not clear buffer if an enter key is pressed
                                        keys.Add(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));
                                        keys.Add(new ConsoleKeyInfo('\n', ConsoleKey.Enter, false, false, false));
                                        Console.WriteLine("");
                                    }

                                    break;
                                case OperationStates.DELETE:
                                    num = CheckKey(keys);
                                    if (num > 0)
                                    {
                                        fileOp.CloseFile().Wait();
                                        num--;
                                        Console.WriteLine($"Deleting file {fileOp.files[num].name}");
                                        fileOp.DeleteFile(fileOp.files[num].name).Wait();
                                        if (!fileOp.resultOP)
                                        {
                                            Console.WriteLine("can't  delete file");
                                        }
                                    }
                                    else if (CheckForSingleKey(keys, 'A'))
                                    {
                                        Console.WriteLine($"Deleting multiple files");
                                        fileOp.CloseFile().Wait();
                                        foreach (var file in fileOp.files)
                                        {
                                            Console.WriteLine($"Deleting file {file.name}");
                                            fileOp.DeleteFile(file.name).Wait();
                                            if (!fileOp.resultOP)
                                            {
                                                Console.WriteLine($"can't  delete file {file.name}");
                                            }
                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }
                            if (clearBuff) keys.Clear();
                            break;
                        case ConsoleKey.Escape:

                            fileOp.operationState = OperationStates.IDLE;
                            keys.Clear();
                            break;
                        default:
                            keys.Add(k);
                            break;
                    }


                }

            }
        }

        private static bool CheckForSingleKey(List<ConsoleKeyInfo> keys, char v)
        {
            if (keys.Count == 1 && keys[0].KeyChar == v)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static int CheckKey(List<ConsoleKeyInfo> keys)
        {
            if (keys.Count == 1 && int.TryParse(keys[0].KeyChar.ToString(), out int num) && num > 0)
            {
                return num;
            }
            else
            {
                return -1;
            }
        }
    }
}
