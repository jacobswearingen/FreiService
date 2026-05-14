use actix_web::{web, HttpResponse};
use rusqlite::{params, Connection};
use serde::{Deserialize, Serialize};

#[derive(Deserialize)]
pub struct VerseRequest {
    book: i32,
    chapter: i32,
    verse: i32,
}

#[derive(Serialize)]
pub struct VerseResponse {
    book: i32,
    chapter: i32,
    verse: i32,
    text: String,
}

pub async fn get_verse(req: web::Json<VerseRequest>) -> HttpResponse {
    let conn = match Connection::open("db.db") {
        Ok(c) => c,
        Err(_) => return HttpResponse::InternalServerError().body("DB open error"),
    };

    let mut stmt =
        match conn.prepare("SELECT text FROM kjv WHERE book=?1 AND chapter=?2 AND verse=?3") {
            Ok(s) => s,
            Err(_) => return HttpResponse::InternalServerError().body("DB prepare error"),
        };

    let verse_text: Result<String, _> =
        stmt.query_row(params![req.book, req.chapter, req.verse], |row| row.get(0));
    match verse_text {
        Ok(text) => {
            let resp = VerseResponse {
                book: req.book,
                chapter: req.chapter,
                verse: req.verse,
                text,
            };
            HttpResponse::Ok().json(resp)
        }
        Err(_) => HttpResponse::NotFound().body("Verse not found"),
    }
}
