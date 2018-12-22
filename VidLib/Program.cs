using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
//using HtmlAgilityPack;
using System.Net.Http;
using System.Web.Script.Serialization;
using System.Threading;
using System.Runtime.InteropServices;

namespace VidLib
{

    class Program
    {
        const string CDN = "https://i.r.worldssl.net/usfi/v/";
        const bool debug = false;
        static bool Thumb = false;
        static bool Video = true;
        static string path;
        static bool Metadata = false;
        static bool Filemode = false;
        static bool error = false;
        static bool saveprogress = false;
        static bool HQ = false;
        static string Archivefile = "";
        static int cycle = 0;
        static List<string> Archive = new List<string>();
        static void Main(string[] args)
        {
            
            Vidlib s = new Vidlib();
        //    DownLoadFileBG("https://i.r.worldssl.net/usfi/v/dGOn0zLQ9DK.3q8vaXQ5L9csuO7ed4OYhNaeTsLyJN-lkv2bYmmA6um5XzqCkx6js9IZiGAAC9_rOQLgCarvA-3HKLb6.mp4", "x\\Succ.mp4");
          //  Console.ReadLine();
            if (ScreenArguments(args, "-get-thumb")){
                Thumb = true;
              //  Console.WriteLine("Tag");


            }
            if (ScreenArguments(args, "-archive"))
            {
              
                Archivefile = args[ScreenArgumentsLoc(args, "-archive")];
           
                    saveprogress = true;
                if (File.Exists(Archivefile))
                { 


                    using (StreamReader reader = new StreamReader(Archivefile))
                    {
                        while (!reader.EndOfStream)
                        {
                            Archive.Add(reader.ReadLine());
                        }
                    }
                }


             //   if (!File.Exists(Archivefile)) File.Create(Archivefile);
                //  Console.WriteLine("Tag");


            }
            if (ScreenArguments(args, "-help"))
            {
                Metadata = true;
                const int pad = 15;
                //  Console.WriteLine("Tag");
                Console.WriteLine("Usage:   \nVidlib.exe [misc arguments] [-archive <file>] -path <location>\n<Link/path to file with links> \n");
                Console.WriteLine("Arguments:\n");
                Console.Write(PadFormat("-HQ", pad));
                Console.WriteLine("Downloads the 720p version of the video if possible");
                Console.Write(PadFormat("-get-thumb", pad));
                Console.WriteLine("Downloads the thumbnail");
                Console.Write(PadFormat("-save-metadata", pad));
                Console.WriteLine("Saves the json metadata of the video");
                Console.Write(PadFormat("-no-video", pad));
                Console.WriteLine("skips downloading the video file (useful for updating metadata)");
                Console.Write(PadFormat("-path <file>", pad));
                Console.WriteLine("A mandatory argument, followed by the path to the file,");
                Console.Write(PadFormat(" ", pad));
                Console.WriteLine("path takes special arguments to generate the file location"); 
                Console.Write(PadFormat("   @[url]", pad+5));
                Console.WriteLine("video id");
                Console.Write(PadFormat("   @[file]", pad + 5));
                Console.WriteLine("CDN server file id");
                Console.Write(PadFormat("   @[title]", pad + 5));
                Console.WriteLine("video title");
                Console.Write(PadFormat("   @[category]", pad + 5));
                Console.WriteLine("Video Category");
                Console.Write(PadFormat("   @[uploaded_by]", pad + 5));
                Console.WriteLine("Uploader");
                Console.Write(PadFormat("   @[uploaded_on]", pad + 5));
                Console.WriteLine("Upload date");
                Console.Write(PadFormat("   @[ext]", pad + 5));
                Console.WriteLine("Extension (MANDATORY)\n");
                Console.Write(PadFormat("-archive <filename>", pad + 5));
                Console.WriteLine("saves the downloaded video id to a file so that you don't download it again.");
                Console.ReadLine();
                return;
            }
            if (ScreenArguments(args, "-save-metadata"))
            {
                Metadata = true;
                //  Console.WriteLine("Tag");


            }
            if (ScreenArguments(args, "-HQ"))
            {
                HQ = true;
                //  Console.WriteLine("Tag");

            }
            if (ScreenArguments(args, "-no-video"))
            {
                Video = false;
                //  Console.WriteLine("Tag");

            }
            string UPath="";
            if (ScreenArguments(args, "-path"))
            {
                UPath= args[ScreenArgumentsLoc(args, "-path")];
                path = UPath;

                 Console.WriteLine(path);
                if (path.Contains("@[")) path = path.Substring(0, path.IndexOf("@["));
                if (!Directory.Exists(path))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Location Does not exist");
                    Console.ReadLine();
                }
                if (!UPath.Contains("@[ext]"))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("WARNING: PATH NEEDS @[ext] FOR THE PROGRAM TO WORK");
                    Console.ReadLine();
                }

            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("WARNING: Program needs path argument to work");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Run with -help to print syntax");
                Console.ReadLine();
            }
            string link = args[args.Length-1];



