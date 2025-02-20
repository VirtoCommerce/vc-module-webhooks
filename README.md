# Virto Commerce Webhooks Module

The Webhooks module allows you to monitor important changes within your Virto Commerce ecosystem, such as order changes, catalog and product updates, and more.
When a change you're subscribed to is triggered, you'll receive a notification at the URL you specified.
You can also configure which fields or parameters to include in the report you receive.

## Key features

* Employees can manage webhooks under their own permission level
* Admin users can manage webhooks
* Resolves Virto Commerce Domain Events for installed modules
* Sends webhook notifications in the background via a POST request with JSON serialized event data to the specified URL
* Supports Basic & Bearer Token authentication
* Access to previous values of selected fields
* Configurable retry policy with exponential intervals
* View error messages when a webhook notification fails

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
