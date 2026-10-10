BIN = server

all: $(BIN)

$(BIN): main.go go.mod go.sum
	go build -o $(BIN) .

clean:
	$(RM) $(BIN)