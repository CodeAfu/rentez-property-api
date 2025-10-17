# RentEZ Property

[http://localhost:5115/scalar](http://localhost:5115/scalar)

## Dependencies

```bash
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package DocuSign.eSign.dll
```

Property Management System

## Task 1

### Features

**Tenant Features**

- Find Rooms
- Contact Agent
- Sign Agreement Digitally
- Rent Payment
  - Reminder Notification
  - Automatic Payment

**Owner Features**

- Room Listing
  - Add Pictures
  - Provide Address
  - Add Agreement (Store in S3)
  - Assign Agent (Optional?)
- Screen Tenants

**Rent Management Features**

- Auto Monthly Payment (Stripe)
- Payment History
- Agreement Renewal

**Lease Management Features**

- E-Signature (DocuSeal)

[DocuSeal API](https://www.docuseal.com/docs/api)

## Task 2

BI
