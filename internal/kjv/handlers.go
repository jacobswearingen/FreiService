package kjv

import (
	"database/sql"
	"encoding/json"
	"errors"
	"log"
	"net/http"
)

type verseRequest struct {
	Book    *int `json:"book"`
	Chapter *int `json:"chapter"`
	Verse   *int `json:"verse"`
}

type chapterRequest struct {
	Book    *int `json:"book"`
	Chapter *int `json:"chapter"`
}

type passageRequest struct {
	Book         *int `json:"book"`
	StartChapter *int `json:"start_chapter"`
	StartVerse   *int `json:"start_verse"`
	EndChapter   *int `json:"end_chapter"`
	EndVerse     *int `json:"end_verse"`
}

// Passage describes an inclusive range to fetch. Nil verse bounds include the
// whole boundary chapter, which lets verse, chapter, and passage requests share
// the same query function.
type Passage struct {
	Book         int
	StartChapter int
	StartVerse   *int
	EndChapter   int
	EndVerse     *int
}

func (request verseRequest) validate() error {
	if request.Book == nil || request.Chapter == nil || request.Verse == nil {
		return errors.New("book, chapter, and verse are required")
	}
	return nil
}

func (request chapterRequest) validate() error {
	if request.Book == nil || request.Chapter == nil {
		return errors.New("book and chapter are required")
	}
	return nil
}

func (request passageRequest) validate() error {
	if request.Book == nil || request.StartChapter == nil || request.StartVerse == nil ||
		request.EndChapter == nil || request.EndVerse == nil {
		return errors.New("book and passage boundaries are required")
	}
	return nil
}

type verse struct {
	Chapter int    `json:"chapter,omitempty"`
	Verse   int    `json:"verse"`
	Text    string `json:"text"`
}

type handler struct {
	db *sql.DB
}

// RegisterRoutes creates a handler with the shared database connection and
// attaches each KJV endpoint to the HTTP mux.
func RegisterRoutes(mux *http.ServeMux, db *sql.DB) {
	h := &handler{db: db}
	mux.HandleFunc("/kjv/get_verse", h.getVerse)
	mux.HandleFunc("/kjv/get_chapter", h.getChapter)
	mux.HandleFunc("/kjv/get_passage", h.handlePassage)
}

// decodeAndValidate parses the request, runs its validation, and sends the
// common bad-request response if either step fails.
func decodeAndValidate(w http.ResponseWriter, r *http.Request, destination any, validate func() error, expected string) bool {
	if err := json.NewDecoder(r.Body).Decode(destination); err != nil {
		http.Error(w, "Invalid JSON: expected "+expected, http.StatusBadRequest)
		return false
	}
	if err := validate(); err != nil {
		http.Error(w, "Invalid JSON: expected "+expected, http.StatusBadRequest)
		return false
	}
	return true
}

// writeJSON sets the response content type and encodes the value as JSON.
func writeJSON(w http.ResponseWriter, value any) {
	w.Header().Set("Content-Type", "application/json")
	if err := json.NewEncoder(w).Encode(value); err != nil {
		log.Printf("write JSON response: %v", err)
	}
}

func writeNotFound(w http.ResponseWriter, message string, err error) {
	if err != nil && !errors.Is(err, sql.ErrNoRows) {
		log.Printf("%s: %v", message, err)
	}
	http.Error(w, message, http.StatusNotFound)
}

func scanVerses(rows *sql.Rows, includeChapter bool) ([]verse, error) {
	defer rows.Close()

	verses := make([]verse, 0)
	for rows.Next() {
		var item verse
		var err error
		if includeChapter {
			err = rows.Scan(&item.Chapter, &item.Verse, &item.Text)
		} else {
			err = rows.Scan(&item.Verse, &item.Text)
		}
		if err != nil {
			return nil, err
		}
		verses = append(verses, item)
	}
	if err := rows.Err(); err != nil {
		return nil, err
	}
	if len(verses) == 0 {
		return nil, sql.ErrNoRows
	}
	return verses, nil
}

func (h *handler) getPassage(r *http.Request, passage Passage) ([]verse, error) {
	rows, err := h.db.QueryContext(r.Context(), `
		SELECT chapter, verse, text FROM kjv WHERE book=?
		AND (chapter > ? OR (chapter = ? AND (? IS NULL OR verse >= ?)))
		AND (chapter < ? OR (chapter = ? AND (? IS NULL OR verse <= ?)))
		ORDER BY chapter ASC, verse ASC`,
		passage.Book,
		passage.StartChapter, passage.StartChapter, passage.StartVerse, passage.StartVerse,
		passage.EndChapter, passage.EndChapter, passage.EndVerse, passage.EndVerse,
	)
	if err != nil {
		return nil, err
	}
	return scanVerses(rows, true)
}

// getVerse parses the requested reference, fetches one verse, and returns its
// reference and text as JSON.
func (h *handler) getVerse(w http.ResponseWriter, r *http.Request) {
	var request verseRequest
	if !decodeAndValidate(w, r, &request, request.validate, "book, chapter, verse") {
		return
	}

	verseNumber := *request.Verse
	passage := Passage{
		Book: *request.Book, StartChapter: *request.Chapter, StartVerse: &verseNumber,
		EndChapter: *request.Chapter, EndVerse: &verseNumber,
	}
	verses, err := h.getPassage(r, passage)
	if err != nil {
		writeNotFound(w, "Verse not found", err)
		return
	}
	item := verses[0]
	writeJSON(w, map[string]any{
		"book": *request.Book, "chapter": *request.Chapter, "verse": item.Verse, "text": item.Text,
	})
}

// getChapter parses the book and chapter, fetches verses in order, and returns
// them as a JSON chapter response.
func (h *handler) getChapter(w http.ResponseWriter, r *http.Request) {
	var request chapterRequest
	if !decodeAndValidate(w, r, &request, request.validate, "book, chapter") {
		return
	}

	passage := Passage{Book: *request.Book, StartChapter: *request.Chapter, EndChapter: *request.Chapter}
	verses, err := h.getPassage(r, passage)
	if err != nil {
		writeNotFound(w, "Chapter not found", err)
		return
	}
	for i := range verses {
		verses[i].Chapter = 0
	}
	writeJSON(w, map[string]any{"book": *request.Book, "chapter": *request.Chapter, "verses": verses})
}

// getPassage parses the inclusive start and end references, fetches matching
// verses in order, and returns the reference range and verses as JSON.
func (h *handler) handlePassage(w http.ResponseWriter, r *http.Request) {
	var request passageRequest
	if !decodeAndValidate(w, r, &request, request.validate, "book, start_chapter, start_verse, end_chapter, end_verse") {
		return
	}

	passage := Passage{
		Book: *request.Book, StartChapter: *request.StartChapter, StartVerse: request.StartVerse,
		EndChapter: *request.EndChapter, EndVerse: request.EndVerse,
	}
	verses, err := h.getPassage(r, passage)
	if err != nil {
		writeNotFound(w, "Passage not found", err)
		return
	}
	writeJSON(w, map[string]any{
		"book": *request.Book, "start_chapter": *request.StartChapter, "start_verse": *request.StartVerse,
		"end_chapter": *request.EndChapter, "end_verse": *request.EndVerse, "verses": verses,
	})
}
