
# Overview

The Webhooks module enables viewing all major changes within your Virto Commerce ecosystem.

For instance, you can get notified on user login and/or password changes, catalog or product changes, and so on, which will assist you in better monitoring your system. Whenever a change you are watching (i.e. you 'subscribed to') is triggered, you will get the relevant notification uploaded to the URL address you previously specified.

Apart from the event you are following, you can also configure which fields or parameters you will see in the report you get.

## Key Features

1. Employees can manage webhooks but only under their own Permission level
1. Admin users can manage webhooks
1. Resolving Virto Commerce Domain Event for installed modules
1. Sending Webhook notifications in the background via a POST request with JSON serialized event data to the specified URL
1. Access to previous values of selected fields
1. Sending the retry policy with configurable exponential intervals
1. Viewing the error(s) when a webhook notification fails

## Configuraton

To configure a new webhook, open the *Webhooks* module and click the *Add* button:

![Adding a new webhook](./media/01-adding-a-new-webhook.png)

This will open the *Webhook List* tab:

![Webhook List tab](./media/02-webhook-list.png)

Give your webhook a name and select the event you want to follow; for instance, you may want to get notified when a user profile has been changed, a user has logged in or out, or has reset their password:

![Selecting an event](./media/03-selecting-an-event.png)

***Please note:*** *You can only select **one** event per webhook. This means that, if you want to track, say, both the **User Logged In** and **User Logged Out** events, you will have to create two different webhooks. You can, however, create any reasonable number of webhooks you need.*

In the *Additional Fields* area, you can select up to ten additional parameters you will see in the notification you will get with this webhook:

![Selecting additional fields](./media/04-additional-fields.png)

By default, only the *ID* and *ObjectType* properties will be included into the report. If you do not want any additional parameters to appear, just skip this step.

Finally, toggle the *Activate* button to get your webhook running or leave it deactivated if you want to enable it later, and specify the URL you will get the report at:

![Activating your webhook and providing the URL](./media/05-activation-and-url.png)

This is it. You can now click the *Save* button in the upper part of the tab to finalize your webhook configuration:

![Saving your webhook](./media/06-saving-webhook.png)

Once you do so, you will be able to see your new webhook in the list:

![Webhook appearing in the list](./media/07-webhook-appearing-in-the-list.png)

Alternatively, you can click the *Reset* button to reconfigure your webhook from scratch.

## Warning Message
If you use webhooks with multiple event subscriptions in the previous version, you might get the following warning message once you update the Virto platform:

![Incorrect webhook warning](./media/incorrect-webhook-warnings.png)

This is actually fine: you can continue using such a webhook as you did before, although **you will not be able to edit it**. However, we recommend you remove such webhooks and replace them with new ones instead. Currently, the *single event per webhook* limitation works for the newly created webhooks only, but, moving forward, we might totally remove the code that supports multiple event subscriptions.

## Webhook Json Format

<details><summary>Please expand this paragraph to see an example of JSON output for a webhook based on the Order Changed event.</summary>

```
{
  "EventId": "VirtoCommerce.OrdersModule.Core.Events.OrderChangedEvent",
  "Attempt": 1,
  "EventBody": [
    {
      "ObjectType": "VirtoCommerce.OrdersModule.Core.Model.CustomerOrder",
      "Id": "1d58ff39-0631-44aa-afc6-a0d4adbc9787",
      "Status": "New",
      "Number": "CO220224-00003",
      "__Previous": {
        "Status": "Pending",
        "Number": "CO220224-00003"
      }
    }
  ]
}


NOTE: You can then call the Order API and get the extended order information.

NOTE: `__Previous` field contains the same list of the fields with previous values.

```
</details>

## References
* We recommend using [webhook.site](https://webhook.site/) to inspect and test web hooks.
