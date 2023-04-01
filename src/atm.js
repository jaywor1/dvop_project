const express = require('express')
const router = express.Router()

router.get('/atm', (req, res) => {
    // list of all atms
})

router.get('/atm/broken', (req, res) => {
    // returns broken atms
})

router.get('/atm/refil', (req, res) => {
    // returns atms that need refil
})

router.get('/atm/:atm_id/log', (req, res) => {
    // returns withdraw_log
})

router.get('/atm/:atm_id/error', (req, res) => {
    // returns error log
})

module.exports = router;