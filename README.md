# Event Horizon Advice Group - Web API

A comprehensive .NET 8 Web API solution for advisor/client management and commission processing using Onion architecture.

## Architecture

- **Domain Layer**: Entities, enums, value objects, and commission calculator
- **Application Layer**: Services, DTOs, validators, and AutoMapper profiles
- **Infrastructure Layer**: EF Core, repositories, Unit of Work, and data seeding
- **API Layer**: Controllers, authentication, Swagger, and middleware

## Features

### Core Business
- **Commission Processing**: Intelligent APE vs Receipts calculation with configurable rates
- **Payment Management**: Full payment lifecycle with approval workflows
- **Policy Management**: Product configuration with split percentages
- **Analytics**: Time-series data and product mix reporting

### User Management
- **Role-Based Access**: Advisor, Manager, ReferralPartner, Administrator, Client
- **Scoped Data Access**: Users see only authorized data based on their role
- **Demo Authentication**: Simple header-based auth for development

### Document Management
- **File Upload/Download**: Secure local file storage
- **E-Signature Workflow**: Mock signing process with HTML interface
- **Access Control**: Owner-based document permissions

### Additional Features
- **Ticketing System**: Support ticket management with messaging
- **Notifications**: Role-based notification system
- **Public Forms**: Job applications and website inquiries
- **Audit Logging**: Complete activity tracking

## Quick Start

1. **Run the API**:
   ```bash
   cd EventHorizon.Api
   dotnet run
   ```

2. **Access Swagger UI**: https://localhost:7000/swagger

3. **Demo Authentication**:
   ```bash
   # Login as advisor
   curl -X POST https://localhost:7000/auth/demo-login \
     -H "Content-Type: application/json" \
     -d '{"email":"sarah.johnson@event-horizon.test","role":"Advisor"}'
   
   # Use returned headers for subsequent requests
   curl -X GET https://localhost:7000/payments \
     -H "X-User-Email: sarah.johnson@event-horizon.test" \
     -H "X-User-Role: Advisor"
   ```

## API Endpoints

### Authentication
- `POST /auth/demo-login` - Demo login
- `GET /auth/session` - Current user session

### Business Operations
- `GET /policies` - List policies
- `GET /payments` - List payments (role-filtered)
- `POST /payments` - Create payment with commission calculation
- `GET /reports/commission-details` - Detailed commission breakdown
- `GET /analytics/series` - Time-series analytics
- `GET /analytics/product-mix` - Product performance data

### Document Management
- `POST /documents` - Upload document
- `GET /files/{id}` - Download file (secure)
- `POST /esignature/requests` - Create signature request
- `GET /sign/{token}` - Sign document (HTML interface)

### Public Endpoints
- `POST /public/jobs/apply` - Job application (with CV upload)
- `POST /public/inquiries` - Website inquiry
- `GET /public/jobs` - Open job postings

## Sample Data

The system seeds with comprehensive test data:

### Users
- **Advisors**: sarah.johnson@event-horizon.test, michael.carter@event-horizon.test
- **Manager**: manager@event-horizon.test
- **Admin**: admin@event-horizon.test
- **Client**: client@event-horizon.test

### Products
- Term Life (PRD-TERM): 7.5% rate, 10% margin
- Critical Illness (PRD-CI): 8.0% rate, 12% margin
- Whole of Life (PRD-WOL): 6.5% rate, 8% margin
- Income Protection (PRD-IP): 7.0% rate, 10% margin

### Test Data
- 50+ payments across 18 months
- Goals with 6-month history
- Sample documents and signature requests
- Tickets with message threads
- Job postings and inquiries

## Commission Calculation

The system uses intelligent commission calculation:

```csharp
threshold = policy.ThresholdMultiplier * payment.APE
if (payment.Receipts <= threshold) {
    method = "APE"
    base = APE * (rate/100)
} else {
    method = "Receipts" 
    base = Receipts * (rate/100)
}
pool = base * (1 - margin/100)
shares = pool * split% (advisor/introducer/manager/exec)
```

## Role-Based Access Control

- **Advisor**: Own payments/reports only
- **Manager**: All advisors, can filter by email
- **Administrator**: Full system access
- **Client**: Own tickets/notifications only

## Technology Stack

- .NET 8 / C#
- ASP.NET Core Web API
- Entity Framework Core + SQLite
- AutoMapper
- FluentValidation
- Swagger/OpenAPI
- CORS enabled for frontend integration

## Health Check

- `GET /health` - API health status

The API is production-ready with comprehensive error handling, validation, pagination, and security controls.