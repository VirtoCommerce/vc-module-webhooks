# Virto Commerce Webhook Module

The Webhooks module allows you to monitor important changes within your Virto Commerce ecosystem,
such as order changes, catalog and product updates, and more.
When a change you're subscribed to is triggered, you'll receive a notification at the URL you specified.
You can also configure which fields or parameters to include in the report you receive.

## Key Features

1. Employees can manage webhooks under their own permission level
1. Admin users can manage webhooks
1. Resolves Virto Commerce Domain Events for installed modules
1. Sends webhook notifications in the background via a POST request with JSON serialized event data to the specified URL
1. Supports Basic & Bearer Token authentication
1. Access to previous values of selected fields
1. Configurable retry policy with exponential intervals
1. View error messages when a webhook notification fails

## Documentation

* [Module Documentation](https://docs.virtocommerce.org/modules/webhooks/)
* [View on GitHub](docs/index.md)

## References

* Deployment: https://docs.virtocommerce.org/developer-guide/deploy-module-from-source-code/
* Installation: https://docs.virtocommerce.org/user-guide/modules/
* Home: https://virtocommerce.com
* Community: https://www.virtocommerce.org
* [Download Latest Release](https://github.com/VirtoCommerce/vc-module-webhooks/releases/latest)

## License

Copyright (c) Virto Solutions LTD.  All rights reserved.

This software is licensed under the Virto Commerce Open Software License (the "License"); you
may not use this file except in compliance with the License. You may
obtain a copy of the License at http://virtocommerce.com/opensourcelicense.

Unless required by the applicable law or agreed to in written form, the software
distributed under the License is provided on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
implied.
