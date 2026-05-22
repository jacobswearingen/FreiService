#ifndef HANDLERS_KJV_H
#define HANDLERS_KJV_H
#include "mongoose.h"
#include <sqlite3.h>

#define DB_PATH "db.db"

void get_verse(struct mg_connection *c, struct mg_http_message *hm, sqlite3 *db);
void get_chapter(struct mg_connection *c, struct mg_http_message *hm, sqlite3 *db);
void get_passage(struct mg_connection *c, struct mg_http_message *hm, sqlite3 *db);
#endif // HANDLERS_KJV_H