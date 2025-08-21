# Modular Monolith PoC

A small proof of concept / playground testing some techniques when build a modular monolith.

## Getting started

Ensure you have Docker installed and running.

Then just start the `ModularMonolithPoC.AppHost` project. This is the Aspire app host which will launch all required components.

## Functionality

There are two modules in this PoC:

- Persons: A simple module to manage persons.
- Eligibility: A module to check if a person is eligible for something.

You can create, update and delete persons in the Persons module via these endpoints:

```
POST /persons
Content-Type: application/json

{
  "id": "<guid>"
  "name": "<name>"
}
```

```
PUT /persons/<id>
Content-Type: application/json

{
  // Required by the model, but not used, can be a random, but valid guid
  "id": "<guid>"
  "name": "<name>"
}
```

```
DELETE /persons/{id}
```

The Persons module will publish an event when a person is created, updated or deleted. The Eligibility module will subscribe to these events and persist the persons
in its own database as a materialized view.

You can then retrieve the persons with a random eligibility score via the following endpoint:

```
GET /eligibility/all-persons?useMediator=true
```

This endpoint has the optional query parameter `useMediator`. If set to `true`, the endpoint will will retrieve all persons directly from the Persons module via a mediator.
If set to `false` or not set, the endpoint will retrieve the persons from the Eligibility module's own database.

Then a person's score is calculated by a random number generator seeded which the person's id for consistent scores for each person.

## Observability

You can view the traces for all requests in the Aspire dashboard.

You can easily access the database via the PgWeb dashboard. Open it via the Resource overview in the Aspire dashboard and select the bookmarked database for easy access.

You can view the RabbitMq management dashboard by opening the RabbitMq resource in the Aspire dashboard.
To access the management dashboard, retrieve the username and password from the RabbitMq resource's environment variables in the Aspire dashboard.
