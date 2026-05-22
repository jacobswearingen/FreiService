#include "mongoose.h"
#include "kjv.h"
#include <sqlite3.h>
#include <stdio.h>
#include "cJSON.h"

static int prepare_stmt(sqlite3 *db, sqlite3_stmt **stmt, const char *sql) {
    int rc = sqlite3_prepare_v2(db, sql, -1, stmt, 0);
    if (rc != SQLITE_OK) {
        fprintf(stderr, "sqlite3_prepare_v2 failed: %s\n", sqlite3_errmsg(db));
    }
    return rc;
}

static int is_post_request(const struct mg_http_message *hm) {
    return mg_strcmp(hm->method, mg_str("POST")) == 0;
}

static int is_valid_reference(int book, int chapter, int verse) {
    return book >= 1 && book <= 66 && chapter >= 1 && verse >= 1;
}


// Returns a malloc'd JSON string for the verse, or NULL if not found or error. Caller must free.
char *query_verse_json(sqlite3 *db, int book, int chapter, int verse) {
    static const char *sql =
        "SELECT text FROM kjv WHERE book=? AND chapter=? AND verse=?";
    sqlite3_stmt *stmt = NULL;
    char *json_str = NULL;
    int rc;

    rc = prepare_stmt(db, &stmt, sql);
    if (rc != SQLITE_OK) goto cleanup;

    sqlite3_bind_int(stmt, 1, book);
    sqlite3_bind_int(stmt, 2, chapter);
    sqlite3_bind_int(stmt, 3, verse);

    if (sqlite3_step(stmt) == SQLITE_ROW) {
        const unsigned char *text = sqlite3_column_text(stmt, 0);
        cJSON *root = cJSON_CreateObject();
        if (root) {
            cJSON_AddNumberToObject(root, "book", book);
            cJSON_AddNumberToObject(root, "chapter", chapter);
            cJSON_AddNumberToObject(root, "verse", verse);
            cJSON_AddStringToObject(root, "text", (const char *)text);
            json_str = cJSON_PrintUnformatted(root);
            cJSON_Delete(root);
        }
    }

cleanup:
    if (stmt) sqlite3_finalize(stmt);
    return json_str;
}

void get_verse(struct mg_connection *c, struct mg_http_message *hm, sqlite3 *db) {
    if (!is_post_request(hm)) {
        mg_http_reply(c, 405, "Allow: POST\r\n", "Method not allowed\n");
        return;
    }

    // Parse JSON body: expect {"book":1, "chapter":1, "verse":1}
    double dbook = 0, dchapter = 0, dverse = 0;
    if (!mg_json_get_num(hm->body, "$.book", &dbook) ||
        !mg_json_get_num(hm->body, "$.chapter", &dchapter) ||
        !mg_json_get_num(hm->body, "$.verse", &dverse)) {
        mg_http_reply(c, 400, "", "Invalid JSON: expected book, chapter, verse\n");
        return;
    }
    int book = (int)dbook, chapter = (int)dchapter, verse = (int)dverse;
    if (!is_valid_reference(book, chapter, verse)) {
        mg_http_reply(c, 400, "", "Invalid reference range\n");
        return;
    }

    char *json = query_verse_json(db, book, chapter, verse);
    if (json) {
        mg_http_reply(c, 200, "Content-Type: application/json\r\n", "%s", json);
        free(json);
    } else {
        mg_http_reply(c, 404, "", "Verse not found\n");
    }
}

char *query_chapter_json(sqlite3 *db, int book, int chapter) {
    static const char *sql =
        "SELECT verse, text FROM kjv WHERE book=? AND chapter=? ORDER BY verse ASC";
    sqlite3_stmt *stmt = NULL;
    struct mg_iobuf buf = {0};
    char *result = NULL;
    int count = 0;

    if (prepare_stmt(db, &stmt, sql) != SQLITE_OK) goto cleanup;
    sqlite3_bind_int(stmt, 1, book);
    sqlite3_bind_int(stmt, 2, chapter);

    while (sqlite3_step(stmt) == SQLITE_ROW) {
        int verse = sqlite3_column_int(stmt, 0);
        const char *text = (const char *)sqlite3_column_text(stmt, 1);
        char *piece = mg_mprintf("%s{%m:%d,%m:%m}",
            count++ > 0 ? "," : "",
            MG_ESC("verse"), verse,
            MG_ESC("text"), MG_ESC(text));
        mg_iobuf_add(&buf, buf.len, piece, strlen(piece));
        free(piece);
    }

    if (count > 0)
        result = mg_mprintf("{%m:%d,%m:%d,%m:[%.*s]}",
            MG_ESC("book"),    book,
            MG_ESC("chapter"), chapter,
            MG_ESC("verses"),  (int)buf.len, buf.buf);

cleanup:
    if (stmt) sqlite3_finalize(stmt);
    mg_iobuf_free(&buf);
    return result;
}

