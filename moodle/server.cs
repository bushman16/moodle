﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.IO;
using System.Web;
namespace server
{
    public class server
    {
        /*реализация сервера*/
        public Dictionary<string,string> setting;
        public server(Dictionary<string, string> s) {
            setting = s;
            setting["adress"] = "localhost" ;
            setting["port"] = "8000";
            listener = new HttpListener();
            //listener.Prefixes.Add("http://localhost:8000/");
            parsers = new Dictionary<string,file_opener.file_opener>();
        }

        public Dictionary<string, file_opener.file_opener> parsers;
        public Dictionary<string, string> global_settings;
        public void set_settings(Dictionary<string, string> s) {
            setting = s;
        }

        public  HttpListener listener;
        public  string url = "http://localhost:8000/";
        //public string pageData = File.ReadAllText("..\\..\\index.html");
        bool quit = false;
        public void stop() {
            if (listener.IsListening) listener.Stop();
            quit = true;
        }

        public void HandleIncomingConnections()
        {

            // Will wait here until we hear from a connection
            if (quit) { return; }
            listener.Start();

            //try {
                HttpListenerContext ctx = listener.GetContext();


                // Peel out the requests and response objects
                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;



                // If `shutdown` url requested w/ POST, then shutdown the server after serving the page
                if ((req.HttpMethod == "POST"))
                {
                    StreamReader reader = new StreamReader(req.InputStream,Encoding.UTF8);
                    string content;
                    string data;
                    content = reader.ReadToEnd();
                    
                    if (parsers.Keys.Contains(req.Url.AbsolutePath.Substring(1))) {
                        parsers[req.Url.AbsolutePath.Substring(1)].parse(content);
                        data = "ok";
                    }
                    else
                    {
                        data = "File type: '" + req.Url.AbsolutePath.Substring(1) + "' not suported.\nSuported types:";
                        foreach (var item in parsers.Keys)
                        {
                            data += item + "\n";
                        }
                    }
                    resp.ContentType = "text/html";
                    resp.ContentEncoding = Encoding.UTF8;
                    resp.ContentLength64 = data.Length;

                    resp.OutputStream.Write(Encoding.UTF8.GetBytes(data), 0, data.Length);
                    resp.Close();
                }

                if (req.Url.AbsolutePath == "/settings") {
                    
                    string page = File.ReadAllText("..\\..\\settings.html");
                    string data = "";
                    
                     if (req.Url.Query == "") {
                        data += "<form>";
                        foreach (var item in global_settings)
                        {
                            data += string.Format("<input type=\"text\" name=\"{0}\" id=\"{0}\" value=\"{1}\" required><label for=\"{0}\">{0}</label><br>", item.Key, item.Value);
                        }
                        data += "</form>";
                        page = page.Replace("<!--SETTINGS-->", data);
                    }
                    else
                    {
                        string[] param = req.Url.Query.Substring(1).Split('&');
                        foreach (var item in param)
                        {
                            if (item == "save-settings=Submit+Query") { continue; }
                            string[] tmp = item.Split('=');
                            global_settings[WebUtility.UrlDecode(tmp[0])] = WebUtility.UrlDecode(tmp[1]);
                        }
                        page = "<html><head><meta charset=\"utf-8\"></head><body><script type=\"text/javascript\">window.location = \"http://localhost:" + setting["port"] + "/settings\";</script></body></html>";
                    }
                     
                    // Write the response info

                   

                    
                    resp.ContentType = "text/html";
                    resp.ContentEncoding = Encoding.UTF8;
                    resp.ContentLength64 = page.Length;
                Console.Write(page);
                    resp.OutputStream.Write(Encoding.UTF8.GetBytes(page), 0, page.Length);
                    resp.Close();
                }
                else
                {
                    // Write the response info
                    byte[] data = Encoding.UTF8.GetBytes(File.ReadAllText("..\\..\\index.html"));
                    resp.ContentType = "text/html";
                    resp.ContentEncoding = Encoding.UTF8;
                    resp.ContentLength64 = data.LongLength;

                    resp.OutputStream.Write(data, 0, data.Length);
                    resp.Close();
                }
                
           // }
           // catch { }
            
            
        }
    }
}
