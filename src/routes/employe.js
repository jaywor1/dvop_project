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
            res.status(500).send("Server error")
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
            res.status(500).send("Server error")
        }
        else {
            res.status(200).send("Success")
            client.release();
        }
    })

})

router.put('/employe/:employe_id', checkAdmin, express.json(), async (req, res) => {
    console.log("PUT /employe/" + req.params.employe_id)

    reqBody = req.body;

    params = [req.params.employe_id, req.body.branch_id, req.body.name, req.body.position, req.body.present]

    for (par of params) {
        if (par == undefined)
            return res.status(400).json("Invalid Request")
    }

    const client = await public.connect();

    client.query('UPDATE employes SET branch_id = $2, name = $3, position = $4, present = $5 WHERE employe_id = $1', params, (err, result) => {
        if (err) {
            console.log(err.stack)
            client.release();
            res.status(500).send("Server error")
        }
        else {
            res.status(200).send("Success")
            client.release();
        }
    })


})

router.get('/employe/:branch_id', checkAdmin, async (req, res) => {
    console.log("GET /employe/" + req.params.branch_id)
    const client = await public.connect();

    client.query('SELECT * FROM employes WHERE branch_id = $1', [req.params.branch_id], (err, result) => {
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

router.delete('/employe/:employe_id', checkAdmin, async (req, res) => {
    console.log("DELETE /employe/" + req.params.employe_id)
    const client = await public.connect();

    client.query('DELETE FROM employes WHERE employe_id = $1', [req.params.employe_id], (err, result) => {
        if (err) {
            console.log(err.stack)
            client.release();
            res.status(404).send("Employee not found")
        }
        else {
            res.status(200).send("Success")
            client.release();
        }
    })

})


module.exports = router;