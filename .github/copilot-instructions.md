# Copilot Instructions

## Project Guidelines
- Do not remove failing/red tests just because they are failing; keep them when they are important for exposing gaps in the implementation.
- Start with a short plan before implementation when the task is uncertain or spans multiple steps.
- Ask clarifying questions to pin down the requested feature shape and expected behavior before coding.
- Write or update a failing unit test first whenever the change is behavior-related.
- Implement the feature after the test is in place.
- Verify the change by running the relevant unit tests after implementation.
- Ignore `ApiChanges.PublicChanges`; it is expected to be run manually by the user.
