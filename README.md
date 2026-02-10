# Virto Commerce Webhooks Module

## Overview

The Webhooks module enables real-time event-driven notifications within the Virto Commerce platform. It automatically discovers all domain events raised by installed modules and delivers HTTP POST notifications with JSON-serialized event data to user-configured endpoints. The module supports selective payload composition, multiple authentication schemes, and resilient delivery with exponential-backoff retries.

## Key Features

* **Automatic event discovery** — resolves all `DomainEvent` types from the platform and installed modules at runtime
* **Selective payload composition** — choose specific entity properties to include in the notification body, with access to previous values for change tracking
* **Multiple authentication schemes** — None, HTTP Basic, Bearer Token, and Custom Header
* **Resilient delivery** — configurable retry policy with exponential backoff powered by Polly (default: 3 retries at 1, 2, 4 minute intervals)
* **Execution audit log (Feed)** — records request/response headers, body, HTTP status, error messages, and attempt counts for every webhook invocation
* **Background processing** — notifications are dispatched asynchronously via Hangfire with a 5-second debounce delay and processed in batches of 20
* **Permission-based access control** — granular permissions for read, update, delete, and feed access
* **Multi-database support** — SQL Server, MySQL, and PostgreSQL via dedicated EF Core provider assemblies

## Configuration

### Application Settings

| Setting | Type | Default | Description |
|---------|------|---------|-------------|
| `Webhooks.General.SendRetryCount` | Integer | `3` | Number of retry attempts for failed webhook deliveries |
| `Webhooks.General.LatestErrorCount` | Integer | `5` | Number of recent error entries retained per webhook |

### Permissions

| Permission | Description |
|------------|-------------|
| `webhooks:access` | Open the Webhooks menu |
| `webhooks:read` | View webhooks and registered events |
| `webhooks:update` | Create or update webhooks |
| `webhooks:delete` | Delete webhooks and feed entries |
| `webhooks:feed:read` | View webhook execution feed |

## Architecture

The module follows a clean, layered architecture aligned with Virto Commerce platform conventions:

```
┌─────────────────────────────────────────────────────────────┐
│  Web Layer (API Controllers, Module initialization)         │
├─────────────────────────────────────────────────────────────┤
│  Data Layer (Services, Repositories, EF Core, Caching)      │
├──────────┬──────────────┬──────────────┬────────────────────┤
│ SqlServer│    MySql     │  PostgreSql  │  DB Providers      │
├──────────┴──────────────┴──────────────┴────────────────────┤
│  Core Layer (Domain Models, Service Interfaces, Constants)  │
└─────────────────────────────────────────────────────────────┘
```

### Event-to-Notification Flow

1. A **domain event** is raised by the platform or any installed module
2. `WebHookManager` handles the event and queries active webhooks subscribed to that event type
3. Entity properties are resolved and the JSON payload is composed based on the webhook's payload configuration
4. A **Hangfire background job** is scheduled with a 5-second delay to debounce rapid event sequences
5. Webhooks are processed in **batches of 20**; each webhook is sent via `RetriableWebHookSender`
6. On failure, **Polly retry policy** retries with exponential backoff (2^(n-1) minutes)
7. Every attempt (success or error) is persisted as a `WebhookFeedEntry` for auditing

## Components

### Projects

| Project | Layer | Purpose |
|---------|-------|---------|
| `VirtoCommerce.WebHooksModule.Core` | Core | Domain models, service interfaces, module constants and permissions |
| `VirtoCommerce.WebHooksModule.Data` | Data | Service implementations, EF Core repository, caching, entity-model mappings |
| `VirtoCommerce.WebhooksModule.Data.SqlServer` | Data | SQL Server EF Core configurations and migrations |
| `VirtoCommerce.WebhooksModule.Data.MySql` | Data | MySQL EF Core configurations and migrations |
| `VirtoCommerce.WebhooksModule.Data.PostgreSql` | Data | PostgreSQL EF Core configurations and migrations |
| `VirtoCommerce.WebHooksModule.Web` | Web | REST API controllers, module bootstrapping and DI registration |
| `VirtoCommerce.WebHooksModule.Tests` | Tests | Unit tests |

### Key Services

| Service | Interface | Responsibility |
|---------|-----------|----------------|
| `WebHookManager` | `IWebHookManager` | Subscribes to domain events, orchestrates webhook notifications |
| `RetriableWebHookSender` | `IWebHookSender` | Sends HTTP POST requests with authentication and retry logic |
| `WebHookService` | `IWebhookService` | CRUD operations for webhook definitions |
| `WebHookSearchService` | `IWebHookSearchService` | Search and filter webhooks by criteria |
| `WebHookFeedService` | `IWebHookFeedService`, `IWebHookFeedSearchService`, `IWebHookFeedReader` | Execution log persistence and querying |
| `WebHookLogger` | `IWebHookLogger` | Thread-safe logging of webhook success and error outcomes |
| `RegisteredEventStore` | `IRegisteredEventStore` | Runtime discovery and caching of all domain event types |

### REST API

Base route: `api/webhooks`

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/{id}` | Get a webhook by ID |
| `POST` | `/search` | Search webhooks |
| `POST` | `/feed/search` | Search webhook feed entries |
| `DELETE` | `/feed` | Delete feed entries |
| `POST` | `/` | Create or update webhooks |
| `DELETE` | `/` | Delete webhooks |
| `POST` | `/send` | Test-fire a webhook |
| `GET` | `/events` | List all registered domain events |
| `GET` | `/properties` | Get event entity properties |

## Documentation

* [Webhooks module user documentation](https://docs.virtocommerce.org/platform/user-guide/webhooks/overview/)
* [REST API](https://virtostart-demo-admin.govirto.com/docs/index.html?urls.primaryName=VirtoCommerce.WebHooks)
* [View on GitHub](https://github.com/VirtoCommerce/vc-module-webhooks/)

## References

* [Deployment](https://docs.virtocommerce.org/platform/developer-guide/Tutorials-and-How-tos/Tutorials/deploy-module-from-source-code/)
* [Installation](https://docs.virtocommerce.org/platform/user-guide/modules-installation/)
* [Home](https://virtocommerce.com)
* [Community](https://www.virtocommerce.org)
* [Download latest release](https://github.com/VirtoCommerce/vc-module-webhooks/releases/latest)

## License

Copyright (c) Virto Solutions LTD.  All rights reserved.

This software is licensed under the Virto Commerce Open Software License (the "License"); you
may not use this file except in compliance with the License. You may
obtain a copy of the License at http://virtocommerce.com/opensourcelicense.

Unless required by the applicable law or agreed to in written form, the software
distributed under the License is provided on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
implied.
