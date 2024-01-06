using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ContextMenuCmd
{

    internal class Program
    {
        public static bool IsUserAdministrator()
        {
            bool isAdmin;
            try
            {
                WindowsIdentity user = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(user);
                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (UnauthorizedAccessException ex)
            {
                isAdmin = false;
            }
            catch (Exception ex)
            {
                isAdmin = false;
            }
            return isAdmin;
        }
        [STAThread]
        static void Main(string[] args)
        {
            RegistryKey ClassesRoot = Registry.ClassesRoot;
            if (args.Length == 0 )
            {
                if (!IsUserAdministrator())
                {
                    Console.WriteLine($"ExecutionLevel: asInvoker");
                    Thread.Sleep(2000);
                    return;
                }
                 
                string Create(RegistryKey shell , string subkey = "ContextMenuCmd", string name = "Cкопировать путь", string parametr = "%1")
                {
                    RegistryKey root = shell.CreateSubKey(subkey);
                    RegistryKey command = root.CreateSubKey("command");
                    root.SetValue("MUIVerb", name);
                    string arg_ = $"\"{System.Reflection.Assembly.GetEntryAssembly().Location}\" \"{parametr}\"";
                    command.SetValue(string.Empty, arg_);
 
                   

                    var v = $"{command.ToString()} : {arg_}";
                    Debug.WriteLine(v);

                    root.Dispose();
                    command.Dispose();
                    shell.Dispose();
                    return $"{v}";
                }
                {
                    Create(ClassesRoot.OpenSubKey("*\\shell", true)          , "ContextMenuCmd" , "Cкопировать путь" , "%1");
                    Create(ClassesRoot.OpenSubKey("Directory\\shell", true)  , "ContextMenuCmd", "Cкопировать путь", "%1");

                    Create(ClassesRoot.OpenSubKey("Directory\\shell" , true) , "ContextMenuCmd_Copy" , "Скопировать имена" , "%1,1");
               //     Create(ClassesRoot.OpenSubKey("*\\shell", true)          , "ContextMenuCmd_Copy", "Скопировать имена", "%1 1");

                  
                }

            }
            else
            {
                if (args.Length == 0)
                    return;

              
              
                if (args.Length == 1)
                {
                    string[] args_d = args[0].Split(',');

                    Console.WriteLine(string.Join(" ", args_d) + ": " + args_d.Length);

                    if (args_d.Length == 2)
                    {
                        if (args_d[1] == "1")
                        {
                            if (Directory.Exists(args_d[0]))
                            {
                                string data = "";
                                foreach (var item in Directory.GetFiles(args_d[0]))
                                {
                                    data += $"\"{item}\" ";
                                }
                                data += $"\n";
                                foreach (var item in Directory.GetDirectories(args_d[0]))
                                {
                                    data += $"\"{item}\" ";
                                }
                                Clipboard.SetText(data);
                                return;
                            }

                        }
                    }
                    else
                    {
                        Clipboard.SetText(args[0]);
                    }

                   
                   
                }
                  
                 

                 
            


                
            }
        }  
    }
}
