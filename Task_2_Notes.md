Task 2: Update Appointment Status

Implemented endpoint to update appointment status.
`PATCH /api/appointment/{id}/status`

Technical Decisions

- Used PATCH for partial updates
- Created generic Result pattern with ErrorTypeEnum for clean error handling
- ResultHelper maps business errors to HTTP status codes
- `AppointmentStatusHelper`: Handles status migration and validation
- `MockNotificationService`: email notifications for cancellations

Business Rules Implementation

- Status migration: InProgress→Scheduled, NoShow→Cancelled
- Only 3 valid statuses allowed: Scheduled, Completed, Cancelled
- Cannot cancel within 1 hour of appointment start time
- Email notification sent when appointment is cancelled

Technical Details

- Service returns Result with specific error types (NotFound, ValidationError, BusinessRuleViolation)
- Controller uses one-line error handling: `ResultHelper.ToActionResult(result)`
- DateTime.Now passed from controller to service for better testability

Assumptions

- Any user can update appointment status (no authentication/authorization implemented)
- Cannot determine if user is a veterinarian without auth system
- Mock email to console is sufficient
- Status migration is transparent to the user
- All DateTime values are in the same timezone
