package main

import (
	"database/sql"
	"log"
	"net/http"
	"os"

	"freiservice/internal/kjv"
	_ "github.com/mattn/go-sqlite3"
)

func main() {
	if err := run(); err != nil {
		log.Fatal(err)
	}
}

// run opens the database, keeps it open for the server lifetime, and starts
// serving HTTP requests.
func run() error {
	db, err := openDB()
	if err != nil {
		return err
	}
	defer db.Close()

	return serve(":8000", db)
}

// openDB chooses the database path, opens the SQLite connection, and verifies
// that the database can be reached.
func openDB() (*sql.DB, error) {
	dbPath := os.Getenv("DB_PATH")
	if dbPath == "" {
		dbPath = "db.db"
	}

	db, err := sql.Open("sqlite3", dbPath)
	if err != nil {
		return nil, err
	}
	if err := db.Ping(); err != nil {
		db.Close()
		return nil, err
	}
	return db, nil
}

// serve registers the resource routes and starts the HTTP server at addr.
func serve(addr string, db *sql.DB) error {
	mux := http.NewServeMux()
	kjv.RegisterRoutes(mux, db)

	log.Printf("Server started on http://localhost%s", addr)
	return http.ListenAndServe(addr, mux)
}
