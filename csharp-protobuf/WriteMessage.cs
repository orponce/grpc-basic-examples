using System;
using System.IO;

using Google.Protobuf;
using static Proto.Addressbook.Person.Types;

namespace Proto.Addressbook
{
    class WriteMessage
    {
        private static Person PromptForAddress(TextReader input, TextWriter output)
        {
            Person person = new Person();

            output.Write("Enter person ID: ");
            person.Id = int.Parse(input.ReadLine());

            output.Write("Enter name: ");
            person.Name = input.ReadLine();

            output.Write("Enter email address (blank for none): ");
            string email = input.ReadLine();
            if (email.Length > 0)
            {
                person.Email = email;
            }

            while (true)
            {
                output.Write("Enter a phone number (or leave blank to finish): ");
                string number = input.ReadLine();
                if (number.Length == 0)
                {
                    break;
                }

                PhoneNumber phoneNumber = new PhoneNumber { Number = number };

                output.Write("Is this a mobile, home, or work phone? ");
                String type = input.ReadLine();
                switch(type)
                {
                    case "mobile":
                        phoneNumber.Type = PhoneType.Mobile;
                        break;
                    case "home":
                        phoneNumber.Type = PhoneType.Home;
                        break;
                    case "work":
                        phoneNumber.Type = PhoneType.Work;
                        break;
                    default:
                        output.Write("Unknown phone type. Using default.");
                        break;
                }
                person.Phones.Add(phoneNumber);
            }
            return person;
        }

        public static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("Usage: WriteMessage ADDRESS_BOOK_FILE");
                return -1;
            }

            AddressBook addressBook;

            if (File.Exists(args[0]))
            {
                using (Stream file = File.OpenRead(args[0]))
                {
                    addressBook = AddressBook.Parser.ParseFrom(file);
                }
            }
            else
            {
                Console.WriteLine("{0}: File not found. Creating a new file.", args[0]);
                addressBook = new AddressBook();
            }

            // Add an address
            addressBook.People.Add(PromptForAddress(Console.In, Console.Out));

            // Write the new address book back to disk
            using (Stream output = File.OpenWrite(args[0]))
            {
                addressBook.WriteTo(output);
            }
            return 0;
        }
    }
}
