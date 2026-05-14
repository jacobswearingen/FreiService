use actix_web::{App, HttpServer};
mod handlers;
mod router;

#[actix_web::main]
async fn main() -> std::io::Result<()> {
    println!("Server started on http://localhost:8000");
    HttpServer::new(|| App::new().configure(router::init_routes))
        .bind(("0.0.0.0", 8000))?
        .run()
        .await
}
