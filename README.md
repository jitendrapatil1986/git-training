## Warranty

[First Time Setup](SETUP.md)

### Code Review

#### Code Review Process

1. Create a remote branch
2. Issue new pull request from your new branch to [/davidweekleyhomes/warranty](/davidweekleyhomes/warranty)
3. Somebody will code review and either give feedback or merge your PR.

#### Code Review Tips

1. Code should do what it is intended
2. Are there any pieces of code that are confusing? Ask about them
3. Any "tricky/hacky/clever" parts of the code should be reviewed or talked about.
4. The level of testing is appropriate
5. Code follows our [standards](CODESTANDARDS.md).
6. Code files are in their appropriate projects/folders/locations.

## Health Checks

A HealthCheck service was introduced to help verify data between systems.  Initially it launches with support for Warranty and Tips.

It uses Quartz rather than the NServiceBus scheduler as it allows you to use "Cron" triggers rather than just hourly triggers.
It currently does not use a database to persist the trigger state but this is something we could explore in the future if needed (Quartz supports it)

### Writing a New HealthCheck

1. In the HealthChecks folder, add a new class that schedules your HealthCheck - inherit from IHealthCheck.  The scheduler built into the HealthCheck uses [Quartz](http://www.quartz-scheduler.net/documentation/quartz-2.x/tutorial/jobs-and-triggers.html)
2. Your new HealthCheck should only send a message locally to kick off your check - don't run it within the Job - this allows you trigger jobs on-demand in the future
3. Add your handler for the job into the Handlers folder - your Handler & message can live in the same class files

### Best Practices

* Keep all messages, data calls, handlers within the HealthCheck project - treat this like you would a neautral party, it shouldn't be privy to any of the domain logic/classes.
* Make use of MediatR for re-use where possible
* If you need to connect to a new database, add a new abstraction following the pattern already in place

### Database Abstraction

Each database connection is abstracted with Structure map / IDatabase interface.
For TIPS - use ITipsDatabase
For Warranty - use IWarrantyDatabase
