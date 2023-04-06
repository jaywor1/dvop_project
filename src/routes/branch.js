const express = require('express')
const pg = require('pg')
const router = express.Router()

const public = require('../db/public')

const checkAdmin = (req, res, next) => {
    if (req.admin == false) {
        return res.status(401).send("Forbiden")
    }
    next()
}

router.get('/branch', checkAdmin, async (req, res) => {
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
router.post('/branch', checkAdmin, express.json(), async (req, res) => {
    console.log("POST /branch")
    const reqBody = req.body;
    const client = await public.connect();

    const params = [reqBody.open_hours, reqBody.close_hours, reqBody.address]

    for (par of params) {
        if (par == undefined)
            return res.status(400).send("Invalid request")
    }

    client.query('INSERT INTO branch (open_hours, close_hours, address) VALUES ($1, $2, $3)', params, (err, result) => {
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