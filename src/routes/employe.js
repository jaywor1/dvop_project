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


module.exports = router;