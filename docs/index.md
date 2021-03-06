
# Overview

The webhooks module allows to register and send webhook notifications for any event available in the system.

## Key Features

1. Sending Webhook notifications in the background via a POST request with JSON serialized event data to the specified URL;  
1. Managing the list of webhooks  
![WebhookList](media/webhook-list.png)
1. Viewing or updating Webhook details;
![WebhookDetails](media/webhook-details.png)
1. Using `DomainEvent` descendant to trigger webhook notification;  
1. Sending retry policy with configurable exponential intervals  
![WebhookSettings](media/webhook-settings.png)
1. Viewing the list of errors for the webhook failed notifications  
![WebhookErrorList](media/webhook-error-list.png)

## Installation

You can find the module in the list of VirtoCommerce available modules.  
![WebhookInstall](media/webhook-install.png)

## How to configure

To create a webhook, you need to "Add" button in the webhook list, and fill in the following details:
- Name;
- URL to send the notification (e.g. the one from Azure LogicApp HttpResponse);
- Content type;
- The list of events you want this webhook be triggered for, or "Trigger all events" for any event;
Turn on "Is active" to make the webhook active, and Save. 
Now the webhook will trigger notification sending for the events you chose.

## Sample of Event JSON

<details><summary>Example of notification, sent on Order creation</summary>

