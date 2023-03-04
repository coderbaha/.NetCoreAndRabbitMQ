namespace Entity.Configuration
{
    public class CustomLogProvider
    {
        public LogConf FileLog { get; set; }
    }
    public class LogConf
    {
        public bool Enable { get; set; }
    }
}