            //console.ReadLine();
            if (link.Contains("watch") || link.Contains("user"))
            {
                if (link.Contains("vidlii.com"))
                {

                   
                        if (link.Contains("watch") && link.Contains("="))
                        {
                            path = UPath;
                            Download(link, path, s);


                        }
                    else if (link.Contains("user"))
                    {
                        bool finished = false;
                        int page = 0;
                        while (!finished)
                        {
                            WebClient client = new WebClient();
                            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                            client.DownloadFile(link, "channelraw.html");
                            string extractedID = Scanforid("channelraw.html");
                            if (File.Exists(extractedID)) File.Delete((extractedID));
                            s.MakeRequest(extractedID, page.ToString());
                            //    Console.ReadLine();
                            Console.WriteLine("Requesting Metadata");
                            while (!File.Exists(extractedID))
                            {
                                Thread.Sleep(50);
                            }
                            if (error)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Error Encountered while downloading json metadata");
                            }
                            Console.WriteLine("File found");
                            foreach (string X in File.ReadAllLines(extractedID))
                            {
                                if (X == "finished")
                                {
                                    finished = true;
                                    break;
                                }
                                path = UPath;
                                if (!string.IsNullOrWhiteSpace(X))
                                {
                                    Download("https://www.vidlii.com/watch?v=" + X, path, s);
                                }
                            }
                            page++;
                        }
                    }
                    else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("This type of vidlii link is not supported");
                            Console.ReadLine();
                        }
                    
                }
            }
            else
            {
                Console.WriteLine(link);
             
                if (File.Exists(link))
                {

                    foreach(string fileline in File.ReadAllLines(link))
                    {
                        if (!string.IsNullOrWhiteSpace(fileline))
                        {
                            link = fileline;
                          
                            if (link.Contains("vidlii.com"))
                            {


                                if (link.Contains("watch") && link.Contains("="))
                                {
                                    path = UPath;
                                    Download(link, path, s);


                                }
                                else if (link.Contains("user"))
                                {
                                    bool finished = false;
                                    int page = 0;
                                    while (!finished)
                                    {
                                        WebClient client = new WebClient();
                                        System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                                        client.DownloadFile(link, "channelraw.html");
                                        string extractedID = Scanforid("channelraw.html");
                                        if (File.Exists(extractedID)) File.Delete((extractedID));
                                        s.MakeRequest(extractedID, page.ToString());
                                    //    Console.ReadLine();
                                        Console.WriteLine("Requesting Metadata");
                                        while (!File.Exists(extractedID))
                                        {
                                            Thread.Sleep(50);
                                        }
                                        if (error)
                                        {
                                            Console.ForegroundColor = ConsoleColor.Red;
                                            Console.WriteLine("Error Encountered while downloading json metadata");
                                        }
                                        Console.WriteLine("File found");
                                        foreach (string X in File.ReadAllLines(extractedID))
                                        {
                                            if (X == "finished")
                                            {
                                                finished = true;
                                                break;
                                            }
                                            path = UPath;
                                            if (!string.IsNullOrWhiteSpace(X))
                                            {
                                                Download("https://www.vidlii.com/watch?v=" + X, path, s);
                                            }
                                        }
                                        page++;
                                    }
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("This type of vidlii link is not supported");
                                    Console.ReadLine();
                                }

                            }
                        }
                    }

                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Not a vidlii link/cannotfind file");
                    Console.ReadLine();
                }
                

            }
            Console.WriteLine("Download Finished");
            if (debug)
            {
              
                s.MakeRequest("moonman", "0");
                //s.MakeVideoRequest("X5rOJ7IVLaJ");
                s.MakeRequest(Console.ReadLine(), Console.ReadLine());
                string H2;
                H2 = Console.ReadLine();

                return;
                using (var client = new GetClient())
                {
                    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                    client.DownloadFile("https://www.vidlii.com/user/DhanesWara", "test");
                }


                Console.WriteLine(s.GetThumbnailLink("https://www.vidlii.com/watch?v=7f9sXjsG4at"));
                Console.WriteLine(s.GetVideoLink("https://www.vidlii.com/watch?v=7f9sXjsG4at", false));
                Console.ReadLine();
            }
                return;
        }
        static bool StringContainsFromList(List<string> X, string match)
        {

            foreach (string X2 in X)
            {
                if (match.Contains(X2))
                {
                    return true;
                }
            }
            return false;
        }
        static string PadFormat(string Input, int pad)
        {
            return Input.PadRight(pad);
        }
        static void Download(string link, string path, Vidlib s)
        {
         
            string FinalFolder = "";
            string Thumbnail = "";
      
            string ID = link.Substring(link.LastIndexOf("=") + 1);
         //   Console.WriteLine(ID);
            if (File.Exists(ID + ".json")) File.Delete((ID + ".json"));

            s.MakeVideoRequest(ID);
            while (!File.Exists(ID + ".json") && !error)
            {
                Thread.Sleep(50);
            }
            if (error)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error Encountered while downloading json metadata");
            }
        //    Console.WriteLine("JSON");
            while (IsFileLocked(ID + ".json"))
            {
                Thread.Sleep(50);
            }
            string JSONdata = File.ReadAllText(ID + ".json");
            VideoJSON Meta = new JavaScriptSerializer().Deserialize<VideoJSON>(JSONdata);
            s.SaveJSON(Meta, "jsontest.json");
            string ProcessedFolder = ProcessPath(path, Meta, "");
          //  Console.WriteLine(ProcessedFolder);
           // Console.WriteLine(ProcessedFolder);
            Directory.CreateDirectory(ProcessedFolder.Substring(0, ProcessedFolder.LastIndexOf("\\")));
            // Console.WriteLine("Directory Created");
            if(!StringContainsFromList(Archive, ID)) {
            if (Video)
            {
                string VideoLink = CDN + ID + "." + Meta.file;
                if (HQ && s.Check204(VideoLink)) VideoLink += ".720";
                VideoLink += ".mp4";
                //Console.WriteLine("Downloading " + VideoLink);
               //   Console.WriteLine(ProcessPath(path, Meta, ".mp4"));

                DownLoadFileBG(VideoLink, ProcessPath(path, Meta, ".mp4"));


            }
            if (Thumb )
            {
                Thumbnail = s.GetThumbnailLink(link);
                    //   Console.WriteLine(Thumbnail);
                    //   File.WriteAllText("test.txt", Thumbnail);
                    //Console.WriteLine(ProcessPath(path, Meta, ".jpg"));
                  //  Console.WriteLine("Downloading " + Thumbnail);
                    DownLoadFileBG(Thumbnail, ProcessPath(path, Meta, ".jpg"));
                    // Console.WriteLine(ProcessPath(path, Meta, ".jpg"));
                }
               
                if (saveprogress) Archive.Add(ID);
            }
            else
            {
                Console.WriteLine(Meta.title+" recorded in archive, not necessary to download");
            }
            if (Metadata)
            {
                //  Console.WriteLine(ProcessPath(path, Meta, ".json"));

                if (File.Exists(ProcessPath(path, Meta, ".json")))
                {
                    while (IsFileLocked( ProcessPath(path, Meta, ".json")))
                    {
                        Thread.Sleep(50);
                    }
                    File.Delete(ProcessPath(path, Meta, ".json"));
                }
                File.Copy("jsontest.json", ProcessPath(path, Meta, ".json"));
                //File.Copy("jsontest.json",)
              //  Console.WriteLine("Copied JSON");
            }
            if (saveprogress) File.WriteAllLines(Archivefile, Archive);


            if (File.Exists(ID + ".json"))
            {
                while (IsFileLocked(ID + ".json"))
                {
                    Thread.Sleep(50);
                }
                File.Delete(ID + ".json");
            }
        }
        static bool IsFileLocked(string filePath)
        {
            try
            {
                using (File.Open(filePath, FileMode.Open)) { }
            }
            catch (IOException e)
            {
                var errorCode = Marshal.GetHRForException(e) & ((1 << 16) - 1);

                return errorCode == 32 || errorCode == 33;
            }

            return false;
        }
        static bool ScreenArguments(string[] arguments, string Argument)
        {
            foreach(string ARG in arguments)
            {
                if (ARG.Contains(Argument)) return true;
            }
            return false;
        }
        static int ScreenArgumentsLoc(string[] arguments, string Argument)
        {
            int argloc = 0;
            foreach (string ARG in arguments)
            {
                argloc++;
                if (ARG.Contains(Argument)) return argloc;
            }
            return -1;
       
        }
        public static void DownLoadFileBG(string address, string path)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            WebClient client = new WebClient();
            Uri uri = new Uri(address);
            Console.WriteLine("Download of " + path.Substring(path.LastIndexOf("\\") + 1) +" started");
            try {
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler((sender, e) => DownloadProgressCallback(sender, e, path.Substring(path.LastIndexOf("\\") + 1)));
                Console.WriteLine(address, path);
                client.DownloadFileAsync(uri, path);

                while (client.IsBusy) Thread.Sleep(100);
                Console.WriteLine(Environment.NewLine);
             //   Console.WriteLine(Environment.NewLine);
            }
            catch(Exception x)
            {
                Console.WriteLine(x);
            }
            }
        static void DownloadProgressCallback(object sender ,DownloadProgressChangedEventArgs e,string path)
        {
            if (cycle % 10 == 0)
            {
                Console.Write("\r{0}%   ", "Downloading " + path + " " + ByteProc(e.BytesReceived) + " of " + ByteProc(e.TotalBytesToReceive) + " " + e.ProgressPercentage);
                if (Console.WindowWidth< path.Length + 50)
                Console.SetWindowSize(path.Length + 50, Console.WindowHeight);
                Console.CursorTop = Console.CursorTop;
            }
                cycle++;
        }
        static string ByteProc(long bytes)
        {
            float flo = bytes;
            if (bytes < 1024)
            {
                return flo.ToString() + " B";
            }else if (bytes < 1024*1024)
            {
                return (flo /1024).ToString("0.00") + " KB";
            }
            else if (bytes < 1024 * 1024*1024)
            {
                return ((flo / 1024)/1024).ToString("0.00") + " MB";
            }
            else
            {
                return (((flo / 1024) / 1024) / 1024).ToString("0.00") + " GB";
            }
        }
        static string Scanforid(string html)
        {
          //  string id;
            foreach (string line in File.ReadAllLines(html))
            {
                if (line.Contains("ch_user"))
                {
                    return line.Substring(line.IndexOf(">")+1, line.IndexOf("</div>") - line.IndexOf(">")-1);
                }
            }
            return "$$%!@";
        }
        static string RemoveIllegal(string og)
        {
            string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());

            foreach (char c in invalid)
            {
                og = og.Replace(c.ToString(), "");
            }
            return og;
        }
        static string ProcessPath(string OGpath, VideoJSON X, string ext)
        {
            string PathCP = OGpath;
            //Enviroment Var Declaration 
            //@[url] - video id
            //@[file] - cdn server file id
            //@[hd] - Hd video status *UNUSED*
            //@[title] - video title
            //@[category] - Video Category
            //@[uploaded_by] - uploader
            //@[uploaded_on] - upload date
            //@[ext] - Extension //MANDATORY

            string[] categories = { "All", "Film & Animation", "Autos & Vehicles", "Music", "Pets & Animals", "Sports", "Travel & Events", "Gaming", "People & Blogs", "Comedy", "Entertainment", "News & Politics", "Howto & Style", "Education", "Science & Technology", "Nonprofits & Activism" };

            PathCP = PathCP.Replace("@[url]", X.url);
            PathCP = PathCP.Replace("@[file]", X.file);
           
            PathCP = PathCP.Replace("@[uploaded_by]", X.uploaded_by);
            PathCP = PathCP.Replace("@[uploaded_on]", X.uploaded_on);
            PathCP = PathCP.Replace("@[ext]", ext);
            string CatName = "";
            if (PathCP.Contains("@[category]")) PathCP.Replace("@[category]", categories[X.category]);
           
            PathCP = PathCP.Replace("@[title]", RemoveIllegal(X.title));
            return PathCP;

        }

    }
    

    //Code from
    //https://stackoverflow.com/questions/924679/c-sharp-how-can-i-check-if-a-url-exists-is-valid
    class GetClient : WebClient
    {
        public bool HeadOnly { get; set; }
        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest req = base.GetWebRequest(address);
            if (HeadOnly && req.Method == "GET")
            {
                req.Method = "HEAD";
            }
            return req;
        }
    }
    //end of borrowed code
    public class VidLiiVideo
    {
        public Metadata Metadata;
        public VideoResponses VideoResponses;
        public Comments Comments;
        public Statistics Statistics;
        string VideoLink;
        string Thumbnail;

    }
    public class Metadata
    {
        string Description;
        string Category;
        string Tags;
        string Date;

    }
    public class VideoJSON
    {
    public    string url;
        public string file;
        public int hd;
        public string title;
        public string description;
        public string tags;
        public int category;
        public int privacy;
        public string uploaded_by;
        public string uploaded_on;
        public int views;
        public int displayviews;
        public int watched;
        public int comments;
        public int responses;
        public int favorites;
        public int star1;
        public int star2;
        public int star3;
        public int star4;
        public int star5;
        public long length;
        public int s_comments;
        public  int s_ratings;
        public int s_responses;
        public int s_related;
        public int featured;
        public int frontpage;
        public int most_popular;
        public int banned_uploader;
        public int shadowbanned_uploader;
        public int status;
        public int show_ads;
        public string displayname;
        public string rating;
    }
    public class VideoResponses
    {



    }
    public class Channel
    {
        string[] Subscribers;
        string[] Videos;
        int SubscriberAmount;
        Comments ChannelComments;

    }
    public class Comment
    {
        string Picture;
        string id;
        int rating;
        int Timespan;
    }

        public class Comments
    {
        List<Comment> CommentList;
        long Length;

    }
    public class Statistics
    {
        public long Views;
        public float Stars;
        public long Ratings;

    }
    class Vidlib
    {
        //Static Thumbnail url
        const string USFI = "https://www.vidlii.com/usfi/prvw/";
        const string THMP = "https://www.vidlii.com/usfi/thmp/";
        private bool IsVideo(string VidLiink)
        {

            if (VidLiink.Contains("watch?v=")) return true; else return false;
        }

        public bool Check204(string Link)
        {
            try {
                using (var client = new GetClient())
                {
                    client.HeadOnly = true;

                    string s1 = client.DownloadString(Link);
                    return true;

                }

            }
            catch
            {
                return false;
            }
        }
        public string GetThumbnailLink(string VidLiink)
        {
            Console.WriteLine(VidLiink);
            if (!VidLiink.Contains("www.vidlii.com")) return "Not a Vidlii Link";
           
            if (IsVideo(VidLiink))
            {
                string ThumbnailLink = USFI + VidLiink.Substring(VidLiink.LastIndexOf("=") + 1) + ".jpg";
                if (Check204(ThumbnailLink))
                {
                    return ThumbnailLink;

                }
                else
                {
                    ThumbnailLink = THMP + VidLiink.Substring(VidLiink.LastIndexOf("=") + 1) + ".jpg";
                    if (Check204(ThumbnailLink))
                    {
                        return ThumbnailLink;

                    }
                    else
                    {
                        return  "Not a valid vidlii thumbnail";
                    }
                }
               
            }
            else
            {
                return "Not a Vidlii Video Link";
            }



        }
     async public void MakeRequest(string channelid, string page)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            HttpClient client = new HttpClient();
            Console.WriteLine("Downloading metadata for " + channelid);
            var content = new FormUrlEncodedContent(new Dictionary<string, string>() { { "type", "videos" },{ "page", page.ToString() },{ "user",channelid } });
            client.DefaultRequestHeaders.Referrer = new Uri("https://www.vidlii.com/user/"+channelid);

            try {
                var resp =  await client.PostAsync("https://www.vidlii.com/ajax/show_more", content);
                var repsStr = await resp.Content.ReadAsStringAsync();
            
            //    Console.WriteLine(repsStr);
                File.WriteAllLines(channelid, GetVideos(repsStr));
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }

        }
        public string[] GetVideos(string HTML)
        {
            List<string> Videos = new List<string>();
            foreach(string Line in HTML.Split('\n'))
            {
                if (Line.Contains("class=\"mnu_vid\""))
                {
                    Videos.Add(Line.Substring(Line.IndexOf("watch=")+7,Line.LastIndexOf("\"")- (Line.IndexOf("watch=") + 7)));
                }
            }
         //   Console.ReadLine();
            Videos.Add(CheckforShowMore(HTML));
            return Videos.ToArray();

        }
        public string CheckforShowMore(string HTML)
        {
            foreach (string Line in HTML.Split('\n'))
            {
                if (Line.Contains("show_more"))
                {
                    return "";
                   
                   
                }
            }
            return "finished";
        }
        async public void MakeVideoRequest(string Videolid)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            HttpClient client = new HttpClient();
            var content = new FormUrlEncodedContent(new Dictionary<string, string>() { { "id", Videolid } });
            client.DefaultRequestHeaders.Referrer = new Uri("https://www.vidlii.com/watch?v=" + Videolid);

            try
            {
                var resp =  await client.PostAsync("https://www.vidlii.com/ajax/get_video_info", content);
                var repsStr = await resp.Content.ReadAsStringAsync();

           //     Console.WriteLine(repsStr);

            //    Console.WriteLine(repsStr);
                VideoJSON Vidstuff = new JavaScriptSerializer().Deserialize<VideoJSON>(repsStr);
                SaveJSON(Vidstuff);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }
        async public void MakeVideoRequest(string Videolid,string path)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            HttpClient client = new HttpClient();
            var content = new FormUrlEncodedContent(new Dictionary<string, string>() { { "id", Videolid } });
            client.DefaultRequestHeaders.Referrer = new Uri("https://www.vidlii.com/watch?v=" + Videolid);

            try
            {
                var resp = await client.PostAsync("https://www.vidlii.com/ajax/get_video_info", content);
                var repsStr = await resp.Content.ReadAsStringAsync();

               // Console.WriteLine(repsStr);

              //  Console.WriteLine(repsStr);
                VideoJSON Vidstuff = new JavaScriptSerializer().Deserialize<VideoJSON>(repsStr);
                SaveJSON(Vidstuff,path);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }
       public void SaveJSON(VideoJSON Vid)
        {
            var json = new JavaScriptSerializer().Serialize(Vid);
            json = ProcessJSONComma(json.Replace("{", "{\n"),',', "\n");
            File.WriteAllText(Vid.url + ".json", json);
        }
       
        public static string ProcessJSONComma (string Source, char Find, string Append)
        {
            string output="";
            bool mode = true;
            foreach(char m in Source)
            {
                output += m;
                if (mode)
                {
                    if (m == Find) output += Append;
                }
                if (m == '"') mode = !mode;
            }
            return output;
        }
        public void SaveJSON(VideoJSON Vid, string path)
        {
            var json = new JavaScriptSerializer().Serialize(Vid);
            json = ProcessJSONComma(json.Replace("{", "{\n"), ',', "\n");
            File.WriteAllText(path, json);
        }

        // public string[] ScanChannel ()
        private string ScanVideo(string HTMLPATH,bool hd)
        {
            string[] file = File.ReadAllLines(HTMLPATH);
            string HDVIDEO = "";
            string VIDEO = "";
            foreach(string Line in file)
            {
                if (Line.Contains("hdsrc:"))
                {
                    HDVIDEO = Line.Substring(Line.IndexOf("https"), Line.LastIndexOf("\"") - Line.IndexOf("https"));
                    if (HDVIDEO.Contains("https://i.r.worldssl.net/usfi/v/") && hd)
                    {
                        return HDVIDEO;
                    }
                    else
                    {
                        return VIDEO;
                    }


                }else
                if (Line.Contains("src:"))
                {
                  //  Console.WriteLine(Line.Substring(Line.IndexOf("https")));

                    VIDEO= Line.Substring(Line.IndexOf("https"), Line.LastIndexOf("\"")- Line.IndexOf("https"));
                }
                
            }
            return "No Video Found";
        }
        public string GetVideoLink(string VidLiink, bool hd)
        {
            try
            {
                if (!VidLiink.Contains("www.vidlii.com")) return "Not a Vidlii Link";

                if (IsVideo(VidLiink))
                {
                    string VideoLink = "";
                    using (var client = new GetClient())
                    {
                        string ID = VidLiink.Substring(VidLiink.LastIndexOf("=") + 1);
                        string FILEHTML = ID + ".lii";
                        System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                        client.DownloadFile("https://www.vidlii.com/embed?v=" + ID + "&a=1", FILEHTML);
                        VideoLink = ScanVideo(FILEHTML, hd);
                        if (VideoLink == "No Video Found") return "Not a Valid Video";
                        return VideoLink;

                    }
                    if (Check204(VidLiink)) return "Not a Valid Video";
                    return VideoLink;
                }
                else
                {
                    return "Not a Vidlii Video Link";
                }

            }
            catch
            {
                return "Not a Vidlii Video Link (Error)";
            }
        }

    }



}
