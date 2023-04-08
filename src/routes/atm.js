const express = require('express')
const pg = require('pg')
const router = express.Router()

const public = require('../db/public')


router.get('/atm', async (req, res) => {
    console.log("GET /atm")
    const client = await public.connect();
    try {
        const result = await client.query('SELECT * FROM atms')
        res.send(result.rows)
    } finally {
        client.release();
    }
})

router.put('/atm', express.json(), async(req, res) => {
    console.log("PUT /atm")
    const client = await public.connect()

    reqBody = req.body;

    params = [reqBody.stock, reqBody.address, 'f']

    for(par of params){
        if (par == undefined)
        return res.status(400).send("Invalid request")
    }
    

    client.query('INSERT INTO atms (stock, address, error) VALUES ($1, $2, $3)', params, (err, result) => {
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

router.delete('/atm/:atm_id', async (req, res) => {
    console.log("DELETE /atm/" + req.params.atm_id)
    const client = await public.connect();
    
    const result = await client.query('DELETE FROM atms WHERE atm_id = $1', [req.params.atm_id], (err, result) => {
        if(err){
            console.log(err.stack)
            client.release()
            return res.status(400).json("Invalid request")
        } else {
            client.release();
            return res.status(200).json("Success")
        }

    });
    
})

router.get('/atm/:atm_id', async (req, res) => {
    console.log("GET /atm/" + req.params.atm_id)
    const client = await public.connect();

    client.query('SELECT * FROM atms WHERE atm_id = $1', [req.params.atm_id], (err, result) => {
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

router.post('/atm/broken', express.json(), async (req, res) => {
    console.log("POST /atm/broken")

    reqBody = req.body;

    if(reqBody.broken == undefined)
        return res.status(400).json("Invalid request")

    const client = await public.connect();
    try {
        const result = await client.query('SELECT atm_id,error FROM atms WHERE error = $1', [reqBody.broken])
        res.json(result.rows)
    } finally {
        client.release();
    }
})

router.post('/atm/refil', express.json(), async (req, res) => {
    console.log("POST /atm/refil")

    reqBody = req.body;

    if(reqBody.limit == undefined)
        return res.status(400).json("Invalid request")

    const client = await public.connect();
    try {
        const result = await client.query('SELECT * FROM atms WHERE stock < $1', [reqBody.limit])
        res.send(result.rows)
    } finally {
        client.release();
    }
})


module.exports = router;