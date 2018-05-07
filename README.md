# AspNetCore 2, Docker

## General Idea

The general Idea of this repo it's to test how to implement a microservices infra using docker and aspnetcore.

I will only focus on the CHKTR (a join between CheKout and caRT... that I misspell as CHecKout c{K}a _TR_ when creating the folder ¯\\_(ツ)_/¯ )

## How to run

### Run in docker

After downloading the code run `.\up.ps1` from a powershell console.
Then access the API using `http://localhost:8181/` in brwoser to see the Swagger UI.

You can also run the `test-console` app:
(assuming you have a console open at the project root level)
1. `cd test-console`
2. `dotnet run -- <ADDRESS_TO_API>`

### Run for development

First you need to specify a redis server in `appSettings.Development.json`.
There is always the option of running `docker-compose up -d redis` from the project's root and use the docker version (in that case there is nothing to change in `appSettings`).

Once you have Redis running you can go ahead and run `.\api\rundev.ps1` from the project's root in a PS console.

Then you can access the SwaggerUI at `http://localhost:5000`.

## Parts

### Api Folder (chktr-api)

This is the cart service that a client (yet to be done) will use to handle whatever their customers want to buy.
This project will not take care of the payment process.

#### Test

You can find the test for it under `[root]\tests\ApiTests`.
To Debug:
1. run the task `debug api test proj` (from `CTRL+SHIFT+P` `>Tasks: Run task`).
2. Switch to Debug Panel (`CTRL+SHIFT+D`) ad pick `debug-tests`
3. Hit Play and pick the process id from step 1

There is also a `runWatch.ps1` under `[root]\tests\ApiTests` scripts that you can run and see how tests get re-run after every change you save.

### Model (chktr-model)

This will contain the shared model between the client and the API.

### Client (chktr-client)

This will be an assembly that anyone can use to easily access the chktr-api API.

#### Test

You can find the test for it under `[root]\tests\ClientTests`.
To Debug:
1. run the task `debug client test proj` (from `CTRL+SHIFT+P` `>Tasks: Run task`).
2. Switch to Debug Panel (`CTRL+SHIFT+D`) ad pick `debug-tests`
3. Hit Play and pick the process id from step 1

There is also a `runWatch.ps1` under `[root]\tests\ClientTests` scripts that you can run and see how tests get re-run after every change you save.

## How to use the chktr-api API

An ApiKey will be handled to the dev that want to use this service and, by using the client, the dev can connect it's project to our service and add a cart to it's website.

----------

### What else can be done?

- Enhance Model (ie.: add an Key to CartItem)
- Fix Code Coverage to increate it up to 80%
- Add some UTs for CartService
- Add a dashboard to generate the dev Key.
- Add E2E Test.
- Add an MVC sample App.
- Rename same projects.
- Add a new redis _subkey_ to separate Carts by client.

----------


