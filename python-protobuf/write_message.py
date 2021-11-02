#! /usr/bin/python

from build import addressbook_pb2
import sys

# Fill in a Person message based on user input
def PromptForAddress(person):
    person.id = int(input("Enter person ID number: "))
    person.name = input("Enter name: ")

    email = input("Enter email address (blank for none): ")
    if email != "":
        person.email = email

    while True:
        number = input("Enter a phone number (or leave blank to finish): ")
        if number == "":
            break

        phone_number = person.phones.add()
        phone_number.number = number

        type = input("Is this a mobile, home or work phone? ")
        if type == "mobile":
            phone_number.type = addressbook_pb2.Person.PhoneType.MOBILE
        elif type == "home":
            phone_number.type = addressbook_pb2.Person.PhoneType.HOME
        elif type == "work":
            phone_number.type = addressbook_pb2.Person.PhoneType.WORK
        else:
            print("Unknown phone type; leaving as default value")

# Main procedure: 
#    Read the entire address book from a file
#    Add one person based on user input, then write it back out to the same file

if len(sys.argv) != 2:
    print("Usage:", sys.argv[0], "ADDRESS_BOOK_FILE")
    sys.exit(-1)

# Create an address book (protobuf message)
address_book = addressbook_pb2.AddressBook()

# Read the existing address book
try:
    with open(sys.argv[1], "rb") as f:
        address_book.ParseFromString(f.read())
except IOError:
    print(sys.argv[1] + ": File not found. Creating a new file.")

# Add an address
PromptForAddress(address_book.people.add())

#Write the new address book back to disk
with open(sys.argv[1], "wb") as f:
    f.write(address_book.SerializeToString())

