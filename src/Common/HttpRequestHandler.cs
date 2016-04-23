using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Configuration;

namespace AXP.AF.Common
{


    /// <summary>
    /// For HttpWebRequest and Response.
    /// </summary>
    public class HttpRequestHandler
    {
        /// <summary>
        /// Initializes a new instance of the HttpRequestHandler class, with details for httprequest
        /// </summary>
        public HttpRequestHandler()
        {
            ////todo
        }

        /// <summary>
        /// Initializes a new instance of the HttpRequestHandler class, with details for httprequest
        /// </summary>
        /// <param name="networkUserName">User Name for the network credential</param>
        /// <param name="networkPassword">Password for the network credential</param>
        /// <param name="certificatePath">Authentication client certificate path</param>
        /// <param name="certificatePassword">Authentication client certificate password</param>
        /// <param name="contentType">Content Type of the data</param>
        /// <param name="userAgent">The user agent for connectivity</param>
        /// <param name="requestComponent">The Http request to be created.</param>
        /// <param name="data">The string data.</param>
        public HttpRequestHandler(
                                    string contentType,
                                    string userAgent,
                                    string requestComponent,
                                    string data,
                                    string networkUserName,
                                    string networkPassword,
                                    string certificatePath,
                                    string certificatePassword
                                    )
        {
            if (networkUserName != string.Empty && networkPassword != string.Empty)
            {
                this.RequestCredential = new NetworkCredential(networkUserName, networkPassword);
            }
            if (certificatePath != string.Empty && certificatePassword != string.Empty)
            {
                this.ClientCertificate = new X509Certificate(certificatePath, certificatePassword);
            }
            this.RequestContentType = contentType;
            this.RequestUserAgent = userAgent;
            this.RequestComponent = requestComponent;
            this.RequestData = data;
        }

        /// <summary>
        /// Initializes a new instance of the HttpRequestHandler class. with details for httprequest
        /// </summary>
        /// <param name="credential">Nerwork used for the connection</param>
        /// <param name="certificate">Client certificate used for authentication</param>
        /// <param name="contentType">Content Type of the data</param>
        /// <param name="userAgent">The user agent for connectivity</param>
        /// <param name="requestComponent">The Http request to be created.</param>
        /// <param name="data">The string data.</param>
        public HttpRequestHandler(
                                    NetworkCredential credential,
                                    X509Certificate certificate,
                                    string contentType,
                                    string userAgent,
                                    string requestComponent,
                                    string data)
        {
            this.RequestCredential = credential;
            this.ClientCertificate = certificate;
            this.RequestContentType = contentType;
            this.RequestUserAgent = userAgent;
            this.RequestComponent = requestComponent;
            this.RequestData = data;
        }

        /// <summary>
        /// Gets or sets Network credential for http request.
        /// </summary>
        public NetworkCredential RequestCredential { get; set; }

        /// <summary>
        /// Gets or sets Authentication Certificate
        /// </summary>
        public X509Certificate ClientCertificate { get; set; }

        /// <summary>
        /// Gets or sets Content type for the HttpRequest
        /// </summary>
        public string RequestContentType { get; set; }

        /// <summary>
        /// Gets or sets Gets or Sets User agent of the HttpRequest
        /// </summary>
        public string RequestUserAgent { get; set; }

        /// <summary>
        /// Gets or sets The create object of the HttpRequest
        /// </summary>
        public string RequestComponent { get; set; }

        /// <summary>
        /// Gets or sets data to be send for precessing
        /// </summary>
        public string RequestData { get; set; }

        /// <summary>
        /// Process the request and gets the response and transmits the data.
        /// </summary>
        /// <returns>Response for the request.</returns>
        public string GetHttpResponse()
        {
            try
            {
                string responseData;
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["Proxy"]))
                {
                    WebProxy p = new WebProxy();
                    p.Address = new Uri(ConfigurationManager.AppSettings["ProxyUri"], UriKind.Absolute);
                    if (ConfigurationManager.AppSettings.AllKeys.Contains("ProxyUserID"))
                    {
                        p.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["ProxyUserID"],
                                                ConfigurationManager.AppSettings["ProxyPwd"],
                                                ConfigurationManager.AppSettings["ProxyDomain"]);
                    }
                    HttpWebRequest.DefaultWebProxy = p;
                }

                HttpWebRequest webRequest = WebRequest.Create(this.RequestComponent) as HttpWebRequest;

                if (this.RequestCredential != null)
                {
                    webRequest.Credentials = this.RequestCredential;
                }
                if (this.ClientCertificate != null)
                {
                    webRequest.ClientCertificates.Add(this.ClientCertificate);
                }
                webRequest.Method = "POST";
                webRequest.ContentType = this.RequestContentType;
                webRequest.UserAgent = this.RequestUserAgent;

                byte[] buffer = Encoding.UTF8.GetBytes(this.RequestData);
                using (Stream requestStream = webRequest.GetRequestStream())
                {
                    requestStream.Write(buffer, 0, buffer.Length);
                    requestStream.Close();
                }

                HttpWebResponse response = webRequest.GetResponse() as HttpWebResponse;
                using (Stream resStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(resStream, Encoding.ASCII);
                    responseData = reader.ReadToEnd();
                }

                return responseData;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}