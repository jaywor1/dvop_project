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
    console.log("GET /atm/" + req.params.atm_id + "/log")
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

router.get('/atm/broken', async (req, res) => {
    console.log("GET /atm/broken")
    const client = await public.connect();
    try {
        const result = await client.query('SELECT atm_id,error_log FROM atms WHERE error_log is not null')
        res.json(result.rows)
    } finally {
        client.release();
    }
})

router.get('/atm/refil', async (req, res) => {
    console.log("GET /atm/refil")
    const client = await public.connect();
    try {
        const result = await client.query('SELECT * FROM atms WHERE stock < 20000')
        res.send(result.rows)
    } finally {
        client.release();
    }
})


module.exports = router;