using SteelkiltSharp.Core;
using SteelkiltSharp.Modules;

namespace SteelkiltSharp.Examples.Cli;

class Program
{
    static OutputPublisher publisher = new OutputPublisher();

    static void Main(string[] args)
    {
        // Subscribe a custom logger
        //publisher.Subscribe(msg => System.Diagnostics.Debug.WriteLine($"[DEBUG] {msg}"));

        // Subscribe to a file (simplified example)
        //publisher.Subscribe(msg => System.IO.File.AppendAllText("log.txt", msg + "\n"));

        // Subscribe standard console output
        publisher.Subscribe(Console.WriteLine);

        ExampleEngine.Run(publisher);

        publisher.Flush();
    }
}
