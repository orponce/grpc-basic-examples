#! /usr/bin/pythin

from build import addressbook_pb2
import sys

# Iterate through all the people in the AddressBook and print info about them
def ListPeople(address_book):
    for person in address_book.people:
        print("Person ID:", person.id)
        print("  Name:", person.name)
        if person.email != "":
            print("  E-mail address:", person.email)

        for phone_number in person.phones:
            if phone_number.type == addressbook_pb2.Person.PhoneType.MOBILE:
                print("  Mobile phone #: ", end="")
            elif phone_number.type == addressbook_pb2.Person.PhoneType.HOME:
                print("  Home phone #: ", end="")
            elif phone_number.type == addressbook_pb2.Person.PhoneType.WORK:
                print("  Work phone #: ", end="")
            print(phone_number.number)


# Read the entire address book from a file and print all the information inside
def main():
    if len(sys.argv) != 2:
        print("Usage:", sys.argv[0], "ADDRESS_BOOK_FILE")
        sys.exit(-1)

    # Create an instance of the protobuf message
    address_book = addressbook_pb2.AddressBook()

    # Read the existing address book
    with open(sys.argv[1], "rb") as f:
        address_book.ParseFromString(f.read())
    
    ListPeople(address_book)


if __name__ == "__main__":
    main()
