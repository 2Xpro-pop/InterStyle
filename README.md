# InterStyle

`InterStyle` is a pet project created to learn `DDD`, bounded contexts, domain events, and application architecture in the `.NET` ecosystem.

This project is not intended to be a reference implementation. It is more of a learning playground where I try ideas, make mistakes, rethink decisions, and gradually improve the model.

## Why this repository exists

The main goal is to practice:

- `Domain-Driven Design`
- separation of `Domain` / `Application` / `Infrastructure`
- a `CQRS` approach in the application layer
- domain events
- decomposition into bounded contexts
- integration between multiple services through a shared host and gateway

## What's inside

At the moment, the repository consists of several separate parts:

- `InterStyle.Curtains.*` — the context related to curtains and their management
- `InterStyle.Reviews.*` — the reviews context
- `InterStyle.Leads.*` — the leads context
- `InterStyle.IdentityApi` — authentication and JWT issuing
- `InterStyle.ImageApi` — image handling
- `AdminPanel` — a `Blazor WebAssembly` app for administration
- `InterStyle.AppHost` — local environment orchestration with `.NET Aspire`

Infrastructure used in the project:

- `PostgreSQL`
- `Redis`
- `YARP` gateway
- `Refit`
- `MediatR`

## Project status

This is a learning project, so it may contain:

- questionable architectural decisions
- excessive abstractions
- imperfect boundaries between layers
- compromises made for experimentation rather than production readiness

In short, this repository exists not to demonstrate “the right way”, but to document the learning process.

## Running locally

At minimum, you will need:

- `.NET 10 SDK`
- `Docker Desktop`

The main entry point for local startup is `InterStyle.AppHost`.

You can run it from `Visual Studio` by selecting `InterStyle.AppHost` as the startup project, or via CLI:

`dotnet run --project InterStyle.AppHost/InterStyle.AppHost.csproj`

### Important

For a full local run, you will need environment parameters / secrets used by `AppHost`, for example:

- `admin-login`
- `admin-password`
- `jwt-signing-pfx`
- `jwt-signing-password`
- `jwt-active-kid`
- `captcha-google-token`
- `mediatr-license-key`

So for now, this repository is aimed more at exploring the architecture and code than providing a polished one-click onboarding experience.

## Where the idea came from

I was inspired by the following materials:

- `https://github.com/dotnet/eShop`
- `https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/`

But there is an important nuance: this repository was started **before** I read this article:

- `https://medium.com/@iamprovidence/everything-wrong-with-eshoponcontainers-ce9319a7a601`

Because of that, the project may contain ideas inspired by `eShop` that today I would implement differently.

## How to look at it

It is better to treat `InterStyle` as:

- a practice journal for `DDD`
- a set of architecture experiments
- an attempt to find better bounded-context boundaries
- a place to compare “how I did it then” with “how I would do it now”

## What you can study in the code

If you are interested in the code itself, the best places to start are:

- aggregates and entities in `*.Domain`
- domain events
- application use cases in `*.Application`
- infrastructure adapters in `*.Infrastructure`
- service interaction through `InterStyle.AppHost`

## Disclaimer

If you are looking for a production-ready template for a `DDD` project, this repository is probably not the right fit without reservations.

If, however, you are interested in the learning path, mistakes, intermediate decisions, and gradually improving understanding of the domain, then this repository may be useful.