```
{
  "EventId": "VirtoCommerce.Domain.Order.Events.OrderChangedEvent",
  "Attempt": 1,
  "EventBody": {
    "changedEntries": [
      {
        "entryState": "Added",
        "newEntry": {
          "customerId": "fef35378-ca50-4f0d-986d-f937e7bfb4d6",
          "customerName": "Anonymous",
          "storeId": "Electronics",
          "shoppingCartId": "4afe11ed0cb34a30aa87e40ebc7a40d8",
          "isPrototype": false,
          "addresses": [
            {
              "addressType": "Shipping",
              "name": "Customer name",
              "countryCode": "RUS",
              "countryName": "Russia",
              "city": "Perm",
              "postalCode": "614000",
              "line1": "Lenina, 22",
              "line2": "23",
              "regionId": "PER",
              "regionName": "Perm Krai",
              "firstName": "Name",
              "lastName": "Surname",
              "phone": "89844456312",
              "email": "Name@Surname.com"
            }
          ],
          "inPayments": [
            {
              "gatewayCode": "DefaultManualPaymentMethod",
              "paymentMethod": {
                "paymentMethodType": "Unknown",
                "paymentMethodGroupType": "Manual",
                "code": "DefaultManualPaymentMethod",
                "name": "Manual test payment method",
                "description": "Manual test, don't use on production",
                "logoUrl": "https://raw.githubusercontent.com/VirtoCommerce/vc-module-core/master/VirtoCommerce.CoreModule.Web/Content/logoVC.png",
                "isActive": true,
                "priority": 0,
                "isAvailableForPartial": false,
                "price": 0,
                "priceWithTax": 0,
                "total": 0,
                "totalWithTax": 0,
                "discountAmount": 0,
                "discountAmountWithTax": 0,
                "taxTotal": 0,
                "taxPercentRate": 0,
                "id": "1c18f2d633fc40f7ade58e868f56d514"
              },
              "customerId": "fef35378-ca50-4f0d-986d-f937e7bfb4d6",
              "paymentStatus": "New",
              "amount": 1516.8,
              "price": 0,
              "priceWithTax": 0,
              "total": 0,
              "totalWithTax": 0,
              "discountAmount": 0,
              "discountAmountWithTax": 0,
              "taxTotal": 0,
              "taxPercentRate": 0,
              "operationType": "PaymentIn",
              "number": "PI191121-00102",
              "isApproved": false,
              "currency": "USD",
              "sum": 1516.8,
              "isCancelled": false,
              "createdDate": "2019-11-21T09:09:12.069654Z",
              "modifiedDate": "2019-11-21T09:09:12.069654Z",
              "createdBy": "frontend",
              "modifiedBy": "frontend",
              "id": "26784beee4ed4d49a12b617292e611d0"
            }
          ],
          "items": [
            {
              "currency": "USD",
              "price": 1259,
              "priceWithTax": 1510.8,
              "placedPrice": 1259,
              "placedPriceWithTax": 1510.8,
              "extendedPrice": 1259,
              "extendedPriceWithTax": 1510.8,
              "discountAmount": 0,
              "discountAmountWithTax": 0,
              "discountTotal": 0,
              "discountTotalWithTax": 0,
              "fee": 0,
              "feeWithTax": 0,
              "taxTotal": 251.8,
              "taxPercentRate": 0.2,
              "reserveQuantity": 0,
              "quantity": 1,
              "productId": "4ed55441810a47da88a483e5a1ee4e94",
              "sku": "DJP3P",
              "productType": "Physical",
              "catalogId": "4974648a41df4e6ea67ef2ad76d7bbd4",
              "categoryId": "e51b5f9eea094a44939c11d4d4fa3bb1",
              "name": "DJI Phantom 3 Professional Quadcopter with 4K Camera and 3-Axis Gimbal",
              "imageUrl": "//webhookqa.blob.core.windows.net/catalog/DJP3P/1428511277000_1133098.jpg",
              "isGift": false,
              "isCancelled": false,
              "createdDate": "2019-11-21T09:09:12.069654Z",
              "modifiedDate": "2019-11-21T09:09:12.069654Z",
              "createdBy": "frontend",
              "modifiedBy": "frontend",
              "id": "91710eaf6b16466ea9019708fd39741c"
            }
          ],
          "shipments": [
            {
              "shipmentMethodCode": "FixedRate",
              "shipmentMethodOption": "Ground",
              "shippingMethod": {
                "code": "FixedRate",
                "name": "fixed shipping rate",
                "description": "Fixed rate shipping method",
                "logoUrl": "https://raw.githubusercontent.com/VirtoCommerce/vc-module-core/master/VirtoCommerce.CoreModule.Web/Content/logoVC.png",
                "isActive": true,
                "priority": 0,
                "settings": [
                  {
                    "moduleId": "VirtoCommerce.Core",
                    "groupName": "Commerce|General",
                    "name": "VirtoCommerce.Core.FixedRateShippingMethod.Air.Rate",
                    "value": "0.00000",
                    "rawValue": 0,
                    "valueType": "Decimal",
                    "defaultValue": "0.00",
                    "rawDefaultValue": 0,
                    "isArray": false,
                    "title": "Air shipping rate",
                    "description": "Fixed Air shipping rate",
                    "isRuntime": false
                  },
                  {
                    "moduleId": "VirtoCommerce.Core",
                    "groupName": "Commerce|General",
                    "name": "VirtoCommerce.Core.FixedRateShippingMethod.Ground.Rate",
                    "value": "0.00000",
                    "rawValue": 0,
                    "valueType": "Decimal",
                    "defaultValue": "0.00",
                    "rawDefaultValue": 0,
                    "isArray": false,
                    "title": "Ground shipping rate",
                    "description": "Fixed Ground shipping rate",
                    "isRuntime": false
                  }
                ],
                "id": "0fa858b127cf44098565fa896c26ed76"
              },
              "deliveryAddress": {
                "addressType": "Shipping",
                "name": "A K, Russia, Perm Krai, Perm st. Prover, 22, 23614000",
                "countryCode": "RUS",
                "countryName": "Russia",
                "city": "Perm",
                "postalCode": "614000",
                "line1": "st. Prover, 22",
                "line2": "23",
                "regionId": "PER",
                "regionName": "Perm Krai",
                "firstName": "A",
                "lastName": "K",
                "phone": "89844456312",
                "email": "a@a"
              },
              "price": 5,
              "priceWithTax": 6,
              "total": 5,
              "totalWithTax": 6,
              "discountAmount": 0,
              "discountAmountWithTax": 0,
              "fee": 0,
              "feeWithTax": 0,
              "taxTotal": 1,
              "taxPercentRate": 0.2,
              "operationType": "Shipment",
              "number": "SH191121-00102",
              "isApproved": false,
              "status": "New",
              "currency": "USD",
              "sum": 5,
              "isCancelled": false,
              "createdDate": "2019-11-21T09:09:12.069654Z",
              "modifiedDate": "2019-11-21T09:09:12.069654Z",
              "createdBy": "frontend",
              "modifiedBy": "frontend",
              "id": "66b4bac7f218411b8ebe9b3cfcb7cb45"
            }
          ],
          "discounts": [],
          "discountAmount": 0,
          "taxDetails": [],
          "total": 1516.8,
          "subTotal": 1259,
          "subTotalWithTax": 1510.8,
          "subTotalDiscount": 0,
          "subTotalDiscountWithTax": 0,
          "subTotalTaxTotal": 251.8,
          "shippingTotal": 5,
          "shippingTotalWithTax": 6,
          "shippingSubTotal": 5,
          "shippingSubTotalWithTax": 6,
          "shippingDiscountTotal": 0,
          "shippingDiscountTotalWithTax": 0,
          "shippingTaxTotal": 0,
          "paymentTotal": 0,
          "paymentTotalWithTax": 0,
          "paymentSubTotal": 0,
          "paymentSubTotalWithTax": 0,
          "paymentDiscountTotal": 0,
          "paymentDiscountTotalWithTax": 0,
          "paymentTaxTotal": 0,
          "discountTotal": 0,
          "discountTotalWithTax": 0,
          "fee": 0,
          "feeWithTax": 0,
          "feeTotal": 0,
          "feeTotalWithTax": 0,
          "taxTotal": 252.8,
          "taxPercentRate": 0,
          "languageCode": "en-US",
          "operationType": "CustomerOrder",
          "number": "CO191121-00102",
          "isApproved": false,
          "status": "New",
          "currency": "USD",
          "sum": 1516.8,
          "isCancelled": false,
          "createdDate": "2019-11-21T09:09:12.069654Z",
          "modifiedDate": "2019-11-21T09:09:12.069654Z",
          "createdBy": "frontend",
          "modifiedBy": "frontend",
          "id": "5d449fa326c44e06b1fbfae19d914dd6"
        },
        "oldEntry": {
          "customerId": "fef35378-ca50-4f0d-986d-f937e7bfb4d6",
          "customerName": "Anonymous",
          "storeId": "Electronics",
          "shoppingCartId": "4afe11ed0cb34a30aa87e40ebc7a40d8",
          "isPrototype": false,
          "addresses": [
            {
              "addressType": "Shipping",
              "name": "A K, Russia, Perm Krai, Perm st. Prover, 22, 23614000",
              "countryCode": "RUS",
              "countryName": "Russia",
              "city": "Perm",
              "postalCode": "614000",
              "line1": "st. Prover, 22",
              "line2": "23",
              "regionId": "PER",
              "regionName": "Perm Krai",
              "firstName": "A",
              "lastName": "K",
              "phone": "89844456312",
              "email": "a@a"
            }
          ],
          "inPayments": [
            {
              "gatewayCode": "DefaultManualPaymentMethod",
              "paymentMethod": {
                "paymentMethodType": "Unknown",
                "paymentMethodGroupType": "Manual",
                "code": "DefaultManualPaymentMethod",
                "name": "Manual test payment method",
                "description": "Manual test, don't use on production",
                "logoUrl": "https://raw.githubusercontent.com/VirtoCommerce/vc-module-core/master/VirtoCommerce.CoreModule.Web/Content/logoVC.png",
                "isActive": true,
                "priority": 0,
                "isAvailableForPartial": false,
                "price": 0,
                "priceWithTax": 0,
                "total": 0,
                "totalWithTax": 0,
                "discountAmount": 0,
                "discountAmountWithTax": 0,
                "taxTotal": 0,
                "taxPercentRate": 0,
                "id": "1c18f2d633fc40f7ade58e868f56d514"
              },
              "customerId": "fef35378-ca50-4f0d-986d-f937e7bfb4d6",
              "paymentStatus": "New",
              "amount": 1516.8,
              "price": 0,
              "priceWithTax": 0,
              "total": 0,
              "totalWithTax": 0,
              "discountAmount": 0,
              "discountAmountWithTax": 0,
              "taxTotal": 0,
              "taxPercentRate": 0,
              "operationType": "PaymentIn",
              "number": "PI191121-00102",
              "isApproved": false,
              "currency": "USD",
              "sum": 1516.8,
              "isCancelled": false,
              "createdDate": "2019-11-21T09:09:12.069654Z",
              "modifiedDate": "2019-11-21T09:09:12.069654Z",
              "createdBy": "frontend",
              "modifiedBy": "frontend",
              "id": "26784beee4ed4d49a12b617292e611d0"
            }
          ],
          "items": [
            {
              "currency": "USD",
              "price": 1259,
              "priceWithTax": 1510.8,
              "placedPrice": 1259,
              "placedPriceWithTax": 1510.8,
              "extendedPrice": 1259,
              "extendedPriceWithTax": 1510.8,
              "discountAmount": 0,
              "discountAmountWithTax": 0,
              "discountTotal": 0,
              "discountTotalWithTax": 0,
              "fee": 0,
              "feeWithTax": 0,
              "taxTotal": 251.8,
              "taxPercentRate": 0.2,
              "reserveQuantity": 0,
              "quantity": 1,
              "productId": "4ed55441810a47da88a483e5a1ee4e94",
              "sku": "DJP3P",
              "productType": "Physical",
              "catalogId": "4974648a41df4e6ea67ef2ad76d7bbd4",
              "categoryId": "e51b5f9eea094a44939c11d4d4fa3bb1",
              "name": "DJI Phantom 3 Professional Quadcopter with 4K Camera and 3-Axis Gimbal",
              "imageUrl": "//webhookqa.blob.core.windows.net/catalog/DJP3P/1428511277000_1133098.jpg",
              "isGift": false,
              "isCancelled": false,
              "createdDate": "2019-11-21T09:09:12.069654Z",
              "modifiedDate": "2019-11-21T09:09:12.069654Z",
              "createdBy": "frontend",
              "modifiedBy": "frontend",
              "id": "91710eaf6b16466ea9019708fd39741c"
            }
          ],
          "shipments": [
            {
              "shipmentMethodCode": "FixedRate",
              "shipmentMethodOption": "Ground",
              "shippingMethod": {
                "code": "FixedRate",
                "name": "fixed shipping rate",
                "description": "Fixed rate shipping method",
                "logoUrl": "https://raw.githubusercontent.com/VirtoCommerce/vc-module-core/master/VirtoCommerce.CoreModule.Web/Content/logoVC.png",
                "isActive": true,
                "priority": 0,
                "settings": [
                  {
                    "moduleId": "VirtoCommerce.Core",
                    "groupName": "Commerce|General",
                    "name": "VirtoCommerce.Core.FixedRateShippingMethod.Air.Rate",
                    "value": "0.00000",
                    "rawValue": 0,
                    "valueType": "Decimal",
                    "defaultValue": "0.00",
                    "rawDefaultValue": 0,
                    "isArray": false,
                    "title": "Air shipping rate",
                    "description": "Fixed Air shipping rate",
                    "isRuntime": false
                  },
                  {
                    "moduleId": "VirtoCommerce.Core",
                    "groupName": "Commerce|General",
                    "name": "VirtoCommerce.Core.FixedRateShippingMethod.Ground.Rate",
                    "value": "0.00000",
                    "rawValue": 0,
                    "valueType": "Decimal",
                    "defaultValue": "0.00",
                    "rawDefaultValue": 0,
                    "isArray": false,
                    "title": "Ground shipping rate",
                    "description": "Fixed Ground shipping rate",
                    "isRuntime": false
                  }
                ],
                "id": "0fa858b127cf44098565fa896c26ed76"
              },
              "deliveryAddress": {
                "addressType": "Shipping",
                "name": "A K, Russia, Perm Krai, Perm st. Prover, 22, 23614000",
                "countryCode": "RUS",
                "countryName": "Russia",
                "city": "Perm",
                "postalCode": "614000",
                "line1": "st. Prover, 22",
                "line2": "23",
                "regionId": "PER",
                "regionName": "Perm Krai",
                "firstName": "A",
                "lastName": "K",
                "phone": "89844456312",
                "email": "a@a"
              },
              "price": 5,
              "priceWithTax": 6,
              "total": 5,
              "totalWithTax": 6,
              "discountAmount": 0,
              "discountAmountWithTax": 0,
              "fee": 0,
              "feeWithTax": 0,
              "taxTotal": 1,
              "taxPercentRate": 0.2,
              "operationType": "Shipment",
              "number": "SH191121-00102",
              "isApproved": false,
              "status": "New",
              "currency": "USD",
              "sum": 5,
              "isCancelled": false,
              "createdDate": "2019-11-21T09:09:12.069654Z",
              "modifiedDate": "2019-11-21T09:09:12.069654Z",
              "createdBy": "frontend",
              "modifiedBy": "frontend",
              "id": "66b4bac7f218411b8ebe9b3cfcb7cb45"
            }
          ],
          "discounts": [],
          "discountAmount": 0,
          "taxDetails": [],
          "total": 1516.8,
          "subTotal": 1259,
          "subTotalWithTax": 1510.8,
          "subTotalDiscount": 0,
          "subTotalDiscountWithTax": 0,
          "subTotalTaxTotal": 251.8,
          "shippingTotal": 5,
          "shippingTotalWithTax": 6,
          "shippingSubTotal": 5,
          "shippingSubTotalWithTax": 6,
          "shippingDiscountTotal": 0,
          "shippingDiscountTotalWithTax": 0,
          "shippingTaxTotal": 0,
          "paymentTotal": 0,
          "paymentTotalWithTax": 0,
          "paymentSubTotal": 0,
          "paymentSubTotalWithTax": 0,
          "paymentDiscountTotal": 0,
          "paymentDiscountTotalWithTax": 0,
          "paymentTaxTotal": 0,
          "discountTotal": 0,
          "discountTotalWithTax": 0,
          "fee": 0,
          "feeWithTax": 0,
          "feeTotal": 0,
          "feeTotalWithTax": 0,
          "taxTotal": 252.8,
          "taxPercentRate": 0,
          "languageCode": "en-US",
          "operationType": "CustomerOrder",
          "number": "CO191121-00102",
          "isApproved": false,
          "status": "New",
          "currency": "USD",
          "sum": 1516.8,
          "isCancelled": false,
          "createdDate": "2019-11-21T09:09:12.069654Z",
          "modifiedDate": "2019-11-21T09:09:12.069654Z",
          "createdBy": "frontend",
          "modifiedBy": "frontend",
          "id": "5d449fa326c44e06b1fbfae19d914dd6"
        }
      }
    ],
    "version": 0,
    "timeStamp": "2019-11-21T09:09:12.1946097Z",
    "id": "8dfe4e36-6f2c-43fc-88d2-fab8f1a9366b"
  }
}
```
</details>