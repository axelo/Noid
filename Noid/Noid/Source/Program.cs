using System;

namespace Noid
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (NodeGame game = new NodeGame())
            {
                game.Run();
            }
        }
    }
#endif
}

