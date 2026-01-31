#ifndef HANDLERS_KJV_H
#define HANDLERS_KJV_H
#include "mongoose.h"
void get_verse(struct mg_connection *c, struct mg_http_message *hm);
void get_chapter(struct mg_connection *c, struct mg_http_message *hm);
void get_passage(struct mg_connection *c, struct mg_http_message *hm);
#endif // HANDLERS_KJV_H