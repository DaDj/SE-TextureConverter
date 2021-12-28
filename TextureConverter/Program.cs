using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TextureConverter
{

    class Program
    {
        public static Process StartProcess(string executable, string commandline)
        {
            try
            {
                ProcessStartInfo sInfo = new ProcessStartInfo();
                var myProcess = new Process();
                myProcess.StartInfo = sInfo;
                sInfo.CreateNoWindow = true;
                sInfo.FileName = executable;
                sInfo.Arguments = commandline;
                myProcess.StartInfo.UseShellExecute = false;
                myProcess.StartInfo.RedirectStandardOutput = true;
                myProcess.OutputDataReceived += (sender, args) =>
                {
                    //Console.WriteLine(args.Data); lock (consoleBuffer) { consoleBuffer.Enqueue(args.Data); }
                };
                myProcess.Start();
                myProcess.BeginOutputReadLine();
                return myProcess;
            }
            catch { Console.WriteLine("Failed to start process"); }
            return null;
        }

        public static void StartConversion(string gamePath, string outDir, string toolPath,string[] Items)
        {
            var count = 0;
            foreach (string obj in Items)
            {
                if (obj != null)
                {
                    count++;
                }
            }

            string[] finalDirList = new string[count];

            var cdx = 0;
            foreach (string obj in Items)
            {
                if (obj != null)
                {
                    finalDirList[cdx] = obj;
                    cdx++;
                }
            }

            var fileCount = 0;

            for (int i = 0; i < finalDirList.Length; i++)
            {
                var iPth = gamePath + finalDirList[i];
                if (Directory.Exists(iPth))
                {
                    var w = Directory.GetFiles(iPth, "*", SearchOption.AllDirectories);
                    fileCount += w.Length;
                }
            }

            var fileList = new string[fileCount];
            var fIdx = 0;
            for (int i = 0; i < finalDirList.Length; i++)
            {
                var iPth = gamePath + finalDirList[i];

                if (Directory.Exists(iPth))
                {
                    var w = Directory.GetFiles(iPth, "*", SearchOption.AllDirectories);
                    for (int b = 0; b < w.Length; b++)
                    {
                        fileList[fIdx] = w[b];
                        fIdx++;
                    }
                }
            }

            List<String> tmpfileList = new List<String>();

            int i_files2 = 0;
            for (int i = 0; i < fileList.Length; i++)
            {
                var DirName = Path.GetDirectoryName(fileList[i]);

                if (!DirName.Contains("Skins"))
                {
                    tmpfileList.Add(fileList[i]);
                    i_files2++;
                }
            }
            String[] Final_filelist = tmpfileList.ToArray();

            int currentfiles = 0;
            int maxfiles = Final_filelist.Length;
            int current_working = 0;
        
            Console.WriteLine("Files Found:" + maxfiles);
            Console.WriteLine("Starting Conversion:");

            using (var progress = new ProgressBar())
            {
                    Parallel.For(0, maxfiles, new ParallelOptions { MaxDegreeOfParallelism = -1 }, i =>
                {
                    var currentFilePath = Final_filelist[i];
                    var DirName = Path.GetDirectoryName(currentFilePath);
                    var relDir = DirName.Replace(gamePath, "");
                    var destDir = outDir + relDir;
                    var convFileName = Path.GetFileName(currentFilePath);
                    Directory.CreateDirectory(destDir);
                    currentfiles++;


                    var cmdArgs = string.Format("-ft TIF -if LINEAR -y -o \"{0}\" \"{1}\"", destDir, currentFilePath);

                    //Hacky way of fixing these textures which had no RGB after conversion.
                    //It just uses antoher format and ditches the alpha completely in the process. not great really.
                    if (convFileName.Contains("terminal_panel_cm") || convFileName.Contains("Emissive") || convFileName.Contains("LCD") || convFileName.Contains("ProgramingBlock_cm"))
                    {
                        cmdArgs = string.Format("-ft TIF -f B8G8R8X8_UNORM -y -o \"{0}\" \"{1}\"", destDir, currentFilePath);
                        //Console.WriteLine(convFileName);
                    }
                    if (convFileName.Contains("_ng") )
                    {
                        cmdArgs = string.Format("-ft TIF -f R8G8B8A8_UNORM -srgbi -y -o \"{0}\" \"{1}\"", destDir, currentFilePath);
                        //Console.WriteLine(convFileName);
                    }

                    var newProcess = StartProcess(toolPath + "\\texconv.exe", cmdArgs);
                    if (newProcess != null)
                    {
                        newProcess.WaitForExit();
                        Interlocked.Increment(ref current_working);
                        double myprogress = (float)current_working /(float)maxfiles;
                        progress.Report(myprogress);
                        // Console.WriteLine(current_working + "/" + maxfiles + "| " + progress);
                        // consoleBuffer.Enqueue("Converting: " + convFileName + " (" + current_working + "/" + maxfiles + ")");
                    }
                }
                );
            }
        }

        static int Main(string[] args)
        {
            string strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string strWorkPath = System.IO.Path.GetDirectoryName(strExeFilePath);

            Console.WriteLine("#####Space Engineers Texture Converter#####");
            Console.WriteLine("###########################################");

            if (args.Length == 0 || args.Length < 3)
            {
                Console.WriteLine("No valid Input arguments given. press any key to close...");
             //   Console.ReadKey();
                return 4; 
            }

            string gamePath = args[0];
            string outDir = args[1];
            string[] TextureRoot = new string[args.Length - 2];
            Array.Copy(args, 2, TextureRoot,0, args.Length-2);
            string toolPath = strWorkPath;

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();


            Console.WriteLine("GamePath:    " + gamePath);
            Console.WriteLine("TextureRoot: " + "todo");
            Console.WriteLine("outDir:      " + outDir);
            Console.WriteLine("###########################################");
         
            // Console.ReadKey();
            StartConversion(gamePath, outDir, toolPath, TextureRoot);


            //just the runtime stuff
            stopWatch.Stop();
            Console.WriteLine("Finished");
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);
            //Console.ReadKey();

            //return 2 is sucess return, not using 0 for reasons :)
            return 2;
        }


    }
}
