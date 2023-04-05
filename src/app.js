const express = require('express')
const pg = require('pg')
const atm_route = require('./atm')

const app = express();


const pool = new pg.Pool({
    user: "postgres",
    host: "localhost",
    database: "db",
    password: "postgres",
    port: 5432
})


app.use('/', atm_route)




app.listen(3000, () => {
    console.log('srv runnin\'')
})