# Repository Guidelines

This project contains multiple ASP.NET Core applications. Use the following rules when creating pull requests or automated changes.

## Development checks

- Run `dotnet build ProjectTracker.sln` to ensure the solution compiles. No test projects are included.
- Formatting should follow the existing style: 4-space indentation and braces on a new line.
- Where available, `dotnet format` may be used to automatically fix style issues.

## Commit messages

- Provide a short summary in the imperative mood ("Add feature" not "Added feature").
- Include a brief description of the main modules you touched.

## Pull request description

- Summarize the changes and mention any build or manual steps required.
- List any known limitations or follow-up work.
