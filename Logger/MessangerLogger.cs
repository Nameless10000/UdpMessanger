namespace Logger
{
    public class MessangerLogger
    {
        private string _path;
        private FileStream _fileStream;
        private StreamReader _reader;
        private StreamWriter _writer;

        public MessangerLogger(string path)
        {
            _path = path;
            _fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            _reader = new StreamReader(_fileStream);
            _writer = new StreamWriter(_fileStream);
        }

        ~MessangerLogger()
        {
            _reader.Close();
            _writer.Close();
            _fileStream.Close();
        }

        public async Task LogErrorAsync(string message)
        {
            var now = DateTime.Now;
            var log = $"[ERROR] {now:F} - {message}\n";
            await _writer.WriteAsync(log);
        }
    }
}