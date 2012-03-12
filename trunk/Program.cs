using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace RayTracerForm
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {


            // zjisti pritomnost DLL 
            string dllPath = Path.Combine(Application.StartupPath, "RayTracerLib.dll");
            if (!File.Exists(dllPath))
            {
                MessageBox.Show("File RayTracerLib.dll not found. Reinstall application.", "Can't start application", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // zjisti pritomnost DLL
            dllPath = Path.Combine(Application.StartupPath, "Splicer.dll");
            if (!File.Exists(dllPath))
            {
                MessageBox.Show("File Splicer.dlll not found. Reinstall application.", "Can't start application", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // zjisti pritomnost DLL
            dllPath = Path.Combine(Application.StartupPath, "DirectShowLib-2005.dll");
            if (!File.Exists(dllPath))
            {
                MessageBox.Show("File DirectShowLib-2005.dll not found. Reinstall application.", "Can't start application", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            // zamezeni vice spusteni soucasne
            if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 3)
            {
                MessageBox.Show("Multipe run not allowed", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());
        }
    }
}
