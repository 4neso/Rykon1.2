using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RykonServer
{
    public enum RemoteCommandType { ReadFile, WriteFile, MessageBox,  CreateFile, CreateDir, Sleep, CloseProcess, CloseAllProcess, unkown, process }
    public class RemoteCommandExecuter
    {
        public string ProcessName = "";
        public string FileName = "";
        public string DirName = "";
        public string OutPut = "";
        public string ProcessPar = "";
        private string StringText;
        private RemoteCommandType ComType = RemoteCommandType.unkown;
        public RemoteCommandExecuter(string main)
        {
            string [] pcs = new string[]{};
            this.StringText = main;
           // jex&com=msgbx&title=hello+It"
            if (main.Contains("&"))
                pcs = main.Split('&');

            foreach (string s in pcs)
            {
                if (!s.Contains("="))
                    continue;
                string[] pc2 = s.Split('=');
                this.SetIdAndVal(pc2[0], pc2[1]);
            }


                               
        }

        private void SetIdAndVal(string id, string val)
        {
            string Decoded = WebServer.DecodeUrlChars(val);
            id = id.ToLower().Trim();
            switch (id)
            {
                case "com":
                    {
                        if(val=="msgbx")
                            this.ComType = RemoteCommandType.MessageBox;
                        else  if (val == "procstr" || val =="proc")
                                this.ComType = RemoteCommandType.process;
                        else if (val == "closproc" || val == "closeprocess")
                            this.ComType = RemoteCommandType.CloseProcess;
                        else if (val == "closallproc" || val == "closeallprocess")
                            this.ComType = RemoteCommandType.CloseAllProcess;
                        
 
                            break;
                    }
                case "mbtitle": this.MessageBoxCap = Decoded; break;
                case "procnm": this.ProcessName = val; break;
                case "proctype": this.ViewImageProcess = val == "pic"; break;
                case "procpar": this.NoProcessPar = false; this.ProcessPar = Decoded; break;
            }
        }
        public string Result ="no operation made "; 
        internal void proceeed()
        {
            switch (ComType)
            {
                case RemoteCommandType.process:
                    {
                        if (this.ViewImageProcess)
                            ProcessName = Application.StartupPath + "\\RootDir\\"+ProcessName;
                        string r = "";
                        if (this.NoProcessPar)
                            r = AppHelper.StartProcess(this.ProcessName).ToString();
                        else
                            r = AppHelper.StartProcess(this.ProcessName, this.ProcessPar);
                            this.Result = this.ProcessName + "      proce = " + r;
                         break;
                    }
                case RemoteCommandType.MessageBox:
                    {
                        AppHelper.EnormusMessageBox(MessageBoxCap); 
                        this.Result = "msgbx sent";
                        break;
                    }
                case RemoteCommandType.CloseProcess:
                    {
                        string r =   AppHelper.CloseProcess(ProcessName); 
                        this.Result = "closed=" + r;
                        break;
                    }
                case RemoteCommandType.CloseAllProcess:
                    {
                        this.Result = "closed=" + AppHelper.CloseProcessAll(ProcessName); break;
                    }


            }
        }

        public string MessageBoxCap="";// { get; set; }

        public bool ViewImageProcess { get; set; }

        public bool NoProcessPar=true;// { get; set; }
    }
}
