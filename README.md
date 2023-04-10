# dvop_project
Please let me pass this subject :))
## Design
### Endpoints
#### ATM
- /atm --> list of atms ATM(atm_id, stock, withdraw_log, errors)
- /atm/broken --> atms with errors
- /atm/refil  --> atms that are close running out of stock
- /atm/{atm_id}/log --> shows withdraw_log
- /atm/{atm_id}/error --> shows error log
#### Branch
- /branch --> list of branches Branch(branch_id, Employee(employe_id, position, present), opening_hours, location)
- /branch/{branch_id}/manager --> list of managers in selected branch
- /branch/director --> list of director of each branch
- /branch/opened --> opened branches
### DB Private
![image](https://user-images.githubusercontent.com/103755136/230916105-eb2e9efa-7374-45a3-9d5d-cd582e707b19.png)
### DB Public
![image](https://user-images.githubusercontent.com/103755136/230916524-ce8901ba-0257-41b6-9433-dfda1954fd9c.png)
