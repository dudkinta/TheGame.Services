using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace RussianBotDaemon
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var _botClient = new TelegramBotClient(Config.BotToken);
            var _receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new[]
                {
                        UpdateType.Message,
                        UpdateType.CallbackQuery
                    },
                ThrowPendingUpdates = true,
            };

            _botClient.StartReceiving(UpdateHandler, ErrorHandler, _receiverOptions); // Запускаем бота
            var me = await _botClient.GetMeAsync();
        }

        private async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                switch (update.Type)
                {
                    case UpdateType.Message:
                        {
                            var message = update.Message;
                            if (message == null)
                                throw new NullReferenceException(nameof(update.Message));

                            var user = message.From;
                            if (user == null)
                                throw new NullReferenceException(nameof(user));

                            var isRussian = user.LanguageCode == "ru";
                            var chat = message.Chat;
                            switch (message.Type)
                            {
                                case MessageType.Text:
                                    {
                                        if (message.Text == "/start")
                                        {
                                            var inlineKeyboard = new InlineKeyboardMarkup(
                                                new List<InlineKeyboardButton[]>()
                                                {
                                                new InlineKeyboardButton[]
                                                {
                                                    InlineKeyboardButton.WithUrl(isRussian ? Config.EnterToGame_Ru : Config.EnterToGame_En, "https://t.me/rus2024bot/game/"),
                                                },
                                                });
                                            var userName = user.FirstName ?? (isRussian ? Config.Gamer_Ru : Config.Gamer_En);
                                            await botClient.SendTextMessageAsync(
                                                chat.Id,
                                                $"{(isRussian ? Config.Welcome_Ru : Config.Welcome_En)} {user.FirstName}",
                                                replyMarkup: inlineKeyboard);

                                            return;
                                        }

                                        return;
                                    }
                                default:
                                    {
                                        await botClient.SendTextMessageAsync(
                                            chat.Id,
                                            isRussian ? Config.UseOnlyText_Ru : Config.UseOnlyText_En);
                                        return;
                                    }
                            }
                        }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
        {
            var ErrorMessage = error switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => error.ToString()
            };
            _logger.LogError(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}
