use actix_web::{web, HttpResponse};
use rusqlite::{params, Connection};
use serde::{Deserialize, Serialize};

#[derive(Deserialize)]
pub struct PassageRequest {
    pub book: i32,
    pub start_chapter: i32,
    pub start_verse: i32,
    pub end_chapter: i32,
    pub end_verse: i32,
}

#[derive(Serialize)]
pub struct PassageVerse {
    pub chapter: i32,
    pub verse: i32,
    pub text: String,
}

#[derive(Serialize)]
pub struct PassageResponse {
    pub book: i32,
    pub start_chapter: i32,
    pub start_verse: i32,
    pub end_chapter: i32,
    pub end_verse: i32,
    pub verses: Vec<PassageVerse>,
}

pub async fn get_passage(req: web::Json<PassageRequest>) -> HttpResponse {
    let conn = match Connection::open("db.db") {
        Ok(c) => c,
        Err(_) => return HttpResponse::InternalServerError().body("DB open error"),
    };
    let sql = "SELECT chapter, verse, text FROM kjv WHERE book=?1 AND ((chapter > ?2 OR (chapter = ?2 AND verse >= ?3)) AND (chapter < ?4 OR (chapter = ?4 AND verse <= ?5))) ORDER BY chapter ASC, verse ASC";
    let mut stmt = match conn.prepare(sql) {
        Ok(s) => s,
        Err(_) => return HttpResponse::InternalServerError().body("DB prepare error"),
    };
    let verses_iter = stmt.query_map(
        params![
            req.book,
            req.start_chapter,
            req.start_verse,
            req.end_chapter,
            req.end_verse
        ],
        |row| {
            let chapter: i32 = row.get(0)?;
            let verse: i32 = row.get(1)?;
            let text: String = row.get(2)?;
            Ok(PassageVerse {
                chapter,
                verse,
                text,
            })
        },
    );
    let mut verses = Vec::new();
    match verses_iter {
        Ok(iter) => {
            for verse in iter {
                if let Ok(v) = verse {
                    verses.push(v);
                }
            }
            if verses.is_empty() {
                return HttpResponse::NotFound().body("Passage not found");
            }
            let resp = PassageResponse {
                book: req.book,
                start_chapter: req.start_chapter,
                start_verse: req.start_verse,
                end_chapter: req.end_chapter,
                end_verse: req.end_verse,
                verses,
            };
            HttpResponse::Ok().json(resp)
        }
        Err(_) => HttpResponse::NotFound().body("Passage not found"),
    }
}
