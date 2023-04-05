const express = require('express')
const pg = require('pg')
const crypto = require('crypto')
const atm_route = require('./atm')

const app = express();


const pool = new pg.Pool({
    user: "postgres",
    host: "localhost",
    database: "db",
    password: "postgres",
    port: 5432
})

const private = new pg.Pool({
    user: "postgres",
    host: "localhost",
    database: "private",
    password: "postgres",
    port: 5432
})

const hashString = (str) => {
    const hash = crypto.createHash('sha256');
    hash.update(str);
    return hash.digest('hex');
};
const apiKeyAuth = async (req, res, next) => {
    const apiKey = req.query.api_key
    if (!apiKey) {
        return res.status(401).send("Invalid request")
    }

    const client = await private.connect()

    try {
        const result = await client.query('SELECT * FROM tokens')

        for (token of result.rows) {
            if (token.token_hash == hashString(apiKey)) {
                console.log("Authentication Success\nAdmin: " + token.admin)
                req.admin = token.admin
                next()
            }
        }


    } finally {
        client.release();
        res.status(403).send("Forbiden");
    }
}



app.use('/', apiKeyAuth, atm_route)



app.listen(3000, () => {
    console.log('srv runnin\'')
})