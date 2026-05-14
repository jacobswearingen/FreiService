use actix_web::web;

use super::get_chapter;
use super::get_passage;
use crate::handlers;

pub fn init_routes(cfg: &mut web::ServiceConfig) {
    cfg.route("/kjv/get_verse", web::post().to(handlers::get_verse))
        .route("/kjv/get_chapter", web::post().to(get_chapter::get_chapter))
        .route("/kjv/get_passage", web::post().to(get_passage::get_passage));
}
