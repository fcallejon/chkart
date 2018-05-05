# AspNetCore 2, Docker

## General Idea

The general Idea of this repo it's to test how to implement a microservices infra using docker and aspnetcore.

I will only focus on the CHKTR (a join between CheKout and caRT... that I misspell as CHecKout c{K}a _TR_ when creating the folder ¯\\_(ツ)_/¯ )

## How to run

After downloading the code run `.\up.ps1` from a powershell console.
> *nix and MacOs coming soon

## Parts

### Api Folder (chktr-api)

This is the cart service that a client (yet to be done) will use to handle whatever their customers want to buy.
This project will not take care of the payment process.

### Client (chktr-client... **yet-to-be-done**)

This will be an assembly that anyone can use to easily access the chktr-api API.

## How to use the chktr-api API

An ApiKey will be handled to the dev that want to use this service and, by using the client, the dev can connect it's project to our service and add a cart to it's website.

----------

### TO-DO

- Define how to communicate with a Purchase Service
- Add Unit Tests
- Add a dashboard to generate the dev Key
- Add small sample app using the client
- Add E2E Test?

----------


