const express = require('express')
const pg = require('pg')
const crypto = require('crypto')

const atm_route = require('./routes/atm')
const branch_route = require('./routes/branch')
const employe_route = require('./routes/employe')

const public = require('./db/public')
const private = require('./db/private')

const app = express();
const EXPRESS_PORT = 3000;

const hashString = (str) => {
    const hash = crypto.createHash('sha256');
    hash.update(str);
    return hash.digest('hex');
};


const apiKeyAuth = async (req, res, next) => {
    const apiKey = req.query.api_key
    if (!apiKey) {
        return res.status(400).send("Invalid Request")
    }

    const client = await private.connect()

    try {
        const result = await client.query('SELECT * FROM tokens')


        for (token of result.rows) {
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



app.use('/api/v1/', apiKeyAuth, atm_route)
app.use('/api/v1/', apiKeyAuth, branch_route)
app.use('/api/v1/', apiKeyAuth, employe_route)


app.listen(EXPRESS_PORT, () => {
    console.log('srv servin\' on port ' + EXPRESS_PORT)
})