using System;
using System.Net;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace p2t.Resources.Modules
{
    class Telegram
    {
        private static bool _status;
        private static string _statusDescription = "General error";
        private static string _telegramBotToken;
        private static string _telegramChatId;
        private static readonly string _telegramFirstPartOfUrl = "https://api.telegram.org/bot";

        public Telegram(string telegramBotToken, string telegramChatId)
        {
            _telegramBotToken = telegramBotToken;
            _telegramChatId = telegramChatId;
        }

        public void SendMessage(string messageText)
        {
            string telegramApiMethod = "sendMessage";
            string telegramCallUri = _telegramFirstPartOfUrl + _telegramBotToken + "/" + telegramApiMethod;

            try
            {
                WebRequest webRequest = WebRequest.Create(telegramCallUri);
                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.Method = "POST";
                string postDataChat = "chat_id=" + _telegramChatId;
                string postDataText = "text=" + messageText;
                string postData = postDataChat + "&" + postDataText;
                byte[] postDataAsByteArray = Encoding.UTF8.GetBytes(postData);

                webRequest.ContentLength = postDataAsByteArray.Length;

                using (Stream dataStream = webRequest.GetRequestStream())
                {
                    dataStream.Write(postDataAsByteArray, 0, postDataAsByteArray.Length);
                    dataStream.Close();
                }

                WebResponse webResponse = webRequest.GetResponse();
                string webResponseStatus = ((HttpWebResponse)webResponse).StatusDescription;
                //for debug
                //Console.WriteLine(webResponseStatus);

                if (webResponseStatus == "OK")
                {
                    using (Stream dataStream = webResponse.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(dataStream ?? throw new InvalidOperationException());
                        string responseFromServer = reader.ReadToEnd();
                        dataStream.Close();

                        //for debug
                        //Console.WriteLine(responseFromServer);

                        //extract state, it's a string BETWEEN FIRST "ok": AND FIRST ,
                        //example of original string: {"ok":true,"result":...
                        //result is: true
                        Regex regExPattern = new Regex(@"(?<=^.""ok"":)(.*?)(?=,)");
                        string regExExtract = regExPattern.Match(responseFromServer).ToString();

                        //for debug
                        //Console.WriteLine(regExExtract.ToString());

                        if (regExExtract == "true")
                        {
                            _status = true;
                        }
                        else
                        {
                            _status = false;
                            _statusDescription = "";
                        }
                    }
                }
                else
                {
                    _status = false;
                    _statusDescription = webResponseStatus;
                }
                webResponse.Close();
            }
            catch (Exception e)
            {
                _status = false;
                _statusDescription = e.ToString();
                //for debug
                //Console.WriteLine(e);
            }
        }
        public bool Status()
        {
            return _status;
        }

        public string StatusDescription()
        {
            return _statusDescription;
        }

    }
}