using System;
using System.IO;
using System.Windows.Forms;
using DotNetEnv;
using GradebookApp.Forms;

namespace GradebookApp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            var envPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".env");
            if (File.Exists(envPath))
            {
                Env.Load(envPath);
            }
            else
            {
                var projectRoot = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)?.Parent?.Parent?.FullName;
                if (projectRoot != null)
                {
                    var rootEnvPath = Path.Combine(projectRoot, ".env");
                    if (File.Exists(rootEnvPath))
                    {
                        Env.Load(rootEnvPath);
                    }
                }
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
          
            Application.Run(new MainForm());
        }
    }
}
