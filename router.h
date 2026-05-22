#ifndef ROUTER_H
#define ROUTER_H
#include "mongoose.h"
#include <sqlite3.h>

void route_request(struct mg_connection *c, struct mg_http_message *hm, sqlite3 *db);
#endif // ROUTER_H
