#include <string.h>
#include "mongoose.h"


#include "kjv.h"
#include <sqlite3.h>

#define DB_PATH "db.db"


// Returns a malloc'd JSON string for the verse, or NULL if not found or error. Caller must free.
char *query_verse_json(int book, int chapter, int verse) {
    char sql[256];
    sqlite3 *db;
    sqlite3_stmt *stmt;
    int rc;
    char *result = NULL;
    rc = sqlite3_open(DB_PATH, &db);
    if (rc != SQLITE_OK) return NULL;
    mg_snprintf(sql, sizeof(sql),
        "SELECT text FROM kjv WHERE book=? AND chapter=? AND verse=?");
    rc = sqlite3_prepare_v2(db, sql, -1, &stmt, 0);
    if (rc != SQLITE_OK) {
        sqlite3_close(db);
        return NULL;
    }
    sqlite3_bind_int(stmt, 1, book);
    sqlite3_bind_int(stmt, 2, chapter);
    sqlite3_bind_int(stmt, 3, verse);
    if (sqlite3_step(stmt) == SQLITE_ROW) {
        const unsigned char *text = sqlite3_column_text(stmt, 0);
        result = mg_mprintf("{ \"book\": %d, \"chapter\": %d, \"verse\": %d, \"text\": %m }\n",
            book, chapter, verse, MG_ESC((const char *)text));
    }
    sqlite3_finalize(stmt);
    sqlite3_close(db);
    return result;
}

void get_verse(struct mg_connection *c, struct mg_http_message *hm) {
    // Parse JSON body: expect {"book":1, "chapter":1, "verse":1}
    double dbook = 0, dchapter = 0, dverse = 0;
    if (!mg_json_get_num(hm->body, "$.book", &dbook) ||
        !mg_json_get_num(hm->body, "$.chapter", &dchapter) ||
        !mg_json_get_num(hm->body, "$.verse", &dverse)) {
        mg_http_reply(c, 400, "", "Invalid JSON: expected book, chapter, verse\n");
        return;
    }
    int book = (int)dbook, chapter = (int)dchapter, verse = (int)dverse;
    char *json = query_verse_json(book, chapter, verse);
    if (json) {
        mg_http_reply(c, 200, "Content-Type: application/json\r\n", "%s", json);
        free(json);
    } else {
        mg_http_reply(c, 404, "", "Verse not found\n");
    }
}


// Returns a malloc'd JSON string for the chapter, or NULL if not found or error. Caller must free.
char *query_chapter_json(int book, int chapter) {
    char sql[256];
    sqlite3 *db;
    sqlite3_stmt *stmt;
    int rc;
    char *json = NULL, *tmp = NULL;
    int first = 1, error = 0;
    rc = sqlite3_open(DB_PATH, &db);
    if (rc != SQLITE_OK) return NULL;
    mg_snprintf(sql, sizeof(sql),
        "SELECT verse, text FROM kjv WHERE book=? AND chapter=? ORDER BY verse ASC");
    rc = sqlite3_prepare_v2(db, sql, -1, &stmt, 0);
    if (rc != SQLITE_OK) {
        sqlite3_close(db);
        return NULL;
    }
    sqlite3_bind_int(stmt, 1, book);
    sqlite3_bind_int(stmt, 2, chapter);
    json = mg_mprintf("{ \"book\": %d, \"chapter\": %d, \"verses\": [", book, chapter);
    if (!json) error = 1;
    while (!error && sqlite3_step(stmt) == SQLITE_ROW) {
        int verse = sqlite3_column_int(stmt, 0);
        const unsigned char *text = sqlite3_column_text(stmt, 1);
        tmp = mg_mprintf(first ? "%s{ \"verse\": %d, \"text\": %m }"
                                 : "%s, { \"verse\": %d, \"text\": %m }",
                        json, verse, MG_ESC((const char *)text));
        free(json);
        if (!tmp) error = 1;
        json = tmp;
        first = 0;
    }
    char *final_json = NULL;
    if (!error) final_json = mg_mprintf("%s]}\n", json);
    free(json);
    sqlite3_finalize(stmt);
    sqlite3_close(db);
    if (error && final_json) { free(final_json); final_json = NULL; }
    if (first && final_json) { free(final_json); final_json = NULL; }
    return final_json;
}

