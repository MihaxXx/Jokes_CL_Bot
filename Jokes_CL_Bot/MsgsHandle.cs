using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

namespace JokesBot
{
    partial class Program
    {

        static async void BotOnMessageReceived(object sender, MessageEventArgs MessageEventArgs)
        {
            Telegram.Bot.Types.Message msg = MessageEventArgs.Message;
            if (msg == null || msg.Type != MessageType.Text)
                return;

            string Answer = "Server Error";

            if (DateTime.UtcNow.Subtract(msg.Date).TotalMinutes > 3)
            {
                await BOT.SendTextMessageAsync(msg.Chat.Id, Answer);
                return;
            }

            if (UserList.ContainsKey(msg.Chat.Id))
                UserList[msg.Chat.Id].LastAccess = DateTime.Now;
            if (!IsRegistered(msg.Chat.Id))
            {
                UserList.Add(msg.Chat.Id, new User() { ident = 3 });
                Json_Data.WriteData();   //регистрация в базе
            }
            if (UserList[msg.Chat.Id].ident == 5)
            {
                bool onOrOff = msg.Text.ToLower() == "включить";
                UserList[msg.Chat.Id].eveningNotify = onOrOff;
                UserList[msg.Chat.Id].ident = 3;
                Json_Data.WriteData();
                string onOrOffMsg = onOrOff ? "включено" : "выключено";
                Answer = $"Вечернее уведомление *{onOrOffMsg}*.";
            }
            else if (UserList[msg.Chat.Id].ident == 6)
            {
                bool onOrOff = msg.Text.ToLower() == "включить";
                UserList[msg.Chat.Id].morningNotify = onOrOff;
                UserList[msg.Chat.Id].ident = 3;
                Json_Data.WriteData();
                string onOrOffMsg = onOrOff ? "включено" : "выключено";
                Answer = $"Уведомление за 15 минут до первой пары *{onOrOffMsg}*.";
            }
            else
            {
                try
                {
                    switch (msg.Text.ToLower())             // Обработка команд боту
                    {
                        case "/start":
                            Answer = "Добро пожаловать!\n" + _help;
                            break;
                        case "/joke":
                        case "анекдот":
                            Answer = Markov.MakeText(1, random);//"No jokes yet:(";//TODO Insert call to jokes getter
                            break;
                        case "/knowme":
                        case "знаешь меня?":
                            Answer = $"Вы {msg.Chat.FirstName.Replace("`", "").Replace("_", "").Replace("*", "")}.";
                            break;

                        case "/eveningnotify":
                            Answer = $"Сейчас вечернее уведомление *{(UserList[msg.Chat.Id].eveningNotify ? "включено" : "выключено")}*. \nНастройте его.";
                            UserList[msg.Chat.Id].ident = 5;
                            await BOT.SendTextMessageAsync(msg.Chat.Id, Answer, ParseMode.Markdown, replyMarkup: notifierKeyboard);
                            return;

                        case "/morningnotify":
                            Answer = $"Сейчас утреннее уведомление *{(UserList[msg.Chat.Id].morningNotify ? "включено" : "выключено")}*. \nНастройте его.";
                            UserList[msg.Chat.Id].ident = 6;
                            await BOT.SendTextMessageAsync(msg.Chat.Id, Answer, ParseMode.Markdown, replyMarkup: notifierKeyboard);
                            return;


                        case "/forget":
                        case "забудь меня":
                            UserList.Remove(msg.Chat.Id);
                            Json_Data.WriteData();
                            Answer = "Я вас забыл! Для повторной регистрации пиши /start";
                            await BOT.SendTextMessageAsync(msg.Chat.Id, Answer, replyMarkup: new ReplyKeyboardRemove());
                            return;

                        case "помощь":
                        case "/help":
                            Answer = _help;
                            break;
                        case "/info":
                        case "информация":
                            Answer = "Меня создали иллюминаты!";
                            break;
                        case "/forceupdate":
                            logger.Info($"Запрошено принудительное обновление, ID: {msg.Chat.Id}, @{msg.Chat.Username}.");

                            logger.Info($"Завершено принудительное обновление, ID: {msg.Chat.Id}, @{msg.Chat.Username}.");
                            Answer = "Данные обновлены!";
                            break;
                        default:
                            Answer = "Введены неверные данные, повторите попытку.";
                            break;
                    }
                }
                catch (System.Net.WebException e)
                {
                    logger.Error(e, "Catched exeption:");
                    Answer = "Ошибка! Вероятно, что-то умерло. Пожалуйста, попробуйте повторить запрос позднее.";
                }
            }
            try
            {
                await BOT.SendTextMessageAsync(msg.Chat.Id, Answer, ParseMode.Markdown, replyMarkup: defaultKeyboard);
            }
            catch (Exception ex) when (ex is System.Net.Http.HttpRequestException && ex.Message.Contains("429"))
            {
                logger.Warn(ex, $"Сетевая ошибка при ответе @{msg.Chat.Username}");
            }
        }

        /// <summary>
        /// Keyboard for registered users
        /// </summary>
        static ReplyKeyboardMarkup defaultKeyboard = new ReplyKeyboardMarkup(new[] {
                            new[]{ new KeyboardButton("Анекдот"),new KeyboardButton("Информация") },      //Кастомная клава для студентов
                            new[]{ new KeyboardButton("Знаешь меня?"),new KeyboardButton("Помощь") }
                            }
                        );

        static ReplyKeyboardMarkup notifierKeyboard = new ReplyKeyboardMarkup(new[] {
                                    new[]{ new KeyboardButton("Включить"),new KeyboardButton("Выключить") },      //Keyboard for notifier settings
                                    }
                        );
        /// <summary>
        /// Some options for keyboards
        /// </summary>
        static void KeyboardInit()
        {
            defaultKeyboard.ResizeKeyboard = true;
            notifierKeyboard.ResizeKeyboard = true;
        }

        /// <summary>
        /// Checks if user is already registered
        /// </summary>
        /// <param name="id">Telegram user ID</param>
        /// <returns></returns>
        static bool IsRegistered(long id) => UserList.ContainsKey(id) && UserList[id].ident > 2;

        private static readonly string _help = @"Список команд: 
/joke - выдать анекдот
/info — краткое описание бота    
/knowme — информация о пользователе
/eveningNotify — настроить вечернее уведомление
/morningNotify — настроить утреннее уведомление
/forget — сменить пользователя
/help — список команд";
    }
}
