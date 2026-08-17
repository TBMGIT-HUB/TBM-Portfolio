#include <stdio.h>
#include <stdlib.h>
#include <string.h>

typedef struct {
    int seat_no;
    int booked;
    char name[50];
} Seat;

// Ask for a valid seat number, or 0 to cancel and go back.
// Return 1 if a valid number has been entered (stored in *numbersSeat),
// 0 if the user typed 0 to cancel.
int ask_seat_number(int* numbersSeat, int total) {
    printf("Number (0 to go back) : ");
    scanf("%d", numbersSeat);

    while (*numbersSeat < 0 || *numbersSeat > total) {
        printf("Invalid seat number. Try again (0 to go back): ");
        scanf("%d", numbersSeat);
    }
    return *numbersSeat != 0;
}

int reserve_seat(Seat* seats, int total) {
    int numbersSeat;
    printf("These seats are available :\n");
    for (int i = 0; i < total; i++) {
        if (seats[i].booked == 0) {
            printf("%3d | ", seats[i].seat_no);
        }
        else {
            printf("%3s | ", "");
        }
        if ((i + 1) % 10 == 0) {
            printf("\n");
        }
    }
    printf("\nWhich seat would you like to have?\n");

    if (!ask_seat_number(&numbersSeat, total)) {
        printf("Reservation cancelled.\n");
        return 0;
    }
    while (seats[numbersSeat - 1].booked == 1) {
        printf("Seat already reserved\n");
        if (!ask_seat_number(&numbersSeat, total)) {
            printf("Reservation cancelled.\n");
            return 0;
        }
    }
    seats[numbersSeat - 1].booked = 1;
    printf("\nEnter your name please : ");
    getchar(); fgets(seats[numbersSeat - 1].name, 50, stdin);
    seats[numbersSeat - 1].name[strcspn(seats[numbersSeat - 1].name, "\n")] = '\0';
    return 1;
}

int cancel_reservation(Seat* seats, int total) {
    int numbersSeat;
    printf("On which seat were you ?\n");
    if (!ask_seat_number(&numbersSeat, total)) {
        printf("Cancellation aborted.\n");
        return 0;
    }
    if (seats[numbersSeat - 1].booked == 0) {
        printf("This seat is not reserved.\n");
        return 0;
    }
    char enteredName[50];


    printf("Enter the name on the reservation to confirm : ");
    getchar();
    fgets(enteredName, 50, stdin);
    enteredName[strcspn(enteredName, "\n")] = '\0';
    if (strcmp(enteredName, seats[numbersSeat - 1].name) != 0) {
        printf("Name does not match this reservation. Cancellation refused.\n");
        return 0;
    }

    seats[numbersSeat - 1].booked = 0;
    seats[numbersSeat - 1].name[0] = '\0';
    printf("Reservation cancelled.\n");
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
    int any = 0;
    for (int i = 0; i < total; i++) {
        if (seats[i].booked == 1) {
            printf("Seat %d - %s\n", seats[i].seat_no, seats[i].name);
            any = 1;
        }
    }
    if (!any) {
        printf("No seat is currently reserved.\n");
    }
}

int main(void) {


    int busLength = 50;
    Seat* arrseats = (Seat*)malloc(busLength * sizeof(Seat));


    for (int i = 0; i < busLength; i++) {
        arrseats[i].seat_no = i + 1;
        arrseats[i].booked = 0;
        arrseats[i].name[0] = '\0';
    }

    int choice = 0;
    do {
        printf("\n===== MENU =====\n");
        printf("1. Reserve a seat\n");
        printf("2. Edit reservation\n");
        printf("3. Cancel reservation\n");
        printf("4. Display all seats\n");
        printf("5. Exit\n");
        printf("Choice: ");
        scanf("%d", &choice);



        switch (choice) {
        case 1: 
            reserve_seat(arrseats, busLength); break;
        case 2:
            edit_reservation(arrseats, busLength); break;
        case 3: 
            cancel_reservation(arrseats, busLength); break;
        case 4: 
            display_reservation(arrseats, busLength); break;
        case 5: 
            break;
        default: 
            printf("Invalid choice.\n");
        }

    }while (choice != 5);
    printf("Thank you for using C-Bus");
    free(arrseats);

    return 0;
}