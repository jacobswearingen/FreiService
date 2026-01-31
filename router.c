#include "kjv.h"
#include "router.h"
#include <stddef.h>

struct route {
    const char *pattern;
    void (*handler)(struct mg_connection *, struct mg_http_message *);
};

static struct route routes[] = {
    {"/kjv/get_verse", get_verse},
    {"/kjv/get_chapter", get_chapter},
    {"/kjv/get_passage", get_passage},
};

void route_request(struct mg_connection *c, struct mg_http_message *hm) {
    for (size_t i = 0; i < sizeof(routes)/sizeof(routes[0]); ++i) {
        if (mg_match(hm->uri, mg_str(routes[i].pattern), NULL)) {
            routes[i].handler(c, hm);
            return;
        }
    }
    mg_http_reply(c, 404, "", "Not found\n");
}
