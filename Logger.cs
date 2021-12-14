using System;
using System.Collections.Generic;
using System.IO;

namespace Napilnik
{
    public class Program
    {
        static void Main(string[] args)
        {
            Pathfinder fileLogWritter = new Pathfinder(new FileLogWritter());
            Pathfinder consoleLogWritter = new Pathfinder(new ConsoleLogWritter());
            Pathfinder fridayFileLogWritter = new Pathfinder(new FridayFileLogWritter());
            Pathfinder fridayConsoleLogWritter = new Pathfinder(new FridayConsoleLogWritter());
            Pathfinder combinedLogWritter = new Pathfinder(ChainOfWritters.Create(new ConsoleLogWritter(), new FridayFileLogWritter()));
        }
    }

    class ChainOfWritters : ILogger
    {
        private IEnumerable<ILogger> _writters;

        public ChainOfWritters(IEnumerable<ILogger> writters)
        {
            _writters = writters;
        }

        public void Find(string message)
        {
            foreach (var write in _writters)
            {
                write.Find(message);
            }
        }

        public static ChainOfWritters Create(params ILogger[] writters)
        {
            return new ChainOfWritters(writters);
        }
    }

    class Pathfinder : ILogger
    {
        private ILogger _logger;

        public Pathfinder(ILogger logger)
        {
            _logger = logger;
        }

        public void Find(string message)
        {
            _logger.Find(message);
        }
    }

    class ConsoleLogWritter : ILogger
    {
        public void Find(string message)
        {
            Console.WriteLine(message);
        }
    }

    class FileLogWritter : ILogger
    {
        public void Find(string message)
        {
            File.WriteAllText("log.txt", message);
        }
    }

    class FridayFileLogWritter : ILogger
    {
        public void Find(string message)
        {
            if (DateTime.Now.DayOfWeek == DayOfWeek.Friday)
            {
                File.WriteAllText("log.txt", message);
            }
        }
    }

    class FridayConsoleLogWritter : ILogger
    {
        public void Find(string message)
        {
            if (DateTime.Now.DayOfWeek == DayOfWeek.Friday)
            {
                Console.WriteLine(message);
            }
        }
    }

    public interface ILogger
    {
        public void Find(string message);
    }
}
