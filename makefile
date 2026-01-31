CC      = gcc
CFLAGS  = -std=gnu99 -Wall -Wextra -O2
LDFLAGS = -pthread -lsqlite3

SRC     = main.c mongoose.c
OBJ     = $(SRC:.c=.o)
BIN     = server

all: $(BIN)

$(BIN): $(OBJ)
	$(CC) $(OBJ) -o $(BIN) $(LDFLAGS)

%.o: %.c mongoose.h
	$(CC) $(CFLAGS) -c $< -o $@

clean:
	$(RM) $(OBJ) $(BIN)