const pg = require('pg')

const public = new pg.Pool({
    user: "postgres",
    host: "localhost",
    database: "db",
    password: "postgres",
    port: 5432
})

module.exports = public