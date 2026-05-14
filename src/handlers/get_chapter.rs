use actix_web::{web, HttpResponse};
use rusqlite::{params, Connection};
use serde::{Deserialize, Serialize};

#[derive(Deserialize)]
pub struct ChapterRequest {
    pub book: i32,
    pub chapter: i32,
}

#[derive(Serialize)]
pub struct ChapterVerse {
    pub verse: i32,
    pub text: String,
}

#[derive(Serialize)]
pub struct ChapterResponse {
    pub book: i32,
    pub chapter: i32,
    pub verses: Vec<ChapterVerse>,
}

pub async fn get_chapter(req: web::Json<ChapterRequest>) -> HttpResponse {
    let conn = match Connection::open("db.db") {
        Ok(c) => c,
        Err(_) => return HttpResponse::InternalServerError().body("DB open error"),
    };
    let mut stmt = match conn
        .prepare("SELECT verse, text FROM kjv WHERE book=?1 AND chapter=?2 ORDER BY verse ASC")
    {
        Ok(s) => s,
        Err(_) => return HttpResponse::InternalServerError().body("DB prepare error"),
    };
    let verses_iter = stmt.query_map(params![req.book, req.chapter], |row| {
        let verse: i32 = row.get(0)?;
        let text: String = row.get(1)?;
        Ok(ChapterVerse { verse, text })
    });
    let mut verses = Vec::new();
    match verses_iter {
        Ok(iter) => {
            for v in iter.flatten() {
                verses.push(v);
            }
            if verses.is_empty() {
                return HttpResponse::NotFound().body("Chapter not found");
            }
            let resp = ChapterResponse {
                book: req.book,
                chapter: req.chapter,
                verses,
            };
            HttpResponse::Ok().json(resp)
        }
        Err(_) => HttpResponse::NotFound().body("Chapter not found"),
    }
}
