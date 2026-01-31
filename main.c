#include "mongoose.h"
#include <sqlite3.h>
#include <stdio.h>


#define DB_PATH "db.db"



static void handle_verse(struct mg_connection *c, struct mg_http_message *hm, int book, int chapter, int verse) {
    char sql[256];
    sqlite3 *db;
    sqlite3_stmt *stmt;
    int rc;

    rc = sqlite3_open(DB_PATH, &db);
    if (rc != SQLITE_OK) {
        mg_http_reply(c, 500, "", "DB open error\n");
        return;
    }

    snprintf(sql, sizeof(sql),
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

// Route handler for /kjv/{book}/{chapter}/{verse}
static void route_kjv(struct mg_connection *c, struct mg_http_message *hm) {
    int book, chapter, verse;
    char path[64];
    snprintf(path, sizeof(path), "%.*s", (int)hm->uri.len, hm->uri.buf);
    int n = sscanf(path, "/kjv/%d/%d/%d", &book, &chapter, &verse);
    if (n == 3) {
        handle_verse(c, hm, book, chapter, verse);
    } else {
        mg_http_reply(c, 400, "", "Invalid path\n");
    }
}

// Route handler for /routes (shows all available routes)
static void route_list(struct mg_connection *c, struct mg_http_message *hm) {
    mg_http_reply(c, 200, "Content-Type: text/plain\r\n",
        "Available routes:\n"
        "/kjv/{book}/{chapter}/{verse}\n"
        "/routes\n"
    );
}

struct route {
    const char *pattern;
    void (*handler)(struct mg_connection *, struct mg_http_message *);
};

static struct route routes[] = {
    {"/kjv/*/*/*", route_kjv},
    {"/routes", route_list},
    // Add more routes here
};


static void fn(struct mg_connection *c, int ev, void *ev_data) {
    if (ev == MG_EV_HTTP_MSG) {
        struct mg_http_message *hm = (struct mg_http_message *) ev_data;
        size_t i;
        for (i = 0; i < sizeof(routes)/sizeof(routes[0]); ++i) {
            if (mg_match(hm->uri, mg_str(routes[i].pattern), NULL)) {
                routes[i].handler(c, hm);
                return;
            }
        }
        mg_http_reply(c, 404, "", "Not found\n");
    }
}

int main(void) {
    struct mg_mgr mgr;
    mg_log_set(MG_LL_DEBUG);
    mg_mgr_init(&mgr);
    mg_http_listen(&mgr, "http://0.0.0.0:8000", fn, NULL);
    printf("Server started on http://localhost:8000\n");
    for (;;) mg_mgr_poll(&mgr, 1000);
    mg_mgr_free(&mgr);
    return 0;
}