CC      = gcc
CFLAGS  = -std=gnu99 -Wall -Wextra -O2
LDFLAGS = -pthread -lsqlite3

SRC = main.c lib/mongoose/mongoose.c \
	handlers/kjv.c \
	router.c
OBJ = $(SRC:.c=.o)
BIN = server

all: $(BIN)

$(BIN): $(OBJ)
	$(CC) $(OBJ) -o $(BIN) $(LDFLAGS)

%.o: %.c
	$(CC) $(CFLAGS) -Ilib/mongoose -Ihandlers -c $< -o $@

clean:
	$(RM) $(OBJ) $(BIN)