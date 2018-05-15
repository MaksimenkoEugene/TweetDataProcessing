using System;
using System.Configuration;
using System.IO;
using System.Text;

using TweetDataProcessing.Repositories.Contracts;

namespace TweetDataProcessing.Repositories.Data
{
    public class Logger : ILogger
    {
        private static object locker = new object();

        public void WriteLog(string text)
        {
            string log = string.Format("{0}\r\n {1}\r\n{2}\r\n\r\n",
                DateTime.Now,
                text,
                "--------------------------------------------------------");

            byte[] logArr = Encoding.UTF8.GetBytes(log);
            string filePath = ConfigurationManager.AppSettings["LogFilePath"];
            string date;

            lock (locker)
            {
                date = DateTime.UtcNow.Day.ToString() + "." + DateTime.Now.Month.ToString() + "." + DateTime.Now.Year.ToString();

                using (FileStream f = new FileStream(filePath + date + ".txt", FileMode.Append))
                {
                    f.Write(logArr, 0, logArr.Length);
                    f.Close();
                    f.Dispose();
                }
            }
        }

        public void Dispose()
        {
        }
    }
}
