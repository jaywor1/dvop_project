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
            res.status(500).send("Server error")
            client.release();
        }
        else {
            res.status(200).json(result.rows)
            client.release();
        }
    })

})
router.put('/branch', checkAdmin, express.json(), async (req, res) => {
    console.log("PUT /branch")
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
            res.status(200).json("Success");
            client.release();
        }
    })

})

router.delete('/branch/:branch_id', checkAdmin, async (req, res) => {
    console.log("DELETE /branch/" + req.params.branch_id)
    const client = await public.connect();

    client.query('DELETE FROM branch WHERE branch_id = $1', [req.params.branch_id], (err, result) => {
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

router.get('/branch/:branch_id/employes', checkAdmin, async (req, res) => {
    console.log("GET /branch/" + req.params.branch_id + "/employes")
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

router.put('/branch/:branch_id', express.json(), async (req, res) => {
    console.log("PUT /branch/" + req.params.branch_id)

    reqBody = req.body;

    params = [req.params.branch_id, req.body.open_hours, req.body.close_hours, req.body.address]

    for (par of params) {
        if (par == undefined)
            return res.status(400).json("Invalid Request")
    }

    const client = await public.connect();

    client.query('UPDATE branch SET open_hours = $2, close_hours = $3, address = $4 WHERE branch_id = $1', params, (err, result) => {
        if (err) {
            console.log(err.stack)
            res.status(400).send("Failure")
            client.release();
        }
        else {
            res.status(200).send("Success")
            client.release();
        }
    })

})

module.exports = router;