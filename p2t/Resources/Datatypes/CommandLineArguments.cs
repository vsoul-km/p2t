namespace p2t.Resources.DataTypes
{
    class CommandLineArguments
    {
        public string Address { get; set; }
        public bool IsAddress { get; set; }
        public int PacketSize { get; set; } = 32;
        public int PingCount { get; set; } = 0;
        public int PingTimeout { get; set; } = 2000;
        public int PingRttInterval { get; set; } = 500;
        public bool DoNotFragment { get; set; } = false;
        public bool LogEnabled { get; set; } = false;
        public bool FollowTheName { get; set; } = false;
        public bool AddDate { get; set; } = false;
        public bool NoTrace { get; set; } = false;
        public bool ErrorsOnly { get; set; } = false;
        public bool UsingTelegram { get; set; } = false;
        public bool TelegramSendAll { get; set; } = false;
        public bool TelegramSendErrors { get; set; } = true;
        public string TelegramBotToken { get; set; }
        public string TelegramChatId { get; set; }
    }
}
