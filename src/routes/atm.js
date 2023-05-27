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

router.get('/atm', checkAdmin, async (req, res) => {
    console.log("GET /atm")
    const client = await public.connect();
    try {
        const result = await client.query('SELECT * FROM atms')
        res.status(200).json(result.rows)
    } finally {
        res.status(500).send("Server error")
        client.release();
    }
})

router.put('/atm', checkAdmin, express.json(), async (req, res) => {
    console.log("PUT /atm")
    const client = await public.connect()

    reqBody = req.body;

    params = [reqBody.stock, reqBody.address, reqBody.branch_id]

    for (par of params) {
        if (par == undefined)
            return res.status(400).send("Invalid request")
    }


    client.query('INSERT INTO atms (stock, address, branch_id) VALUES ($1, $2, $3)', params, (err, result) => {
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

router.put('/atm/:atm_id', checkAdmin, express.json(), async (req, res) => {
    console.log("PUT /atm/" + req.params.atm_id)
    const client = await public.connect()

    reqBody = req.body;

    params = [req.params.atm_id, reqBody.stock, reqBody.address]

    for (par of params) {
        if (par == undefined)
            return res.status(400).send("Invalid request")
    }

    client.query('UPDATE atms SET stock = $2, address = $3 WHERE atm_id = $1', params, (err, result) => {
        if (err) {
            console.log(err.stack)
            res.send(500).send("Server error");
            client.release();
        }
        else {
            res.status(200).send("Success")
            client.release();
        }
    })
})

router.delete('/atm/:atm_id', checkAdmin, async (req, res) => {
    console.log("DELETE /atm/" + req.params.atm_id)
    const client = await public.connect();

    const result = await client.query('DELETE FROM atms WHERE atm_id = $1', [req.params.atm_id], (err, result) => {
        if (err) {
            console.log(err.stack)
            client.release()
            return res.status(404).json("ATM not found")
        } else {
            client.release();
            return res.status(200).json("Success")
        }

    });

})

router.get('/atm/:branch_id', checkAdmin, async (req, res) => {
    console.log("GET /atm/" + req.params.branch_id)
    const client = await public.connect();

    client.query('SELECT * FROM atms WHERE branch_id = $1', [req.params.branch_id], (err, result) => {
        if (err) {
            console.log(err.stack)
            client.release();
            res.status(404).send("Branch not found")
        }
        else {
            res.status(200).json(result.rows)
            client.release();
        }
    })

})

router.post('/atm/refil', checkAdmin, express.json(), async (req, res) => {
    console.log("POST /atm/refil")

    reqBody = req.body;
    if (reqBody.limit == undefined)
        return res.status(400).json("Invalid request")

    const client = await public.connect();
    try {
        const result = await client.query('SELECT * FROM atms WHERE stock < $1', [reqBody.limit])
        res.status(200).send(result.rows)
    } finally {
        res.status(500).send("Server error")
        client.release();
    }
})



module.exports = router;