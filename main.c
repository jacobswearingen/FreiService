#include "mongoose.h"
#include "kjv.h"
#include "router.h"
#include <sqlite3.h>
#include <stdio.h>

struct app_state {
    sqlite3 *db;
};

static void fn(struct mg_connection *c, int ev, void *ev_data) {
    struct app_state *state = (struct app_state *) c->fn_data;

    if (ev == MG_EV_HTTP_MSG) {
        struct mg_http_message *hm = (struct mg_http_message *) ev_data;
        route_request(c, hm, state->db);
    }
}

int main(void) {
    struct mg_mgr mgr;
    struct app_state state = {0};

    mg_log_set(MG_LL_DEBUG);
    if (sqlite3_open(DB_PATH, &state.db) != SQLITE_OK) {
        fprintf(stderr, "Failed to open database: %s\n", sqlite3_errmsg(state.db));
        if (state.db) sqlite3_close(state.db);
        return 1;
    }

    mg_mgr_init(&mgr);
    mg_http_listen(&mgr, "http://0.0.0.0:8000", fn, &state);
    printf("Server started on http://localhost:8000\n");
    for (;;) mg_mgr_poll(&mgr, 1000);
    mg_mgr_free(&mgr);
    sqlite3_close(state.db);
    return 0;
}