using System;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;

// 基于 C# 实现的 HTTP 基本认证示例，使用 RTC 的服务端 RESTful API
namespace Examples.System.Net
{
    public class WebRequestPostExample
    {
        public static void Main()
        {
            // 客户 ID
            string customerKey = "4bc9fcd5ce2b466ba3d8e15556f7bf76";
            // 客户密钥
            string customerSecret = "050a7d0ca6674f258549d6b514ae5c68";
            // 拼接客户 ID 和客户密钥
            string plainCredential = customerKey + ":" + customerSecret;

            // 使用 base64 进行编码
            var plainTextBytes = Encoding.UTF8.GetBytes(plainCredential);
            string encodedCredential = Convert.ToBase64String(plainTextBytes);
            // 创建 authorization header
            string authorizationHeader = "Authorization: Basic " + encodedCredential;

            // 创建请求对象
            WebRequest request = WebRequest.Create("https://api.agora.io/dev/v1/channel/8f13546e9a7b4beb954d69f8e3cb932a");
            request.Method = "GET";

            // 添加 authorization header
            request.Headers.Add(authorizationHeader);
            request.ContentType = "application/json";

            WebResponse response = request.GetResponse();
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);

            using (Stream dataStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                Debug.Log(responseFromServer);
            }

            Debug.Log(response.ResponseUri);

            response.Close();
        }
    }
}