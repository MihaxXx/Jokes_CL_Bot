using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using AnekParser;
using Newtonsoft.Json;
using NLog;
using Telegram;
using Telegram.Bot.Types.Enums;

//using Notify


namespace JokesBot
{
    public static class Extensions
    {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> func)
            => enumerable.Select(x =>
            {
                func(x);
                return 0;
            }).ToList();
    }

    partial class Program
    {
        public static NodeChainOptimized Markov { get; set; }

        public static Random random = new Random(); 
        
        /// <summary>
        /// "-nopreload" - prevents loading shedules on start
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Markov = JsonConvert.DeserializeObject<NodeChainOptimized>(File.ReadAllText("markov-o.json"));
            
            Json_Data.ReadData();
            KeyboardInit();


            BOT = new Telegram.Bot.TelegramBotClient(ReadToken());
            logger.Info("Bot connected.");
            BOT.OnMessage += BotOnMessageReceived;

            BOT.StartReceiving(new UpdateType[] { UpdateType.Message });
            //Scheduler.RunNotifier().GetAwaiter().GetResult();
            logger.Info("Waiting for messages...");

            Console.CancelKeyPress += OnExit;
            _closing.WaitOne();
        }


        static public Logger logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// Bot instance to interact with Telegram
        /// </summary>
        static Telegram.Bot.TelegramBotClient BOT;

        /// <summary>
        /// User DB by Telegram IDs
        /// </summary>
        static public Dictionary<long, User> UserList = new Dictionary<long, User>();

        private static readonly AutoResetEvent _closing = new AutoResetEvent(false);

        protected static void OnExit(object sender, ConsoleCancelEventArgs args)
        {
            BOT.StopReceiving();
            Json_Data.WriteData();
            logger.Info("Exit.");
            _closing.Set();
        }

        
        /// <summary>
        /// Reads the bot token from file 'token.key'.
        /// </summary>
        /// <returns>The token.</returns>
        public static string ReadToken()
        {
            string token = string.Empty;
            try
            {
                token = File.ReadAllText("token.key", Encoding.UTF8);
            }
            catch (FileNotFoundException e)
            {
                logger.Info("File 'token.key' wasn't found in the working directory!\nPlease save Telegram BOT token to file named 'token.key'.", e);
                Environment.Exit(1);
            }
            return token;
        }
    }
}
