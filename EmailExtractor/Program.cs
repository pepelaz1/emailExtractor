using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using EmailExtractor.Properties;

namespace EmailExtractor
{
    class Program
    {
        static string GetSource(string url)
        {
            string result = "";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;
                if (response.CharacterSet == null)
                    readStream = new StreamReader(receiveStream);
                else
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                result = readStream.ReadToEnd();

 
                response.Close();
                readStream.Close();
            }
            return result;
        }


        static void Main(string[] args)
        {
            using (StreamWriter outfile = new StreamWriter("result.txt"))
            {
                //int n = 0;
                foreach (var url in File.ReadAllLines("input.txt"))
                {
                    Console.WriteLine("Processing url: " + url);
                    outfile.WriteLine("Processing url: " + url);

                   // n++;
                    try
                    {

                        //if (n % 5 == 0)
                        //{
                        //    throw new Exception("Error");
                        //}

                        List<String> lst = new List<String>();

                        string id = Path.GetFileNameWithoutExtension(url);
                        Uri uri = new Uri(url);
                        string host = uri.Host;
                        string u = "http://" + host + "/reply/" + id;

                        bool found = ProcessUrl(u, lst);
                        if (!found)
                        {
                            found = ProcessUrl(url, lst);
                        }

                        if (!found)
                        {
                            outfile.WriteLine("Nothing found");
                        }
                        else
                        {
                            foreach (string s in lst)
                                outfile.WriteLine(s);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        outfile.Write(ex.Message);

                        string s = "Wait for " + Settings.Default.Delay.ToString() + " seconds";
                        Console.WriteLine(s);

                        Thread.Sleep(Settings.Default.Delay * 1000);
                    }
                    outfile.Write(Environment.NewLine);
                }
            }
        }

        static private bool ProcessUrl(string url, List<string> lst)
        {
            bool found = false;
            try
            {
                string src = GetSource(url);
                foreach (Match m in Regex.Matches(src, @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?"))
                {
                    if (!lst.Contains(m.Value))
                        lst.Add(m.Value);
                    //sw.WriteLine(m.Value);
                    found = true;
                }
            }
            catch (Exception)
            {
            }
            return found;
        }
    }
}
