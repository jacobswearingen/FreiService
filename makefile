CC      = gcc
CFLAGS  = -std=gnu99 -Wall -Wextra -O2 -Ilib/mongoose -Ilib/cJSON -Ihandlers
LDFLAGS = -pthread -lsqlite3

SRC = main.c lib/mongoose/mongoose.c lib/cJSON/cJSON.c handlers/kjv.c router.c
BIN = server

all: $(BIN)

$(BIN): $(SRC)
	$(CC) $(CFLAGS) $^ -o $@ $(LDFLAGS)

clean:
	$(RM) $(BIN)