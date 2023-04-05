const express = require('express')
const pg = require('pg')
const router = express.Router()

const public = require('../db/public')

router.get('/branch', async (req, res) => {
    console.log("GET /branch")
    const client = await public.connect();

    client.query('SELECT * FROM branch', (err, result) => {
        if (err) {
            console.log(err.stack)
            client.release();
        }
        else {
            res.status(200).json(result.rows)
            client.release();
        }
    })

})

module.exports = router;