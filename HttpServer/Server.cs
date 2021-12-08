
using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Threading;
using System.Web;
using System.Collections;
using System.Collections.Specialized;
using System.Text;
using System.Windows.Forms;
using System.Windows;
using System.Reflection;

namespace HttpServer
{


    public class Server
    {
        private readonly string[] _indexFiles = {
        "index.html",
        "index.htm",
        "default.html",
        "default.htm"
    };


        private Thread _serverThread;
        private string _postPath;
        private string _gettPath;
        private HttpListener _listener;
        private int _port;
        private string _ip;
        public frmMain imainfrm;

        public string OnMessage
        {
            get { return this.imainfrm.OnMessage; }
            set { this.imainfrm.OnMessage = value; }
        }

        public int Port
        {
            get { return _port; }
            private set { }
        }

        public string IP
        {
            get { return _ip; }
            private set { }
        }

        public string GetPath
        {
            get { return _gettPath; }
            private set { }
        }
        
        public string PostPath
        {
            get { return _postPath; }
            private set { }
        }

        public Server(string IP, int port, string getpath, string postpath , frmMain main1 ) 
        {

            _ip = IP;
            _port = port;
            _gettPath = getpath;
            _postPath = postpath;
            imainfrm = main1;

        }

        public void Start()
        {
            StartServer();
        }

        public void Stop()
        {
            _serverThread.Abort();
            _listener.Stop();
        }

        public bool IsServerRunning { get {

                if (_listener != null)
                    return _listener.IsListening;
                else
                    return false;

         } }

        private void Listen(frmMain imainfrm)
        {
            try
            {
                _listener = new HttpListener();
                _listener.Prefixes.Add("http://" + IP + ":" + Port.ToString() + "/");
                _listener.Start();
            }
            catch (Exception e)
            {
                OnMessage = e.Message;
                return;

            }
            
            while (true)
            {
                try
                {
                    HttpListenerContext context = _listener.GetContext();
                    Process(context, imainfrm);
                }
                catch (Exception ex)
                {

                }
            }
        }
        private void messageBeep01()
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(@".\beep-01a.wav");
            player.Play();
        }
        private void Process(HttpListenerContext context, frmMain imainfrm)
        {
            try
            {
                DateTime localDate = DateTime.Now;
                string PathName = context.Request.Url.AbsolutePath;
                string clientIP = context.Request.RemoteEndPoint.ToString();

                string logMessage = "";
                logMessage += "Time: " + localDate.ToString(" H:mm:ss");
                logMessage += ", Client IP: " + clientIP;
                logMessage += ", URL: " + context.Request.RawUrl;
                logMessage += "\r\n";

                //MessageBeep(100000);
                messageBeep01();
                imainfrm.Invoke(imainfrm.myDelegate, (logMessage));

                context.Response.ContentType = "text/html";
                context.Response.AddHeader("Date", DateTime.Now.ToString("r"));
                string replyMessage = "<!DOCTYPE html>";
                replyMessage += "<html>";
                replyMessage += "<head>";
                replyMessage += "</head>";
                replyMessage += "<body>";
                replyMessage += "<h1>I'm web Server !</h1>";
                replyMessage += "<h2>Your requested url was not found on this server </h2>";
                replyMessage += "<p>"+ context.Request.RawUrl + "</p>";

                replyMessage += "</body>";
                replyMessage += "</html>";
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(replyMessage);





                context.Response.OutputStream.Write(bytes, 0, bytes.Length);


                context.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Response.OutputStream.Flush();
                context.Response.OutputStream.Close();
 


            }
            catch (Exception ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
        }

        private void StartServer()
        {
            // Supply the state information required by the task.
            ThreadWithState tws = new ThreadWithState(
                "This report displays the number {0}.",
                42,
               new ExampleCallback(Listen),
               imainfrm
            );

            // _serverThread = new Thread(this.Listen);
            _serverThread = new Thread(new ThreadStart(tws.ThreadProc));
            _serverThread.Start();
        }

        public static void ResultCallback(int lineCount)
        {
            Console.WriteLine(
                "Independent task printed {0} lines.", lineCount);
        }
    }
    public delegate void ExampleCallback(frmMain imainfrm);

    // The ThreadWithState class contains the information needed for
    // a task, the method that executes the task, and a delegate
    // to call when the task is complete.
    //
    public class ThreadWithState
    {
        // State information used in the task.
        private string boilerplate;
        private int numberValue;
        private frmMain imainfrm;

        // Delegate used to execute the callback method when the
        // task is complete.
        private ExampleCallback callback;

        // The constructor obtains the state information and the
        // callback delegate.
        public ThreadWithState(string text, int number,
            ExampleCallback callbackDelegate, frmMain imainfrm1)
        {
            imainfrm = imainfrm1;
            boilerplate = text;
            numberValue = number;
            callback = callbackDelegate;
        }

        // The thread procedure performs the task, such as
        // formatting and printing a document, and then invokes
        // the callback delegate with the number of lines printed.
        public void ThreadProc()
        {
            Console.WriteLine(boilerplate, numberValue);
            if (imainfrm != null)
                callback(imainfrm);
        }
    }
}

