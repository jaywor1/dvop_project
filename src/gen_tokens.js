const crypto = require('crypto')

const generateApiKey = () => {
    return crypto.randomBytes(32).toString('hex');
}

const hashString = (str) => {
    const hash = crypto.createHash('sha256');
    hash.update(str);
    return hash.digest('hex');
};


const token = generateApiKey()

const hash = hashString(token)

console.log("Token: " + token)
console.log("Hash: " + hash)