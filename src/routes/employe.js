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

router.get('/employe', checkAdmin, async (req, res) => {
    console.log("GET /employe")
    const client = await public.connect();

    client.query('SELECT * FROM employes', (err, result) => {
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

router.put('/employe', checkAdmin, express.json(), async (req, res) => {
    console.log("PUT /employe")
    const client = await public.connect();

    reqBody = req.body;

    params = [reqBody.branch_id, reqBody.name, reqBody.position, reqBody.present]

    console.log(params)

    for (par of params) {
        if (par == undefined)
            return res.status(400).send("Invalid request")
    }

    client.query('INSERT INTO employes (branch_id, name, position, present) VALUES ($1, $2, $3, $4)', params, (err, result) => {
        if (err) {
            console.log(err.stack)
            client.release();
        }
        else {
            res.status(200).send("Success")
            client.release();
        }
    })

})



module.exports = router;