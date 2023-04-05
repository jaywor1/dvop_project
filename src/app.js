const express = require('express')
const pg = require('pg')
const crypto = require('crypto')

const atm_route = require('./routes/atm')
const public = require('./db/public')
const private = require('./db/private')

const app = express();

const hashString = (str) => {
    const hash = crypto.createHash('sha256');
    hash.update(str);
    return hash.digest('hex');
};
const apiKeyAuth = async (req, res, next) => {
    const apiKey = req.query.api_key
    if (!apiKey) {
        return res.status(401).send("Invalid Request")
    }

    const client = await private.connect()

    try {
        const result = await client.query('SELECT * FROM tokens')


        for (token of result.rows) {
            console.log(token.token_hash + ";" + hashString(apiKey))
            if (token.token_hash === hashString(apiKey)) {
                req.admin = token.admin
                next()
                break
            }
        }


    } finally {
        client.release();
    }

}



app.use('/', apiKeyAuth, atm_route)



app.listen(3000, () => {
    console.log('srv runnin\'')
})