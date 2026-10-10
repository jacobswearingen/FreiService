package main

import (
	"database/sql"
	"encoding/json"
	"fmt"
	"log"
	"net/http"
	"os"
	"strconv"

	_ "github.com/mattn/go-sqlite3"
)

type verse struct {
	Chapter int    `json:"chapter,omitempty"`
	Verse   int    `json:"verse"`
	Text    string `json:"text"`
}

type handler struct {
	db *sql.DB
}

func main() {
	dbPath := os.Getenv("DB_PATH")
	if dbPath == "" {
		dbPath = "db.db"
	}

	db, err := sql.Open("sqlite3", dbPath)
	if err != nil {
		log.Fatal(err)
	}
	defer db.Close()
	if err := db.Ping(); err != nil {
		log.Fatal(err)
	}

	mux := http.NewServeMux()
	mux.Handle("/kjv/", &handler{db: db})

	addr := ":8000"
	fmt.Printf("Server started on http://localhost%s\n", addr)
	log.Fatal(http.ListenAndServe(addr, mux))
}

func (h *handler) ServeHTTP(w http.ResponseWriter, r *http.Request) {
	switch r.URL.Path {
	case "/kjv/get_verse":
		h.getVerse(w, r)
	case "/kjv/get_chapter":
		h.getChapter(w, r)
	case "/kjv/get_passage":
		h.getPassage(w, r)
	default:
		http.Error(w, "Not found", http.StatusNotFound)
	}
}

func readNumbers(r *http.Request, fields ...string) (map[string]int, error) {
	var body map[string]json.RawMessage
	decoder := json.NewDecoder(r.Body)
	if err := decoder.Decode(&body); err != nil || body == nil {
		return nil, fmt.Errorf("invalid JSON")
	}

	values := make(map[string]int, len(fields))
	for _, field := range fields {
		raw, ok := body[field]
		if !ok {
			return nil, fmt.Errorf("missing %s", field)
		}
		var number json.Number
		if err := json.Unmarshal(raw, &number); err != nil {
			return nil, fmt.Errorf("invalid %s", field)
		}
		value, err := strconv.ParseFloat(number.String(), 64)
		if err != nil {
			return nil, fmt.Errorf("invalid %s", field)
		}
		values[field] = int(value)
	}
	return values, nil
}

func badRequest(w http.ResponseWriter, expected string) {
	http.Error(w, "Invalid JSON: expected "+expected, http.StatusBadRequest)
}

func writeJSON(w http.ResponseWriter, value any) {
	w.Header().Set("Content-Type", "application/json")
	if err := json.NewEncoder(w).Encode(value); err != nil {
		log.Printf("write JSON response: %v", err)
	}
}

func (h *handler) getVerse(w http.ResponseWriter, r *http.Request) {
	v, err := readNumbers(r, "book", "chapter", "verse")
	if err != nil {
		badRequest(w, "book, chapter, verse")
		return
	}

	var text string
	err = h.db.QueryRow(
		"SELECT text FROM kjv WHERE book=? AND chapter=? AND verse=?",
		v["book"], v["chapter"], v["verse"],
	).Scan(&text)
	if err != nil {
		http.Error(w, "Verse not found", http.StatusNotFound)
		return
	}
	writeJSON(w, map[string]any{
		"book": v["book"], "chapter": v["chapter"], "verse": v["verse"], "text": text,
	})
}

func (h *handler) getChapter(w http.ResponseWriter, r *http.Request) {
	v, err := readNumbers(r, "book", "chapter")
	if err != nil {
		badRequest(w, "book, chapter")
		return
	}

	rows, err := h.db.Query(
		"SELECT verse, text FROM kjv WHERE book=? AND chapter=? ORDER BY verse ASC",
		v["book"], v["chapter"],
	)
	if err != nil {
		http.Error(w, "Chapter not found", http.StatusNotFound)
		return
	}
	defer rows.Close()

	verses := make([]verse, 0)
	for rows.Next() {
		var item verse
		if err := rows.Scan(&item.Verse, &item.Text); err != nil {
			http.Error(w, "Chapter not found", http.StatusNotFound)
			return
		}
		verses = append(verses, item)
	}
	if err := rows.Err(); err != nil || len(verses) == 0 {
		http.Error(w, "Chapter not found", http.StatusNotFound)
		return
	}
	writeJSON(w, map[string]any{"book": v["book"], "chapter": v["chapter"], "verses": verses})
}

func (h *handler) getPassage(w http.ResponseWriter, r *http.Request) {
	fields := []string{"book", "start_chapter", "start_verse", "end_chapter", "end_verse"}
	v, err := readNumbers(r, fields...)
	if err != nil {
		badRequest(w, "book, start_chapter, start_verse, end_chapter, end_verse")
		return
	}

	rows, err := h.db.Query(`
		SELECT chapter, verse, text FROM kjv WHERE book=?
		AND ((chapter > ? OR (chapter = ? AND verse >= ?))
		AND (chapter < ? OR (chapter = ? AND verse <= ?)))
		ORDER BY chapter ASC, verse ASC`,
		v["book"], v["start_chapter"], v["start_chapter"], v["start_verse"],
		v["end_chapter"], v["end_chapter"], v["end_verse"],
	)
	if err != nil {
		http.Error(w, "Passage not found", http.StatusNotFound)
		return
	}
	defer rows.Close()

	verses := make([]verse, 0)
	for rows.Next() {
		var item verse
		if err := rows.Scan(&item.Chapter, &item.Verse, &item.Text); err != nil {
			http.Error(w, "Passage not found", http.StatusNotFound)
			return
		}
		verses = append(verses, item)
	}
	if err := rows.Err(); err != nil || len(verses) == 0 {
		http.Error(w, "Passage not found", http.StatusNotFound)
		return
	}
	writeJSON(w, map[string]any{
		"book": v["book"], "start_chapter": v["start_chapter"], "start_verse": v["start_verse"],
		"end_chapter": v["end_chapter"], "end_verse": v["end_verse"], "verses": verses,
	})
}