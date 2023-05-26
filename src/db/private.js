const pg = require('pg')

const private = new pg.Pool({
    user: "postgres",
    host: "localhost",
    database: "private",
    password: "postgres",
    port: 5432
})

module.exports = private