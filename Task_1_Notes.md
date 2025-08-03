Task 1: List Vet Appointments

Implemented a RESTful endpoint to retrieve appointments for a specific veterinarian within a date range.
`GET /api/appointment/veterinarian/{veterinarianId}/appointments?startDate={date}&endDate={date}`

- Added service layer (`AppointmentService`) to separate business logic from controllers
- Created response DTO (`AppointmentSummaryResponse`)
- Implemented reusable `DateRangeHelper` for overlap detection logic

Details

- Date range overlap detection ensures all relevant appointments are returned
- Proper validation for required parameters and date range logic
- RESTful route structure
- XML documentation for Swagger UI integration

Technical Decisions

- Used `required` modifier on Appointment model properties to enforce data integrity
- Kept parameterless constructor with `[Obsolete]` attribute for backward compatibility
- Results ordered by start time for better usability

The endpoint successfully:

- Returns filtered appointments with animal and owner information
- Validates that end date is not before start date
- Returns 400 Bad Request for missing or invalid parameters
- Returns empty array when no appointments match criteria

Assumptions

- All DateTime values are in the same timezone
- I changed initial AppointmentData, because the appointments were missing required VeterinarianId and StartTime/EndTime values, just doesn't make sense to have such data from my point of view.