void get_chapter(struct mg_connection *c, struct mg_http_message *hm, sqlite3 *db) {
    if (!is_post_request(hm)) {
        mg_http_reply(c, 405, "Allow: POST\r\n", "Method not allowed\n");
        return;
    }

    double dbook = 0, dchapter = 0;
    if (!mg_json_get_num(hm->body, "$.book", &dbook) ||
        !mg_json_get_num(hm->body, "$.chapter", &dchapter)) {
        mg_http_reply(c, 400, "", "Invalid JSON: expected book, chapter\n");
        return;
    }
    int book = (int)dbook, chapter = (int)dchapter;
    if (!is_valid_reference(book, chapter, 1)) {
        mg_http_reply(c, 400, "", "Invalid reference range\n");
        return;
    }
    char *json = query_chapter_json(db, book, chapter);
    if (json) {
        mg_http_reply(c, 200, "Content-Type: application/json\r\n", "%s", json);
        free(json);
    } else {
        mg_http_reply(c, 404, "", "Chapter not found\n");
    }
}


// Returns a malloc'd JSON string for the passage, or NULL if not found or error. Caller must free.
char *query_passage_json(sqlite3 *db, int book, int start_chapter, int start_verse, int end_chapter, int end_verse) {
    static const char *sql =
        "SELECT chapter, verse, text FROM kjv WHERE book=? AND ((chapter > ? OR (chapter = ? AND verse >= ?)) AND (chapter < ? OR (chapter = ? AND verse <= ?))) ORDER BY chapter ASC, verse ASC";
    sqlite3_stmt *stmt = NULL;
    struct mg_iobuf buf = {0};
    mg_iobuf_resize(&buf, 8192);
    char *result = NULL;
    int count = 0;

    if (prepare_stmt(db, &stmt, sql) != SQLITE_OK) goto cleanup;

    sqlite3_bind_int(stmt, 1, book);
    sqlite3_bind_int(stmt, 2, start_chapter);
    sqlite3_bind_int(stmt, 3, start_chapter);
    sqlite3_bind_int(stmt, 4, start_verse);
    sqlite3_bind_int(stmt, 5, end_chapter);
    sqlite3_bind_int(stmt, 6, end_chapter);
    sqlite3_bind_int(stmt, 7, end_verse);

    while (sqlite3_step(stmt) == SQLITE_ROW) {
        int chapter = sqlite3_column_int(stmt, 0);
        int verse = sqlite3_column_int(stmt, 1);
        const unsigned char *text = sqlite3_column_text(stmt, 2);
        char *piece = mg_mprintf("%s{%m:%d,%m:%d,%m:%m}",
            count++ > 0 ? "," : "",
            MG_ESC("chapter"), chapter,
            MG_ESC("verse"), verse,
            MG_ESC("text"), MG_ESC(text));
        mg_iobuf_add(&buf, buf.len, piece, strlen(piece));
        free(piece);
    }   
    if (count > 0)
        result = mg_mprintf("{%m:%d,%m:%d,%m:[%.*s]}",
            MG_ESC("book"),    book,
            MG_ESC("chapter"), start_chapter,
            MG_ESC("verses"),  (int)buf.len, buf.buf);

cleanup:
    if (stmt) sqlite3_finalize(stmt);
    mg_iobuf_free(&buf);
    return result;
}

void get_passage(struct mg_connection *c, struct mg_http_message *hm, sqlite3 *db) {
    if (!is_post_request(hm)) {
        mg_http_reply(c, 405, "Allow: POST\r\n", "Method not allowed\n");
        return;
    }

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
    if (!is_valid_reference(book, start_chapter, start_verse) ||
        !is_valid_reference(book, end_chapter, end_verse) ||
        start_chapter > end_chapter ||
        (start_chapter == end_chapter && start_verse > end_verse)) {
        mg_http_reply(c, 400, "", "Invalid reference range\n");
        return;
    }

    char *json = query_passage_json(db, book, start_chapter, start_verse, end_chapter, end_verse);
    if (json) {
        mg_http_reply(c, 200, "Content-Type: application/json\r\n", "%s", json);
        free(json);
    } else {
        mg_http_reply(c, 404, "", "Passage not found\n");
    }
}
