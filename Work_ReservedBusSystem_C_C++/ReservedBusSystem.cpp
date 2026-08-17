#include <iostream>
using namespace std;
#include <string>
#include <iomanip>
#include <limits>

struct Seat {
    int seat_no;
    int booked;
    string name;
};

// Ask a valid seat number or 0.
// Return true if a valid number has been entered (stored in "numbersSeat"),
// Else, return false if the user typed 0 to cancel and go back.
bool ask_seat_number(int& numbersSeat, int total) {
    cout << "Number (0 to go back) : ";
    cin >> numbersSeat;

    while (numbersSeat < 0 || numbersSeat > total) {
        cout << "Invalid seat number. Try again (0 to go back): ";
        cin >> numbersSeat;
    }
    return numbersSeat != 0;
}

bool reserve_seat(Seat* seats, int total) {
    int numbersSeat;
    cout << "These seats are available :\n";
    for (int i = 0; i < total; i++) {
        if (seats[i].booked == 0) {
            cout << setw(3) << seats[i].seat_no << " | ";
        }
        else {
            cout << setw(3) << " " << " | ";
        }
        if ((i + 1) % 10 == 0) {
            cout << "\n";
        }
    }
    cout << "\nWhich seat would you like to have?\n";

    if (!ask_seat_number(numbersSeat, total)) {
        cout << "Reservation cancelled.\n";
        return false;
    }

    while (seats[numbersSeat - 1].booked == 1) {
        cout << "Seat already reserved\n";
        if (!ask_seat_number(numbersSeat, total)) {
            cout << "Reservation cancelled.\n";
            return false;
        }
    }

    seats[numbersSeat - 1].booked = 1;
    cout << "\nEnter your name please : ";
    cin.ignore();
    getline(cin, seats[numbersSeat - 1].name);
    return true;
}


// Return the canceled seat number, or 0 if the user cancel the operation.
int cancel_reservation(Seat* seats, int total) {
    int numbersSeat;

    cout << "On which seat were you ?\n";
    if (!ask_seat_number(numbersSeat, total)) {
        cout << "Cancellation aborted.\n";
        return 0;
    }

    if (seats[numbersSeat - 1].booked == 0) {
        cout << "This seat is not reserved.\n";
        return 0;
    }

    // Identity verification
    string enteredName;
    cout << "Enter the name on the reservation to confirm : ";
    cin.ignore();
    getline(cin, enteredName);

    if (enteredName != seats[numbersSeat - 1].name) {
        cout << "Name does not match this reservation. Cancellation refused.\n";
        return 0;
    }

    seats[numbersSeat - 1].booked = 0;
    seats[numbersSeat - 1].name.clear();
    cout << "Reservation cancelled.\n";
    return numbersSeat;
}

void edit_reservation(Seat* seats, int total) {
    int cancelledSeat = cancel_reservation(seats, total);

    if (cancelledSeat == 0) {
        return;
    }

    reserve_seat(seats, total);
}

void display_reservation(Seat* seats, int total) {
    bool any = false;
    for (int i = 0; i < total; i++) {
        if (seats[i].booked == 1) {
            cout << "Seat " << seats[i].seat_no << " - " << seats[i].name << "\n";
            any = true;
        }
    }
    if (!any) {
        cout << "No seat is currently reserved.\n";
    }
}

int main() {
    int busLength = 50;
    Seat* arrseats = new Seat[busLength];

    for (int i = 0; i < busLength; i++) {
        arrseats[i].seat_no = i + 1;
        arrseats[i].booked = 0;
    }

    int choice = 0;
    do {
        cout << "\n===== MENU =====\n";
        cout << "1. Reserve a seat\n";
        cout << "2. Edit reservation\n";
        cout << "3. Cancel reservation\n";
        cout << "4. Display all reserved seats\n";
        cout << "5. Exit\n";

        cout << "Choice: ";
        cin >> choice;

        switch (choice) {
        case 1: reserve_seat(arrseats, busLength); break;
        case 2: edit_reservation(arrseats, busLength); break;
        case 3: cancel_reservation(arrseats, busLength); break;
        case 4: display_reservation(arrseats, busLength); break;
        case 5: break;
        default:
            cout << "Invalid choice.\n";
        }

    } while (choice != 5);

    cout << "Thank you for using C-Bus";

    delete[] arrseats;

    return 0;
}