using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;

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
                myProcess.OutputDataReceived += (sender, args) => {
                    //Console.WriteLine(args.Data); lock (consoleBuffer) { consoleBuffer.Enqueue(args.Data); }
                };
                myProcess.Start();
                myProcess.BeginOutputReadLine();
                return myProcess;
            }
            catch { Console.WriteLine("Failed to start process"); }
            return null;
        }

        public static void StartConversion(string gamePath, string outDir,string toolPath)
        {

          /*  string[] Items =
            {
                "\\Models\\Environment\\Bushes",
                "\\Models\\Environment\\Grass",
                "\\Models\\Environment\\Trees",
                "\\Models\\Environment\\Props"
            };*/

            string[] Items =
           {
                "\\Models",
                "\\BackgroundCube"
            };

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

            int currentfiles = 0;
            int  maxfiles = fileList.Length;
            int current_working = 0;
            Parallel.For(0, maxfiles, new ParallelOptions { MaxDegreeOfParallelism = 50 }, i =>
            {
                var currentFilePath = fileList[i];
                var DirName = Path.GetDirectoryName(currentFilePath);
                var relDir = DirName.Replace(gamePath, "");
                var destDir = outDir + relDir;
                var convFileName = Path.GetFileName(currentFilePath);
                Directory.CreateDirectory(destDir);
                currentfiles++;


                var cmdArgs = string.Format("-ft TIF -if LINEAR -sRGB -y -o \"{0}\" \"{1}\"", destDir, currentFilePath);
                if (convFileName.Contains("terminal_panel_cm") || convFileName.Contains("Emissive") || convFileName.Contains("LCD") || convFileName.Contains("ProgramingBlock_cm"))
                {
                       cmdArgs = string.Format("-ft TIF -f B8G8R8X8_UNORM -y -o \"{0}\" \"{1}\"", destDir, currentFilePath);
                    //Console.WriteLine(convFileName);
                }
                var newProcess = StartProcess(toolPath + "\\texconv.exe", cmdArgs);
                if (newProcess != null)
                {
                    newProcess.WaitForExit();
                    Interlocked.Increment(ref current_working);
                    int progress = (int)((float)current_working / (float)maxfiles*100.0f);
                    Console.WriteLine(current_working + "/" + maxfiles + "| " + progress);
                   // consoleBuffer.Enqueue("Converting: " + convFileName + " (" + current_working + "/" + maxfiles + ")");
                }
            }
            );
        }

            static void Main(string[] args)
        {
            Console.WriteLine("Startup");
            Console.ReadKey();
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            string gamePath = @"C:\Program Files (x86)\Steam\steamapps\common\SpaceEngineers\Content\Textures";
            string outDir = @"F:\Modding\VRage\VRageToolbox\TextureTest";
            string toolPath = @"E:\Steam\SteamApps\common\SpaceEngineersModSDK\Tools\TexturePacking\Tools";
            StartConversion(gamePath, outDir, toolPath);


            stopWatch.Stop();
            Console.WriteLine("Finished");
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
             ts.Hours, ts.Minutes, ts.Seconds,
             ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);
            Console.ReadKey();
        }


    }
}
