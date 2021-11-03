using System;

namespace Protos.Addressbook
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.Error.WriteLine("Usage: AddressBook [file]");
                Console.Error.WriteLine("If the filename is not specified, \"addressbook.data\" is used");
                //return 1;
            }
            string addressBookFile = args.Length > 0 ? args[0] : "addressbook.data";

            bool stopping = false;
            while(!stopping)
            {
                Console.WriteLine("Options:");
                Console.WriteLine("  L: List contents");
                Console.WriteLine("  A: Add new person");
                Console.WriteLine("  Q: Quit");
                Console.Write("Action? ");
                Console.Out.Flush();
                char choice = Console.ReadKey().KeyChar;
                Console.WriteLine();
                try
                {
                    switch(choice)
                    {
                        case 'A':
                        case 'a':
                            WriteMessage.Main(new string[] { addressBookFile });
                            break;
                        case 'l':
                        case 'L':
                            ReadMessage.Main(new string[] { addressBookFile });
                            break;
                        case 'Q':
                        case 'q':
                            stopping = true;
                            break;
                        default:
                            Console.WriteLine("Unknown option: {0}", choice);
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception executing action: {0}", e);
                }
                Console.WriteLine();
            }
            //return 0;
        }
    }
}