void get_chapter(struct mg_connection *c, struct mg_http_message *hm) {
    // Parse JSON body: expect {"book":1, "chapter":1}
    double dbook = 0, dchapter = 0;
    if (!mg_json_get_num(hm->body, "$.book", &dbook) ||
        !mg_json_get_num(hm->body, "$.chapter", &dchapter)) {
        mg_http_reply(c, 400, "", "Invalid JSON: expected book, chapter\n");
        return;
    }
    int book = (int)dbook, chapter = (int)dchapter;
    char *json = query_chapter_json(book, chapter);
    if (json) {
        mg_http_reply(c, 200, "Content-Type: application/json\r\n", "%s", json);
        free(json);
    } else {
        mg_http_reply(c, 404, "", "Chapter not found\n");
    }
}


// Returns a malloc'd JSON string for the passage, or NULL if not found or error. Caller must free.
char *query_passage_json(int book, int start_chapter, int start_verse, int end_chapter, int end_verse) {
    char sql[512];
    sqlite3 *db;
    sqlite3_stmt *stmt;
    int rc;
    char *json = NULL, *tmp = NULL;
    int first = 1, error = 0;
    rc = sqlite3_open(DB_PATH, &db);
    if (rc != SQLITE_OK) return NULL;
    mg_snprintf(sql, sizeof(sql),
        "SELECT chapter, verse, text FROM kjv WHERE book=? AND ((chapter > ? OR (chapter = ? AND verse >= ?)) AND (chapter < ? OR (chapter = ? AND verse <= ?))) ORDER BY chapter ASC, verse ASC");
    rc = sqlite3_prepare_v2(db, sql, -1, &stmt, 0);
    if (rc != SQLITE_OK) {
        sqlite3_close(db);
        return NULL;
    }
    sqlite3_bind_int(stmt, 1, book);
    sqlite3_bind_int(stmt, 2, start_chapter);
    sqlite3_bind_int(stmt, 3, start_chapter);
    sqlite3_bind_int(stmt, 4, start_verse);
    sqlite3_bind_int(stmt, 5, end_chapter);
    sqlite3_bind_int(stmt, 6, end_chapter);
    sqlite3_bind_int(stmt, 7, end_verse);
    json = mg_mprintf("{ \"book\": %d, \"start_chapter\": %d, \"start_verse\": %d, \"end_chapter\": %d, \"end_verse\": %d, \"verses\": [", book, start_chapter, start_verse, end_chapter, end_verse);
    if (!json) error = 1;
    while (!error && sqlite3_step(stmt) == SQLITE_ROW) {
        int chapter = sqlite3_column_int(stmt, 0);
        int verse = sqlite3_column_int(stmt, 1);
        const unsigned char *text = sqlite3_column_text(stmt, 2);
        tmp = mg_mprintf(first ? "%s{ \"chapter\": %d, \"verse\": %d, \"text\": %m }"
                                 : "%s, { \"chapter\": %d, \"verse\": %d, \"text\": %m }",
                        json, chapter, verse, MG_ESC((const char *)text));
        free(json);
        if (!tmp) error = 1;
        json = tmp;
        first = 0;
    }
    char *final_json = NULL;
    if (!error) final_json = mg_mprintf("%s]}\n", json);
    free(json);
    sqlite3_finalize(stmt);
    sqlite3_close(db);
    if (error && final_json) { free(final_json); final_json = NULL; }
    if (first && final_json) { free(final_json); final_json = NULL; }
    return final_json;
}

void get_passage(struct mg_connection *c, struct mg_http_message *hm) {
    // Parse JSON body: expect {"book":1, "start_chapter":1, "start_verse":1, "end_chapter":1, "end_verse":1}
    double dbook = 0, dstart_ch = 0, dstart_vs = 0, dend_ch = 0, dend_vs = 0;
    if (!mg_json_get_num(hm->body, "$.book", &dbook) ||
        !mg_json_get_num(hm->body, "$.start_chapter", &dstart_ch) ||
        !mg_json_get_num(hm->body, "$.start_verse", &dstart_vs) ||
        !mg_json_get_num(hm->body, "$.end_chapter", &dend_ch) ||
        !mg_json_get_num(hm->body, "$.end_verse", &dend_vs)) {
        mg_http_reply(c, 400, "", "Invalid JSON: expected book, start_chapter, start_verse, end_chapter, end_verse\n");
        return;
    }
    int book = (int)dbook, start_chapter = (int)dstart_ch, start_verse = (int)dstart_vs, end_chapter = (int)dend_ch, end_verse = (int)dend_vs;
    char *json = query_passage_json(book, start_chapter, start_verse, end_chapter, end_verse);
    if (json) {
        mg_http_reply(c, 200, "Content-Type: application/json\r\n", "%s", json);
        free(json);
    } else {
        mg_http_reply(c, 404, "", "Passage not found\n");
    }
}
