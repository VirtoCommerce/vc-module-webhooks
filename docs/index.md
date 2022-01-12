
# Webhooks Module

## What Is Webhooks Module?

Simply put, the Webhooks module enables viewing all major changes within your Virto Commerce ecosystem. For instance, you can get notified on user login and/or password changes, catalog or product changes, and so on, which will assist you in better monitoring your system. Whenever a change you are watching (i.e. you 'subscribed to') is triggered, you will get the relevant notification uploaded to the URL address you previously specified. Apart from the event you are following, you can also configure which fields or parameters you will see in the report you get.

## Key Features

Using the Webhooks module basically enables you to do the following:
1. Sending Webhook notifications in the background via a POST request with JSON serialized event data to the specified URL
1. Managing webhooks
1. Viewing or updating Webhook details
1. Using `DomainEvent` descendant to trigger webhook notifications
1. Sending the retry policy with configurable exponential intervals
1. Viewing the error(s) when a webhook notification fails

## Configuring Webhooks

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

## Warning Message for Webhooks Created with Previous Versions
If you used webhooks with multiple event subscription in the previous version, you might get the following warning message once you update the Virto platform:

![Incorrect webhook warning](./media/incorrect-webhook-warnings.png)

This is actually fine: you can continue using such a webhook as you did before, although **you will not be able edit it**. However, we recommend you removing such webhooks and replacing them with new ones instead. Currently, the *single event per webhook* limitation works for the newly created webhooks only, but, moving forward, we might totally remove the code that supports multiple event subscription.

## Example of Output JSON File

<details><summary>Please expand this paragraph to see an example of JSON output for a weebhook based on the User Changed event.</summary>

```
{
  "EventId": "VirtoCommerce.Platform.Core.Security.Events.UserChangedEvent",
  "Attempt": 1,
  "EventBody": "[
  {
  "ObjectType":"VirtoCommerce.Platform.Core.Security.ApplicationUser",
  "MemberId":"cb0a5340-f9fb-4f49-bd62-9d03518868ff",
  "StoreId":"B2B-store",
  "IsAdministrator":false,
  "Id":"78b0208a-bb52-4a33-9250-583d63aa1f77"
  }
]"
}

NOTE: You can then call the User API and get the user by its ID using this request: GET /api/users/id/{id}
```
</details>
