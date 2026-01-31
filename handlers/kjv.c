#include <string.h>
#include "mongoose.h"

// Helper to escape JSON special characters using mg_snprintf
void json_escape(const char *src, char *dest, size_t dest_size) {
    size_t di = 0;
    for (size_t si = 0; src[si] && di + 2 < dest_size; si++) {
        char c = src[si];
        if (c == '"' || c == '\\') {
            if (di + 2 >= dest_size) break;
            dest[di++] = '\\';
            dest[di++] = c;
        } else if (c >= 0 && c < 0x20) {
            if (di + 7 >= dest_size) break;
            di += mg_snprintf(dest + di, dest_size - di, "\\u%04x", c);
        } else {
            dest[di++] = c;
        }
    }
    dest[di] = 0;
}
#include "kjv.h"
#include <sqlite3.h>
#include <stdio.h>

#define DB_PATH "db.db"

void get_verse(struct mg_connection *c, struct mg_http_message *hm) {
    int book = 0, chapter = 0, verse = 0;
    char sql[256];
    sqlite3 *db;
    sqlite3_stmt *stmt;
    int rc;

    // Parse JSON body: expect {"book":1, "chapter":1, "verse":1}
    double dbook = 0, dchapter = 0, dverse = 0;
    if (!mg_json_get_num(hm->body, "$.book", &dbook) ||
        !mg_json_get_num(hm->body, "$.chapter", &dchapter) ||
        !mg_json_get_num(hm->body, "$.verse", &dverse)) {
        mg_http_reply(c, 400, "", "Invalid JSON: expected book, chapter, verse\n");
        return;
    }
    book = (int)dbook;
    chapter = (int)dchapter;
    verse = (int)dverse;

    rc = sqlite3_open(DB_PATH, &db);
    if (rc != SQLITE_OK) {
        mg_http_reply(c, 500, "", "DB open error\n");
        return;
    }

    mg_snprintf(sql, sizeof(sql),
        "SELECT text FROM kjv WHERE book=? AND chapter=? AND verse=?");

    rc = sqlite3_prepare_v2(db, sql, -1, &stmt, 0);
    if (rc != SQLITE_OK) {
        mg_http_reply(c, 500, "", "DB prepare error\n");
        sqlite3_close(db);
        return;
    }

    sqlite3_bind_int(stmt, 1, book);
    sqlite3_bind_int(stmt, 2, chapter);
    sqlite3_bind_int(stmt, 3, verse);

    if (sqlite3_step(stmt) == SQLITE_ROW) {
        const unsigned char *text = sqlite3_column_text(stmt, 0);
        mg_http_reply(c, 200, "Content-Type: application/json\r\n",
            "{ \"book\": %d, \"chapter\": %d, \"verse\": %d, \"text\": \"%s\" }\n",
            book, chapter, verse, text);
    } else {
        mg_http_reply(c, 404, "", "Verse not found\n");
    }

    sqlite3_finalize(stmt);
    sqlite3_close(db);
}

void get_chapter(struct mg_connection *c, struct mg_http_message *hm) {
    int book = 0, chapter = 0;
    char sql[256];
    sqlite3 *db;
    sqlite3_stmt *stmt;
    int rc;

    // Parse JSON body: expect {"book":1, "chapter":1}
    double dbook = 0, dchapter = 0;
    if (!mg_json_get_num(hm->body, "$.book", &dbook) ||
        !mg_json_get_num(hm->body, "$.chapter", &dchapter)) {
        mg_http_reply(c, 400, "", "Invalid JSON: expected book, chapter\n");
        return;
    }
    book = (int)dbook;
    chapter = (int)dchapter;

    rc = sqlite3_open(DB_PATH, &db);
    if (rc != SQLITE_OK) {
        mg_http_reply(c, 500, "", "DB open error\n");
        return;
    }

    mg_snprintf(sql, sizeof(sql),
        "SELECT verse, text FROM kjv WHERE book=? AND chapter=? ORDER BY verse ASC");

    rc = sqlite3_prepare_v2(db, sql, -1, &stmt, 0);
    if (rc != SQLITE_OK) {
        mg_http_reply(c, 500, "", "DB prepare error\n");
        sqlite3_close(db);
        return;
    }

    sqlite3_bind_int(stmt, 1, book);
    sqlite3_bind_int(stmt, 2, chapter);

    char esc_text[1024];
    char *json = NULL, *tmp = NULL;
    int first = 1, error = 0;

    json = mg_mprintf("{ \"book\": %d, \"chapter\": %d, \"verses\": [", book, chapter);
    if (!json) error = 1;

    while (!error && sqlite3_step(stmt) == SQLITE_ROW) {
        int verse = sqlite3_column_int(stmt, 0);
        const unsigned char *text = sqlite3_column_text(stmt, 1);
        json_escape((const char *)text, esc_text, sizeof(esc_text));
        tmp = mg_mprintf(first ? "%s{ \"verse\": %d, \"text\": \"%s\" }"
                                 : "%s, { \"verse\": %d, \"text\": \"%s\" }",
                        json, verse, esc_text);
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

    if (error || !final_json) {
        if (final_json) free(final_json);
        mg_http_reply(c, 500, "", "Out of memory\n");
    } else if (first) {
        free(final_json);
        mg_http_reply(c, 404, "", "Chapter not found\n");
    } else {
        mg_http_reply(c, 200, "Content-Type: application/json\r\n", "%s", final_json);
        free(final_json);
    }
}

void get_passage(struct mg_connection *c, struct mg_http_message *hm) {
};