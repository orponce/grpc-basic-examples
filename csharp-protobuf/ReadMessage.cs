using System;
using System.IO;

using Google.Protobuf;
using static Protos.Addressbook.Person.Types;

namespace Protos.Addressbook
{
    class ReadMessage
    {
        private static void Print(AddressBook addressBook)
        {
            foreach (Person person in addressBook.People)
            {
                Console.WriteLine("Person ID: {0}", person.Id);
                Console.WriteLine("  Name: {0}", person.Name);
                if (person.Email != "")
                {
                    Console.WriteLine("  E-mail address: {0}", person.Email);
                }

                foreach (PhoneNumber phoneNumber in person.Phones)
                {
                    switch (phoneNumber.Type)
                    {
                        case PhoneType.Mobile:
                            Console.Write("  Mobile phone #: ");
                            break;
                        case PhoneType.Home:
                            Console.Write("  Home phone #: ");
                            break;
                        case PhoneType.Work:
                            Console.Write("  Work phone #: ");
                            break;
                    }
                    Console.WriteLine(phoneNumber.Number);
                }
            }
        }

        public static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("Usage: ReadMessage ADDRESS_BOOK_FILE");
                return 1;
            }

            if (!File.Exists(args[0]))
            {
                Console.WriteLine("{0} does not exist. Add a person to create the file first.", args[0]);
                return 0;
            }

            // Read the existing address book
            using (Stream stream = File.OpenRead(args[0]))
            {
                AddressBook addressBook = AddressBook.Parser.ParseFrom(stream);
                Print(addressBook);
            }
            return 0;
        }
    }
}
